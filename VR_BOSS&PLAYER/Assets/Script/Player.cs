using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Player : Entity
{
    private float moveX;
    private float moveY;

    private Rigidbody rigid;

    public Image[] skill = new Image[5];
 


    void Start()
    {
        InitPlayer();
    }

    public void InitPlayer()
    {
        rigid = GetComponent<Rigidbody>();
        SetStatus("PlayerA", 100, 10, 10, 0.5f);
    }


    void Update()
    {
        /*
        if (!photonView.IsMine)
            return;
            */


        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");



        if( Mathf.Abs(moveX) > 0.75f || Mathf.Abs(moveY) > 0.75f)
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


        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (Act(STATE.ATTACK))
            {
            }

        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Act(STATE.ATTACK))
            {
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (Act(STATE.JUMP))
            {
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (Act(STATE.ATTACK))
            {
                StartCoroutine(SkillCooldownTime(0));
                    
            }
        }

    }




    public bool Act(STATE nextState)
    {
        if (state == STATE.DIE || state == STATE.ATTACKED
                || state == STATE.JUMP || state == STATE.ATTACK)
            return false;
        else if (state == nextState)
            return true;

        Debug.Log(nextState.ToString());
        animator.SetTrigger(nextState.ToString());
        state = nextState;



        switch (nextState)
        {
            case STATE.ATTACK:
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


    IEnumerator AnimationState()
    {

        yield return new WaitForSeconds(1f);
        float totalTime = animator.GetCurrentAnimatorStateInfo(0).length;
        Debug.Log(totalTime);
        yield return new WaitForSeconds(totalTime - 0.75f);
        state = STATE.IDLE;
        animator.SetTrigger("IDLE");
    }

    IEnumerator SkillCooldownTime(int i_skill)
    {
        skill[i_skill].fillAmount = 0;
        while (true)
        {
            if (skill[i_skill].fillAmount < 1)
                skill[i_skill].fillAmount += Time.deltaTime/3;
            else
            {
                break;
            }
            yield return null;
        }
        yield return null;
    }
}
