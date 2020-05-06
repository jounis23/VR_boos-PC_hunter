using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassKnight : ClassEntity
{
    public struct DetailStatus
    {
        public float skill1_damge;
        public float skill1_spendMp;
        public float skill2_defBuff;
        public float skill2_spendMp;
        public float skill3_spendMp;
        public float skill4_damge;
        public float skill4_spendMp;
    }
    public DetailStatus detailStatus;

    public override void Attack(Entity entity)
    {

    }


    public override void Skill1(Entity entity)
    {
        entity.status.mp -= detailStatus.skill1_spendMp;
        StartCoroutine(SkillEffectManage(skillEffect[0], skillTransfrom[0]));
    }

    public override void Skill2(Entity entity)
    {
        //주변 아군 방어력 증가
        entity.status.mp -= detailStatus.skill2_spendMp;
        StartCoroutine(SkillEffectManage(skillEffect[1], skillTransfrom[1]));
    }

    public override void Skill3(Entity entity)
    {
        //보스 시야 제한
        entity.status.mp -= detailStatus.skill3_spendMp;
        StartCoroutine(SkillEffectManage(skillEffect[2], skillTransfrom[2]));
    }

    public override void Skill4(Entity entity)
    {
        entity.status.mp -= detailStatus.skill4_spendMp;
    }
    public void Skill1Active()
    {
        Debug.Log("Effect");
        //StartCoroutine(SkillEffectManage(skillEffect[0], skillTransfrom[0]));
    }
    public void Skill4Active()
    {
        StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3]));
    }
}
//스킬이 나가고 히트 처리 될 경우 대미지 계산 및 이펙트 효과