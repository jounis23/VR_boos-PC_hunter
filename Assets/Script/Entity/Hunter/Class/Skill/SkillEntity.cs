using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SkillEntity : MonoBehaviourPun
{
    // 
    public Hunter player;
    public ClassEntity classEntity;



    // 이 스킬 이펙트의 기준 위치
    // 스킬이 사라지면 자동으로 parent로 돌아오며 SetActive(false) 됨
    public Transform parent;

    // 스킬을 쓸때 이펙트가 parent에 붙어 있을지의 여부
    public bool isParent = false;
    public bool isCasting = false;

    private bool isPlay = false;


    // 이 스킬을 사용하는 직업
    public ClassName className;

    // 이 스킬의 번호 (1~4번, index : 0~3)
    public int skillNumber;
    public GameObject effect;


    public SkillAttack skillAttack;
    public SkillBuff skillBuff;
    public SkillRecover skillRecover;
    public SkillDebuff skillDebuff;


    // 스킬 정보
    public STATE state;      // 상태 : 스킬1,2,3,4, 캐스팅
    public float coolTime = 5;
    public float spendMp;           // mp소모량 (버서커일 경우 hp가 소모)



    // 이펙트 시간
    // 일정 시간 이후 SetActive(false)됨
    public float effectDuration = 5;
    private float durationTime = 0;





    //#############################################################################
    //#############################################################################
    //#############################################################################


    private void Awake()
    {
        parent = this.transform.parent;


        skillAttack = GetComponent<SkillAttack>();
        skillBuff = GetComponent<SkillBuff>();
        skillRecover = GetComponent<SkillRecover>();
    }





    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 스킬 초기화
    // 스킬 사용 직전 실행
    public void Initialize()
    {
        // parent의 위치로 옮김
        // 무기or 케릭터의 정면 위치
        this.transform.position = parent.position;



        // parent에서 떼어져 이펙트 실행
        if (!isParent)
        {
            this.transform.parent = null;
            this.transform.rotation = Quaternion.identity;
        }
        // parent의에 붙어서 이펙트 실행
        else
        {
            this.transform.rotation = parent.rotation;
        }



        // 버프형일 경우 버프 실행
        if (skillBuff != null)
        {
            classEntity.Buff(skillNumber);
        }

        effect.SetActive(true);

        isPlay = true;
    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 스킬 리셋
    // 스킬 사용 직후 실행
    public void Reset()
    {
        if (!isParent)
            this.transform.parent = parent;

        this.transform.position = parent.position;
        this.transform.rotation = parent.rotation;


        if (skillRecover != null)
            skillRecover.recoverTarget.Clear();

        effect.SetActive(false);
        
        isPlay = false;
    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 이 스킬에 대한 이펙트 지속
    // 기본적으로 5초
    private void Update()
    {
        if (!isPlay)
            return; 


        durationTime += Time.deltaTime;
        if(durationTime > effectDuration)
        {
            durationTime = 0;

            Reset();
        }



        if(isParent)
            if (player.state == STATE.IDLE || player.state == STATE.DIE || player.state == STATE.RUN)
            {
                Reset();
            }

    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 이 스킬이 가지는 공격
    public void OnTriggerEnter(Collider other)
    {
        Debug.Log(this.name + " / " + skillAttack + " . " + other.name);

        /*
        if (!photonView.IsMine)
            return;
            */
        if (other.CompareTag("Boss"))
        {
            // 스킬 공격
            if (skillAttack != null)
                Attack(other);

            // 스킬 디버프
            if (skillDebuff != null)
                Debuff(other);
        }



        // 스킬 회복
        if (skillRecover != null)
        {
            // 타격 조건 없을때 자신을 회복
            if (!skillRecover.isAttackRecover)
                Recover();


            // 일정 시간과, 영역 내의 Hunter의 hp,mp 회복
            // skillRecover에 Hunter의 list를 저장하여 일정 간격마다 회복시킴
            if (skillRecover.isRangeRecover && other.CompareTag("Player"))
            {
                Entity hunter = other.transform.root.GetComponent<Entity>();
                if (hunter != null)
                    skillRecover.recoverTarget.Add(hunter);
            }
        }

    }


    //#############################################################################


    private void OnTriggerExit(Collider other)
    {
        if (skillRecover != null && skillRecover.isRangeRecover && other.CompareTag("Player"))
        {
            Hunter hunter = other.GetComponent<Hunter>();
            if (hunter != null && skillRecover.recoverTarget.Contains(hunter))
                skillRecover.recoverTarget.Remove(hunter);
        }


    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 적(Boss)를 공격
    private void Attack(Collider other)
    {

        //Debug.Log("[SKILL" + (skillNumber + 1) + "] : " + this.name + " ->" + other.name + " / " + atk);

        Entity enemy = other.transform.root.GetComponentInChildren<Entity>();
        if (enemy == null)
            return;


        // 스킬 총 데미지 : 기본 공격력 * 스킬 데미지
        float atk = player.status.atk;
        atk *= (skillAttack.skillDamage / 100);


        // 방어무시 공격(방무뎀)
        bool isTrueDamage = skillAttack.isTrueDamage;


        // 크리티컬
        // 확률 : 스피트*2
        // 비율 : 125%
        int critical = Random.Range(0, 100);
        if (critical <= (player.status.speed * 2))
            atk *= 1.25f;

        

        // 적 공격 (스킬 총 데미지, 방무뎀, 직업이름)
        enemy.Attacked(atk, isTrueDamage, className.ToString());


        // 스킬 회복
        // (타격 성공 조건일때)
        if (skillRecover != null && skillRecover.isAttackRecover)
            Recover();
    }



    //#############################################################################
    //#############################################################################
    //#############################################################################


    // 자신의 hp,mp회복
    private void Recover()
    {
        player.UpdateHp(skillRecover.recoverHp, true);
        player.UpdateHp(skillRecover.recoverMp, true);
    }



    //#############################################################################
    //#############################################################################
    //#############################################################################


    // 적(Boss)를 디버프
    private void Debuff(Collider other)
    {
        Entity enemy = other.transform.root.GetComponentInChildren<Entity>();
        if (enemy == null)
            return;

        StartCoroutine(DebuffSkill(enemy));
    }

    IEnumerator DebuffSkill(Entity enemy)
    {
        // 적의 스테이터스 감소
        enemy.UpdateAtk(-skillDebuff.atkDown);
        enemy.UpdateDef(-skillDebuff.defDown);
        enemy.UpdateSpeed(-skillDebuff.speedDown);

        // 유지시간 동안 대기
        yield return new WaitForSeconds(skillDebuff.downDuration);

        // 적의 스테이터스 복귀
        enemy.UpdateAtk(skillDebuff.atkDown);
        enemy.UpdateDef(skillDebuff.defDown);
        enemy.UpdateSpeed(skillDebuff.speedDown);
    }

}
