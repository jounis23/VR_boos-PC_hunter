using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class Entity : MonoBehaviourPun
{
    private string nameEntity;
    private float hp;
    private float hpMax;
    private float atk;
    private float def;
    private float speed;

    protected Animator animator;

    public enum STATE
    {
        NONE,
        IDLE,
        WARK,
        RUN,
        JUMP,
        ATTACK,
        ATTACKED,
        DIE
    }

    public STATE state;

    public Text textState;

    public void SetStatus(string name, float hp, float atk, float def, float speed)
    {
        this.nameEntity = name;
        this.hp = hp;
        this.hpMax = this.hp;
        this.atk = atk;
        this.def = def;
        this.speed = speed;
        state = STATE.IDLE;


        animator = GetComponent<Animator>();
    }



    protected void Move(float x, float z)
    {
        this.transform.position += new Vector3(x * speed, 0, z * speed);
    }



    public void Attack()
    {
    }


    public void Skill()
    {
    }


    public void Recovery(float amount)
    {
        hp += amount;

        if (hpMax < hp)
            hp = hpMax;
    }
     
    public void Attacked(float damage)
    {
        hp -= (damage * (1-def));

        if (hp <= 0)
        {
            hp = 0;
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
        textState.text = state.ToString();

    }



}
