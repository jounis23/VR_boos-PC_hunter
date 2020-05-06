using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBerserker : ClassEntity
{
    [System.Serializable]
    public struct DetailStatus
    {
        public float skill1_spendDamageDebuff;
        public float skill1_healBuff;
        public bool skill1_staticDamageBuff;

        public float skill2_damage;
        public float skill2_spendHp;
        public float skill2_healHp;

        public float skill3_damage;
        public float skill3_spendHp;
        public float skill3_healHp;

        public float skill4_damage;
        public float skill4_spendHp;
        public float skill4_healHp;
    }
    public DetailStatus detailStatus;

    public override void Attack(Entity entity)
    {

    }


    public override void Skill1(Entity entity)
    {
        StartCoroutine(SkillEffectManage(skillEffect[0], skillTransfrom[0]));
    }

    public override void Skill2(Entity entity)
    {
        entity.status.hp -= detailStatus.skill2_spendHp;
        StartCoroutine(SkillEffectManage(skillEffect[1], skillTransfrom[1]));
    }

    public override void Skill3(Entity entity)
    {
        entity.status.hp -= detailStatus.skill3_spendHp;
        StartCoroutine(SkillEffectManage(skillEffect[2], skillTransfrom[2]));
    }

    public override void Skill4(Entity entity)
    {
        entity.status.hp -= detailStatus.skill4_spendHp;
    }
    public void Skill4Active()
    {
        StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3], false));
    }

    IEnumerator Skill1Active()
    {
        detailStatus.skill1_staticDamageBuff = true;
        detailStatus.skill2_healHp += detailStatus.skill1_healBuff;
        detailStatus.skill3_healHp += detailStatus.skill1_healBuff; 
        detailStatus.skill4_healHp += detailStatus.skill1_healBuff;
        yield return new WaitForSeconds(20f);
        detailStatus.skill1_staticDamageBuff = false;
        detailStatus.skill2_healHp -= detailStatus.skill1_healBuff;
        detailStatus.skill3_healHp -= detailStatus.skill1_healBuff;
        detailStatus.skill4_healHp -= detailStatus.skill1_healBuff;
    }
}

