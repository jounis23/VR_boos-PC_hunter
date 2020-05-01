using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : Entity
{

    private float moveX;
    private float moveY;

    private bool canMoveX = true;
    private bool canMoveY = true;

    
    //public Rigidbody rigid;


    // 각 직업별 스킬 구현 클래스
    public ClassEntity classEntity;

    void Start()
    {
        InitPlayer();
    }

    public void InitPlayer()
    {
        state = STATE.IDLE;
        //rigid = GetComponent<Rigidbody>();
        //SetStatus("PlayerA", 100, 10, 10, 0.5f);
    }


    void Update()
    {
        /*
        if (!photonView.IsMine)
            return;
            */

        // 스킬 쿨타임 계산
        SkillTime();

        // 움직임 키 입력받기
        InputMovement();

        // 행동 키 입력받기
        // (움직임 외의 행동)
        InputAction();
    }





    // 스킬 쿨타임 제어
    // UI에 스킬 쿨타임 표시
    public void SkillTime()
    {
        for (int i = 0; i < skillresetTime.Length; i++)
        {
            if (skillresetTime[i] > 0)
                skillresetTime[i] -= Time.deltaTime;
            else if (skillresetTime[i] < 0)
                skillresetTime[i] = 0;

            skillImage[i].fillAmount = 1 - (skillresetTime[i] / skillCoolTime[i]);
            skillText[i].text = skillresetTime[i].ToString("F1");
        }
    }





    // 키보드 입력에 의한 움직임값 설정
    // 여러 키를 입력하더라도 상하, 좌우중 한쪽으로만 이동
    public void InputMovement()
    {
        if (canMoveX)
            moveX = Input.GetAxis("Horizontal");
        else
            moveX = 0;

        if (canMoveY)
            moveY = Input.GetAxis("Vertical");
        else
            moveY = 0;


        if (canMoveX && Mathf.Abs(moveX) > Mathf.Abs(moveY) && Mathf.Abs(moveY) > 0.1f)
        {
            canMoveX = false;
            moveX = 0;
        }
        else if (canMoveY && Mathf.Abs(moveY) > Mathf.Abs(moveX) && Mathf.Abs(moveX) > 0.1f)
        {
            canMoveY = false;
            moveY = 0;
        }
        else if (!canMoveY && (Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f))
        {
            canMoveY = true;
        }
        else if (!canMoveX && (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f))
        {
            canMoveX = true;
        }
        else if (Mathf.Abs(moveX) < 0.1f && Mathf.Abs(moveY) < 0.1f)
        {
            canMoveY = true;
            canMoveX = true;
        }
    }





    // 키입력에 따른 특정 행동 실행
    private void InputAction()
    {
        // 움직임
        if (Mathf.Abs(moveX) > 0.5f || Mathf.Abs(moveY) > 0.5f)
        {
            if (Act(STATE.RUN))
            {
                Move(moveX, moveY);
            }
        }
        else
        {
            Act(STATE.IDLE);
        }


        // 점프
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Act(STATE.JUMP))
            {

            }
        }

        // 기본공격
        if (Input.GetMouseButtonDown(0))
        {
            if (Act(STATE.ATTACK))
            {

                classEntity.Attack(this);
            }
        }
        
        // 스킬1
        if (Input.GetKeyDown(skillKey[0]))
        {
            if (skillresetTime[0] == 0 && Act(classEntity.type[0]))
            {
                if (classEntity.type[0] == STATE.CASTING)
                    classEntity.CASTING(this, 0);
                else
                {
                    skillresetTime[0] = skillCoolTime[0];
                    classEntity.Skill1(this);

                }
            }

        }


        // 스킬2
        if (Input.GetKeyDown(skillKey[1]))
        {
            if (skillresetTime[1] == 0 && Act(classEntity.type[1]))
            {
                if (classEntity.type[1] == STATE.CASTING)
                    classEntity.CASTING(this, 1);
                else
                {
                    skillresetTime[1] = skillCoolTime[1];
                    classEntity.Skill2(this);

                }
            }
        }


        // 스킬3
        if (Input.GetKeyDown(skillKey[2]))
        {
            if (skillresetTime[2] == 0 && Act(classEntity.type[2]))
            {
                if (classEntity.type[2] == STATE.CASTING)
                    classEntity.CASTING(this, 2);
                else
                {
                    skillresetTime[2] = skillCoolTime[2];
                    classEntity.Skill3(this);

                }
            }
        }

        // 스킬4
        if (Input.GetKeyDown(skillKey[3]))
        {
            if (skillresetTime[3] == 0 && Act(classEntity.type[3]))
            {
                if (classEntity.type[3] == STATE.CASTING)
                    classEntity.CASTING(this, 3);
                else
                {
                    skillresetTime[3] = skillCoolTime[3];
                    classEntity.Skill4(this);
                }
            }
        }
    }





    // 행동 구형
    // 애니메이션 실행
    public bool Act(STATE nextState)
    {
        if (state != STATE.IDLE && state != STATE.RUN)
            return false;
        else if (nextState == STATE.RUN)
        {
            animator.SetFloat("RunFwd", moveY);
            animator.SetFloat("RunRight", moveX);
            state = STATE.RUN;
            return true;
        }
        else if (nextState == STATE.IDLE)
        {
            animator.SetFloat("RunFwd", 0);
            animator.SetFloat("RunRight", 0);
            state = STATE.IDLE;

            return true;
        }
        else if(nextState == STATE.CASTING)
        {
            state = STATE.CASTING;
            animator.SetTrigger("CASTING");
            return true;
        }
        else if (state == nextState)
            return true;


        animator.SetTrigger(nextState.ToString());
        state = nextState;
        Debug.Log(state.ToString());

        switch (nextState)
        {
            case STATE.ATTACK:
            case STATE.SKILL1:
            case STATE.SKILL2:
            case STATE.SKILL3:
            case STATE.SKILL4:
            case STATE.ATTACKED:
            case STATE.JUMP:
                StartCoroutine(AnimationState());
                break;
            case STATE.RUN:
                break;
            default:
                break;
        }

        return true;
    }





    // 타이밍에 맞게 애니메이션 실행
    // 모든 애니메이션은 끝나면 IDLE 상태로 돌아옴
    IEnumerator AnimationState()
    {
        yield return new WaitForSeconds(1f);
        float totalTime = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(totalTime - 0.75f);
        state = STATE.IDLE;
        animator.SetTrigger("IDLE");
    }
}
