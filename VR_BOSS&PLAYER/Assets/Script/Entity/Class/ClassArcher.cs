using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassArcher : ClassEntity
{

    public override void Attack(Entity entity)
    {

        GameObject skill = Instantiate(attackEffect, skillTransfrom[0]);
        Destroy(skill, 3);
    }


    public override void Skill1(Entity entity)
    {
        StartCoroutine(SkillEffectManage(skillEffect[0], skillTransfrom[0]));
    }

    public override void Skill2(Entity entity)
    {
        StartCoroutine(SkillEffectManage(skillEffect[1], skillTransfrom[1]));
    }

    public override void Skill3(Entity entity)
    {
        StartCoroutine(SkillEffectManage(skillEffect[2], skillTransfrom[2]));
    }

    public override void Skill4(Entity entity)
    {
        StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3]));
    }
}
