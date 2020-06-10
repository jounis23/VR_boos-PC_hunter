using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


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
    CASTING,
    ATTACKED,
    DIE
}


[System.Serializable]
public struct STATUS
{
    public float hp;        //현재 체력
    public float mp;        //현재 마력
    public float hpMax;     //최대 체력
    public float mpMax;     //최대 마력
    public float atk;       //공격력
    public float def;       //방어력
    public float speed;     //속도
    public float recovery;  //회복력
};



public class Entity : MonoBehaviourPun
{

    // 플레이어의 스테이터스
    public STATUS status;

    // 자신의 상태
    // IDLE, RUN, ATTACK, 등
    public STATE state;

    // 자신의 카메라
    public GameObject mainCam;

    // 플레이어가 hunter
    public GameObject player;

    // 플레이어의 에니메이터 
    public Animator animator;

    // 피격 이펙트(오라)
    public GameObject attackedAura;

    // 회복 이펙트(오라)
    public GameObject hpHealAura;
    public GameObject mpHealAura;



    // 공격시 나타날 데미지 텍스트
    //public DamageText hubDamageText;
    //public Transform hubPos;


    // 누적 대미지 저장
    [HideInInspector]
    public float[] stackDamage;
    
    public bool isNetwork = true;


    public SoundEntity soundEntity;




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 초기화
    private void Awake()
    {

    }

    void Start()
    {
    }





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
        Debug.Log(this.name);
        player.transform.position += player.transform.forward * x * status.speed * 0.1f * Time.deltaTime;
    }



    public void Attack()
    {
    }


    public void Skill()
    {
    }





    //#############################################################################
    //#############################################################################
    //#############################################################################

    // HP회복
    public void UpdateHp(float amount, bool isEffect = false)
    {
        if (!isNetwork)
            UpdateHpRPC(amount, isEffect);
        else if (photonView.IsMine)
            photonView.RPC("UpdateHpRPC", RpcTarget.All, amount, isEffect);
    }
    [PunRPC]
    public void UpdateHpRPC(float amount, bool isEffect = false)
    {
        //Debug.Log("[Update Hp] " + this.name + " : " + status.hp + "->" + (status.hp + amount) + "("+ amount+")");

        status.hp += amount;

        if (status.hpMax < status.hp)
            status.hp = status.hpMax;
        else if (status.hp < 0)
        {
            status.hp = 0;
            Die();
        }

        if (amount > 1 && isEffect)
            SetActiveAura("HpHeal");

    }



    // MP회복
    public void UpdateMp(float amount, bool isEffect = false)
    {
        if (!isNetwork)
            UpdateMpRPC(amount, isEffect);
        else if (photonView.IsMine)
            photonView.RPC("UpdateMpRPC", RpcTarget.All, amount, isEffect);
    }
    [PunRPC]
    public void UpdateMpRPC(float amount, bool isEffect = false)
    {
        //Debug.Log("[Update Mp] " + this.name + " : " + status.hp + "->" + (status.hp + amount) + "(" + amount + ")");

        status.mp += amount;

        if (status.mpMax < status.mp)
            status.mp = status.mpMax;
        else if (status.mp < 0)
            status.mp = 0;

        if (amount > 1 && isEffect)
            SetActiveAura("MpHeal");


    }






    //#############################################################################

    // 공격 당했을 때 HP감소
    public virtual void Attacked(float damage, bool isTrueDamage = false, string attacker = "None")
    {
        if (state == STATE.DIE)
            return;

        Debug.Log("[Attacked] : " + this.name + " / attacker : " + attacker);

        bool isMiss = false;
        float amountDamage;
        if (isTrueDamage)
        {
            amountDamage = damage;
        }
        else
        {
            amountDamage = damage * (1 - (status.def/100) );
        }

        int miss = Random.Range(0, 100);
        if (miss < status.speed/2)
        {
            isMiss = true;
            amountDamage = 0;
        }

        /*
        // 보스에서 어택 처리 할 경우 사용됌
        switch (attacker)
        {
            case "Archer":
                stackDamage[0] += amountDamage;
                break;
            case "Berserker":
                stackDamage[1] += amountDamage;
                break;
            case "Knight":
                stackDamage[2] += amountDamage;
                break;
            case "Sorceress":
                stackDamage[3] += amountDamage;
                break;
            default:
                Debug.LogWarning("타격자의 이름이 입력되지 않았습니다.");
                break;
        }

        */



        photonView.RPC("AttackedRPC", RpcTarget.All, amountDamage, isTrueDamage, isMiss);
    }
    [PunRPC]
    public void AttackedRPC(float damage, bool isTrueDamage = false, bool isMiss = false)
    {



        Debug.Log("\t\t" + this.name + " Hp : " + status.hp + " -> " + (status.hp - damage) + " / Damage : " + damage + " / TrueDamage : " + isTrueDamage + " / Miss : " + isMiss);

        UpdateHpRPC(-damage);


        soundEntity.AttackedSound();



    }



    //#############################################################################

    // 공격력 증가,감소
    public void UpdateAtk(float amount)
    {
        if (!isNetwork)
            UpdateAtkRPC(amount);
        else if (photonView.IsMine && isNetwork)
            photonView.RPC("UpdateAtkRPC", RpcTarget.All, amount);
    }
    [PunRPC]
    protected void UpdateAtkRPC(float amount)
    {
        Debug.Log("[Update Atk] " + this.name + " : " + amount);

        status.atk += amount;

        if (status.atk <= 0)
        {
            status.atk = 0;
        }

    }



    //#############################################################################

    // 스피드 증가,감소
    public void UpdateSpeed(float amount)
    {
        if (!isNetwork)
            UpdateSpeedRPC(amount);
        else if (photonView.IsMine)
            photonView.RPC("UpdateSpeedRPC", RpcTarget.All, amount);
    }
    [PunRPC]
    protected void UpdateSpeedRPC(float amount)
    {
        Debug.Log("[Update Speed] " + this.name + " : " + amount);

        status.speed += amount;

        if (status.speed <= 0)
        {
            status.speed = 0;
        }

    }



    //#############################################################################

    // 방어력 증가,감소
    public void UpdateDef(float amount)
    {
        if (!isNetwork)
            UpdateDefRPC(amount);
        else if (photonView.IsMine)
            photonView.RPC("UpdateDefRPC", RpcTarget.All, amount);
    }
    [PunRPC]
    protected void UpdateDefRPC(float amount)
    {
        Debug.Log("[Update Def] " + this.name + " : " + amount);

        status.def += amount;

        if (status.def <= 0)
        {
            status.def = 0;
        }else if (status.def > 95f)
        {
            status.def = 95f;
        }

    }



    //#############################################################################

    

    public void SpeedDown(float amount)
    {
        if (photonView.IsMine)
            photonView.RPC("SpeedDownRPC", RpcTarget.All, amount);
    }
    [PunRPC]
    protected void SpeedDownRPC(float amount)
    {

        Debug.Log("[Speed Down] " + this.name + " : " + amount);

        status.speed -= amount;

        if(status.speed <= 0)
        {
            status.speed = 0;
        }
    }



    //#############################################################################

    // 캐릭터 죽음
    public void Die()
    {
        if (state == STATE.DIE)
            return;

        if (photonView.IsMine)
            photonView.RPC("DieRPC", RpcTarget.All);
    }
    [PunRPC]
    protected void DieRPC()
    {
        Debug.Log("[Die] " + this.name);

        //animator.SetTrigger()
        state = STATE.DIE;

        
        if(animator!=null)
           animator.SetTrigger("DIE");
           
    }




    //#############################################################################
    //#############################################################################
    //#############################################################################


    // 아우라 이펙트
    public void SetActiveAura(string type)
    {
        // 스킬 이펙트 SetActive(false)
        if (isNetwork)
            photonView.RPC("SetActiveAuraRPC", RpcTarget.All, type);
        else
            SetActiveAuraRPC(type);
    }
    [PunRPC]
    public void SetActiveAuraRPC(string type)
    {
        StartCoroutine(AuraControl(type));
    }

    IEnumerator AuraControl(string type)
    {
        GameObject aura = null;
        if (type == "Attacked")
            aura = attackedAura;
        else if (type == "HpHeal")
            aura = hpHealAura;
        else if (type == "MpHeal")
            aura = mpHealAura;

        if(aura != null)
            aura.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        if (aura != null)
            aura.SetActive(false);
    }


}
