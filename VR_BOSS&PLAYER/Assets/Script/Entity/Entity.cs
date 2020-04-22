using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Entity : MonoBehaviourPun
{
    [System.Serializable]
    public struct Status
    {
        public string nameEntity;
        public float hp;
        public float hpMax;
        public float atk;
        public float def;
        public float speed;
    };

    public Status status;

    public Animator animator;

    public enum STATE
    {
        NONE,
        IDLE,
        WARK,
        RUN,
        JUMP,
        ATTACK,
        SKILL1,
        SKILL2,
        SKILL3,
        SKILL4,
        ATTACKED,
        DIE
    }

    public STATE state;

    public Text textState;

    public void SetStatus(float hp, float atk, float def, float speed)
    {
        status.hp = hp;
        status.hpMax = hp;
        status.atk = atk;
        status.def = def;
        status.speed = speed;
        state = STATE.IDLE;

    }



    protected void Move(float x, float z)
    {
        this.transform.position += new Vector3(x * status.speed * 0.1f , 0, z * status.speed * 0.1f);
    }



    public void Attack()
    {
    }


    public void Skill()
    {
    }


    public void Recovery(float amount)
    {
        status.hp += amount;

        if (status.hpMax < status.hp)
            status.hp = status.hpMax;
    }

    public void Attacked(float damage)
    {
        status.hp -= (damage * (1 - status.def));

        if (status.hp <= 0)
        {
            status.hp = 0;
            Die();
        }
    }

    private void Die()
    {
        //animator.SetTrigger()
        state = STATE.DIE;
    }

    public void Jump()
    {
    }



    void Start()
    {

    }


    private void LateUpdate()
    {
        /*
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            state = STATE.IDLE;
        }
        */
        //textState.text = state.ToString();

    }



}
