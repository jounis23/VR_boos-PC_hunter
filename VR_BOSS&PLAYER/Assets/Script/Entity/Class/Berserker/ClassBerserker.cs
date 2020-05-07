using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBerserker : ClassEntity
{
    [System.Serializable]
    public struct DetailStatus
    {
        public float skill1_healBuff;
        public bool skill1_trueDamageBuff;

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
        StartCoroutine(Skill1Active());
    }

    public override void Skill2(Entity entity)
    {
        entity.Attacked(detailStatus.skill2_spendHp,true);
        StartCoroutine(SkillEffectManage(skillEffect[1], skillTransfrom[1]));
    }

    public override void Skill3(Entity entity)
    {
        entity.Attacked(detailStatus.skill2_spendHp, true);
        StartCoroutine(SkillEffectManage(skillEffect[2], skillTransfrom[2]));
    }

    public override void Skill4(Entity entity)
    {
        entity.Attacked(detailStatus.skill2_spendHp, true);
    }
    public void Skill4Active()
    {
        StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3], false));
    }

    IEnumerator Skill1Active(float time = 20f)
    {
        detailStatus.skill1_trueDamageBuff = true;
        detailStatus.skill2_healHp += detailStatus.skill1_healBuff;
        detailStatus.skill3_healHp += detailStatus.skill1_healBuff; 
        detailStatus.skill4_healHp += detailStatus.skill1_healBuff;
        yield return new WaitForSeconds(time);
        detailStatus.skill1_trueDamageBuff = false;
        detailStatus.skill2_healHp -= detailStatus.skill1_healBuff;
        detailStatus.skill3_healHp -= detailStatus.skill1_healBuff;
        detailStatus.skill4_healHp -= detailStatus.skill1_healBuff;
    }
}

