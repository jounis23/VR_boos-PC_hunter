using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassKnight : ClassEntity
{



    public override void Attack(Entity entity)
    {
        
    }


    public override void Skill1(Entity entity)
    {

    }

    public override void Skill2(Entity entity)
    {
        //주변 아군 방어력 증가
        StartCoroutine(SkillEffectManage(skillEffect[1], skillTransfrom[1]));
    }

    public override void Skill3(Entity entity)
    {
        //보스 시야 제한
        StartCoroutine(SkillEffectManage(skillEffect[2], skillTransfrom[2]));
    }

    public override void Skill4(Entity entity)
    {
        
    }
    public void Skill1Active()
    {
        Debug.Log("Effect");
        StartCoroutine(SkillEffectManage(skillEffect[0], skillTransfrom[0]));
    }
    public void Skill4Active()
    {
        StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3]));
    }
}
//스킬이 나가고 히트 처리 될 경우 대미지 계산 및 이펙트 효과