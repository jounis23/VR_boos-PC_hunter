using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hunter : Entity
{



    // 각 직업별 스킬 구현 클래스
    public ClassEntity classEntity;

    // 스킬 입력키 설정 (기본 1,2,3,4로 됨)
    public KeyCode[] skillKey = new KeyCode[4];

    // 자신의 UI
    // HP,MP
    // 팀원과 보스의 UI
    public HunterUI hunterUI;


    public bool isStart = false;


    private float moveX;
    private float moveY;


    private float mpRecover = 0f;
    private float turnValueRight = 0f;
    private float turnValueForward = 0f;
    private float turnSpeed = 1080f;
    private Quaternion targetRotation;


    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 초기화
    void Start()
    {

        if (!photonView.IsMine && isNetwork)
            mainCam.gameObject.SetActive(false);
        //InitPlayer();

    }

    public void Initialized()
    {
        isStart = true;
        state = STATE.IDLE;

        hunterUI.Initialized();
    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 업데이트
    void Update()
    {
        if (state == STATE.DIE || !isStart)
            return;

        if (!photonView.IsMine && isNetwork)
        {
            return;
        }

        float animationSpeed = 1 + (status.speed / 200);
        if (animationSpeed > 1.3f)
            animationSpeed = 1.3f;
        // 케릭터의 스피드에 따라 애니메이션 속도 조절 (최대 1.3배)
        animator.speed = animationSpeed;

        // 행동 키 입력받기
        // (움직임 외의 행동)
        InputAction();

        // 움직임 키 입력받기
        InputMovement();

        // 일정 시간마다 hp,mp회복
        Recovery();

        // 스킬 쿨타임 계산
        // 쿨타임에 대한 UI포함
        hunterUI.UpdateSkillCoolTime(classEntity.skillresetTime, classEntity.skillCoolTime);

    }


    public void UpdateTeamStatusUI(float[] teamhp, float[] teammp, float bossHp)
    {
        hunterUI.UpdateTeamStatusUI(teamhp, teammp, bossHp);
    }
    public void UpdateTeamStatusUIClass(string[] data)
    {
        hunterUI.UpdateTeamStatusUIClass(data);
    }

    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 플레이어는 일정 시간마다 hp,mp가 회복 됨
    private void Recovery()
    {

        mpRecover += Time.deltaTime;
        if (mpRecover > 1)
        {
            mpRecover = 0;
            UpdateMp(status.mpMax * status.recovery / 20f, false);
            UpdateHp(status.hpMax * status.recovery / 40f, false);
        }

    }









    //#############################################################################



    public void UpdateHp(float amount, bool isEffect = false)
    {
        base.UpdateHp(amount, isEffect);

        hunterUI.UpdateHpUI();
    }
    public void UpdateMp(float amount, bool isEffect = false)
    {
        base.UpdateMp(amount, isEffect);

        hunterUI.UpdateMpUI(); 
    }
    public override void Attacked(float damage, bool isTrueDamage = false, string attacker = "None")
    {


        base.Attacked(damage, isTrueDamage, attacker);

        hunterUI.UpdateHpUI();
        hunterUI.AttackedText(damage);

        SetActiveAura("Attacked");

    }



    //#############################################################################

    // 키보드 입력에 의한 움직임값 설정
    public void InputMovement()
    {
        if (state != STATE.IDLE && state != STATE.RUN)
        {
            moveX = 0;
            moveY = 0;
            return;
        }


        // W,S : 수직이동 (정면, 후면으로 직진)
        moveY = Input.GetAxis("Vertical");


        // A,D : 수평이동 (좌, 우로 90도 턴 한 후 직진)
        moveX = Input.GetAxis("Horizontal");




        if (moveY > 0.5f)
            turnValueForward = 1;
        else if (moveY < -0.5f)
            turnValueForward = -1;
        else
            turnValueForward = 0;

        // 오른쪽 이동
        if (moveX > 0.5f)
            turnValueRight = 1;
        // 왼쪽 이동
        else if (moveX < -0.5f)
            turnValueRight = -1;
        else
            turnValueRight = 0;



        // 정면+우측
        if (turnValueForward > 0.5f && turnValueRight > 0.5f)
        {
            Vector3 dir = (mainCam.transform.forward + mainCam.transform.right).normalized;
            targetRotation = Quaternion.LookRotation(dir);
        }
        // 정면+좌측
        else if (turnValueForward > 0.5f && turnValueRight < -0.5f)
        {
            Vector3 dir = (mainCam.transform.forward - mainCam.transform.right).normalized;
            targetRotation = Quaternion.LookRotation(dir);
        }



        // 후면+우측
        else if (turnValueForward < -0.5f && turnValueRight > 0.5f)
        {
            return;
        }
        // 후면+좌측
        else if (turnValueForward < -0.5f && turnValueRight < -0.5f)
        {
            return;
        }



        // 정면
        else if (turnValueForward > 0.5f)
        {
            targetRotation = Quaternion.LookRotation(mainCam.transform.forward);
        }
        // 후면
        else if (turnValueForward < -0.5f)
        {
            targetRotation = Quaternion.LookRotation(mainCam.transform.forward);
        }
        // 우측
        else if(turnValueRight > 0.5f)
        {
            targetRotation = Quaternion.LookRotation(mainCam.transform.right);
        }
        // 좌측
        else if(turnValueRight < -0.5f)
        {
            targetRotation = Quaternion.LookRotation(-mainCam.transform.right);
        }
        else
        {
            targetRotation = Quaternion.identity;
        }

        if(targetRotation != Quaternion.identity)
            player.transform.rotation = Quaternion.RotateTowards(player.transform.rotation, targetRotation, turnSpeed * Time.deltaTime);




        // 움직임에 따른 애니메이션 실행
        // 스크립트가 아닌 애니메니션 자체의 이동 값으로 움직이게 함
        if (Mathf.Abs(moveY) > 0.5f || Mathf.Abs(moveX) > 0.5f)
        {
            //Debug.Log(moveY);
            if (ActAnimation(STATE.RUN))
            {
                //Move(moveY, 0);
            }else if (turnValueForward != 0 || turnValueRight != 0 )
            {
                //Move(moveY, 0);

            }
        }
        else
        {
            ActAnimation(STATE.IDLE);
        }

    }


    //#############################################################################

    // 키입력에 따른 특정 행동 실행
    private void InputAction()
    {


        // 기본 공격
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
        {
            if (ActAnimation(STATE.ATTACK))
            {
                classEntity.Attack();
            }
        }

        // 스킬 공격
        // 입력키 : 1,2,3,4
        for(int i = 0; i<skillKey.Length; i++)
        {
            if (Input.GetKeyDown(skillKey[i]))
            {

                // 남은 쿨타임에 따라 실행
                if(!classEntity.EnableSkill(i))
                    continue;

                // 스킬의 종류 (SKILL1,2,3,4, CASTING)
                STATE skillState = classEntity.skill[i].state;

                // 애니메이션 실행, 스킬 실행
                if (!classEntity.skill[i].isCasting && ActAnimation(skillState))
                {
                    classEntity.Skill(i);
                }
                else if(classEntity.skill[i].isCasting)
                {
                    classEntity.Skill(i);
                }

            }
        }

        

        if (!Input.anyKey && state != STATE.IDLE)
        {
            ActAnimation(STATE.IDLE);
        }


    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 케릭터의 행동 구현
    // 다음 상태를 체크하여 실행 여부와 애니메이션을 실행
    public bool ActAnimation(STATE nextState)
    {

        if (nextState != STATE.RUN && nextState == state)
        {
            return false;

        }

        // 기본적으로 상태를 바꿀 수 잇는 것은 IDLE이거나 RUN 일 때
        else if (state != STATE.IDLE && state != STATE.RUN)
        {
            return false;
        }


        // 현재 상태는 다음 상태가 됨
        state = nextState;


        // 다음 상태가 RUN 일떄
        if (nextState == STATE.RUN)
        {
            // *수평키 입력이라도 애니메이션은 정면으로 가는것으로 실행
            //      수평키 입력 시 케릭터가 즉시 좌, 우 90도로 회전 후 직진
            // *수직키 입력 시 전 후로 이동
            if (Mathf.Abs(moveX) > 0.5f)
                animator.SetFloat("RunFwd", 1);
            else if (Mathf.Abs(moveY)>0.5f)
                animator.SetFloat("RunFwd", moveY);

            return true;
        }

        // 다음 상태가 IDLE 일떄
        // 평상시로 돌아 갈때는 움직임 애니메이션 값 초기화
        else if (nextState == STATE.IDLE)
        {
            animator.SetFloat("RunFwd", 0);

            return true;
        }

        // 다음 상태가 IDLE 일떄
        // 평상시로 돌아 갈때는 움직임 애니메이션 값 초기화
        else if (nextState == STATE.CASTING)
        {
            animator.SetFloat("RunFwd", 0);

            return true;
        }

        // 그 외의 경우
        else
        {
            // 움직이는 상태(RUN)는 위에서 걸렀기 때문에 움직임 멈춤
            animator.SetFloat("RunFwd", 0);
            //animator.SetFloat("RunRight", 0);

            // 다음 상태에 대한 애니메이션 실행
            // 각 애니메이션에 대한 trigger 파라미터 명은 상태 명으로 통일됨
            animator.SetTrigger(nextState.ToString());
            //Debug.Log(state.ToString());

            switch (nextState)
            {
                case STATE.ATTACK:
                case STATE.SKILL1:
                case STATE.SKILL2:
                case STATE.SKILL3:
                case STATE.SKILL4:
                case STATE.ATTACKED:
                case STATE.CASTING:
                    StartCoroutine(AnimationState());
                    break;
            }

            return true;
        }

        
    }



    //#############################################################################

    // 타이밍에 맞게 애니메이션 실행
    // 모든 애니메이션은 끝나면 IDLE 상태로 돌아옴
    IEnumerator AnimationState()
    {
        yield return new WaitForSeconds(0.5f);

        // 현재 실행중인 애니메이션의 시간
        float totalTime = animator.GetCurrentAnimatorStateInfo(0).length;

        // 위 WaitForSeconds와 합쳐 총 0.25f만큼의 후반 딜레이를 가짐
        yield return new WaitForSeconds(totalTime - 0.25f);

        // 평상시 상태로 돌아옴
        state = STATE.IDLE;
        animator.SetTrigger("IDLE");
    }



}
