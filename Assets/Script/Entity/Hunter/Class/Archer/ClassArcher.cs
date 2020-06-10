using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassArcher : ClassEntity
{

    // 기본 공격은 BowShot.cs에서 따로 처리함
    // (화살 타이밍을 조절하기 위해 따로 처리)
    public override void Attack()
    {
        player.soundEntity.AttackSound(0.8f);
    }



    // 스킬 공격
    // 마찬가지로 타이밍 조절을 위해 BowShot.cs에서 따로 처리함
    public override void Skill(int number)
    {

        // 스킬의 쿨타임 적용
        SetSkillCoolTime(number);

        // 각 스킬에 대한 mp소모 실행
        // 각 스킬에 대한 이펙트, 공격은 state에 따라 BowShot.cs에서 실행되게 함
        player.UpdateMp(-skill[number].spendMp);


        // 스킬 4번은 회피기로 여기서 처리
        // 그 외 스킬은 BowShot.cs에서 처리하게 됨
        if (number == 3)
            Buff(3);
        else
           player.soundEntity.AttackSound(0.8f);

    }
}
