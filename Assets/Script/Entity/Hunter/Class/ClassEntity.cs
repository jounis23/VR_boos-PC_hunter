using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


// 각 직업 이름에 대한 구분
public enum ClassName
{
    Archer,
    Berserker,
    Knight,
    Sorceress
}


public class ClassEntity : MonoBehaviourPun
{
    // 각 스킬에 대한 정보
    [System.Serializable]
    public struct SkillInfo
    {
        public STATE state;      // 상태 : 스킬1,2,3,4, 캐스팅
        public float damage;            // 공격력(%)
        public bool trueDamage;         // 방어무시 공격
        public float coolTime;          // 쿨타임
        public float spendMp;           // mp소모량 (버서커일 경우 hp가 소모)

        public float buffDuration;      // 버프 : 지속 시간
        public float changeAtk;         // 버프 : 공격력 변화
        public float changeDef;         // 버프 : 방어 변화
        public float changeSpeed;       // 버프 : 속도 변화
    };


    // 플레이어의 스테이터스를 저장
    public Hunter player;

    [System.Serializable]
    public struct AuraArray
    {
        public GameObject[] aura;
    }


    // 직업 이름
    public ClassName className;

    // 각 스킬에 대한 정보
    //public SkillInfo[] skillInfo;

    // 스킬 이펙트, 위치
    public SkillEntity[] skill;

    public bool isTrueDamage = false;


    // 각 스킬의 쿨타임
    public float[] skillCoolTime;

    // 각 스킬의 현재 남은 쿨타임
    public float[] skillresetTime;



    // 아우라 이펙트
    public GameObject skillAura;

    // 각 스킬 이펙트는 특정 위치에 존재
    // 손이나 무기 하위에 위치해 있고 SetActive(true / false)로 이펙트 실행
    public GameObject hand;
    public GameObject weapon;


    private bool isNetwork;

    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 초기화
    private void Awake()
    {
        isNetwork = player.isNetwork;

        // 스킬의 쿨타임 설정
        skillCoolTime = new float[skill.Length];
        skillresetTime = new float[skill.Length];

        for(int i = 0; i< skill.Length; i++)
        {
            skillCoolTime[i] = skill[i].coolTime;
            skillresetTime[i] = 0;
        }

    }



    private void Update()
    {

        for (int i = 0; i < skillresetTime.Length; i++)
        {
            if (skillresetTime[i] > 0)
                skillresetTime[i] -= Time.deltaTime;
            else if (skillresetTime[i] == 0)
                continue;
            else if (skillresetTime[i] < 0)
                skillresetTime[i] = 0;

        }
    }


    public bool EnableSkill(int skillNumber)
    {
        bool result = false;
        if(className == ClassName.Berserker)
        {
            if (skillresetTime[skillNumber] <= 0 && player.status.hp >= skill[skillNumber].spendMp)
                result = true;
            else
                result = false;
        }
        else
        {
            if (skillresetTime[skillNumber] <= 0 && player.status.mp >= skill[skillNumber].spendMp)
                result = true;
            else
                result = false;
        }


        if(player.state == STATE.IDLE || player.state == STATE.RUN)
            result = result && true;
        else
            result = result && false;

        return result;
    }

    public void SetSkillCoolTime(int skillNumber)
    {
        skillresetTime[skillNumber] = skillCoolTime[skillNumber];

    }


    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 기본 공격
    public virtual void Attack()
    {

    }

    // 스킬 공격
    public virtual void Skill(int number)
    {

    }

    // 스킬 버프
    // 스테이터스 증가
    public void Buff(int skillNumber)
    {
        StartCoroutine(SkillBuff(skillNumber));

        if(skillAura!=null)
            SetActiveAura(skillNumber);
    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 스킬 이펙트 생성
    public IEnumerator SkillEffect(int number, float delay = 0)
    {
        // 이펙트 생성 초기 딜레이
        // 케릭터 애니메이션 타이밍에 맞추기 위해 사용
        yield return new WaitForSeconds(delay);


        // 스킬 이펙트 SetActive(true)
        // 포톤 사용 여부에 따른 동기화 실행
        if (isNetwork)
            photonView.RPC("SkillEffectSetActiveRPC", RpcTarget.All, number, true);
        else
            SkillEffectSetActiveRPC(number, true);



        // 모든 이펙트는 5초 후 SetActive(false)
        yield return new WaitForSeconds(skill[number].effectDuration);



        // 스킬 이펙트 SetActive(false)
        if (isNetwork)
            photonView.RPC("SkillEffectSetActiveRPC", RpcTarget.All, number, false);
        else
            SkillEffectSetActiveRPC(number, false);
    }


    // 스킬 이펙트를 보여줌
    [PunRPC]
    public void SkillEffectSetActiveRPC(int number, bool active)
    {

        if (active)
        {
            //skill[number].gameObject.SetActive(true);
            skill[number].Initialize();
        }
        else
        {
            skill[number].Reset();
            //skill[number].gameObject.SetActive(false);

        }
    }





    // 스킬 버프
    // 헌터의 특정 스킬 사용시 일정 시간 스테이터스 변화
    public IEnumerator SkillBuff(int skillNumber)
    {
        SkillBuff buffSkill = skill[skillNumber].skillBuff;


        if (buffSkill.buffDuration != 0)
        {
            // 스테이터스 증가
            // 공격력, 스피드, 방어력은 동기화 시킴
            
            player.UpdateAtk(buffSkill.changeAtk);
            player.UpdateSpeed(buffSkill.changeSpeed);
            player.UpdateDef(buffSkill.changeDef);

            // 방무뎀은 자신만 설정(동기화x)
            // 방무뎀은 공격시인데 공격자체는 동기화 없이 자신만 코드를 실행 하도록 함
            bool[] initTrueDamage = new bool[4];

            if (buffSkill.isTrueDamage)
            {
                for (int i = 0; i < skill.Length; i++)
                {
                    if (skill[i].skillAttack == null)
                        continue;
                    initTrueDamage[i] = skill[i].skillAttack.isTrueDamage;
                    skill[i].skillAttack.isTrueDamage = true;
                }
            }


            // 지속시간 만큼 기다림
            yield return new WaitForSeconds(buffSkill.buffDuration);



            // 원래 스테이터스로 되돌림
            if (buffSkill.isTrueDamage)
            {
                for (int i = 0; i < skill.Length; i++)
                {
                    if (skill[i].skillAttack == null)
                        continue;
                    skill[i].skillAttack.isTrueDamage = initTrueDamage[i];
                }
            }

            player.UpdateAtk(-buffSkill.changeAtk);
            player.UpdateSpeed(-buffSkill.changeSpeed);
            player.UpdateDef(-buffSkill.changeDef);
        }



    }






    // 아우라 이펙트
    public void SetActiveAura(int skillNumber)
    {
        // 스킬 이펙트 SetActive(false)
        if (isNetwork)
            photonView.RPC("SetActiveAuraRPC", RpcTarget.All, skillNumber);
        else
            SetActiveAuraRPC(skillNumber);
    }
    [PunRPC]
    public void SetActiveAuraRPC(int skillNumber)
    {
        StartCoroutine(AuraControl(skillNumber));
    }

    IEnumerator AuraControl(int skillNumber)
    {
        if (skill[skillNumber].skillBuff != null && skillAura != null)
        {

            skillAura.SetActive(true);
            yield return new WaitForSeconds(skill[skillNumber].skillBuff.buffDuration);
            skillAura.SetActive(false);
        }
        else
            yield return null;
    }

}
