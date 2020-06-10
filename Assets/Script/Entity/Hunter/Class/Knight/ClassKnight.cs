using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassKnight : ClassEntity
{

    public override void Attack()
    {

    }


    public override void Skill(int number)
    {
        // 스킬의 쿨타임 적용
        SetSkillCoolTime(number);


        player.UpdateMp(-skill[number].spendMp);

        // 스킬 이펙트 실행 시작 딜레이
        float delay = 0.3f;

        // 스킬 2번, 3번
        if (number == 3 )
            delay = 0.1f;


        // 스킬 번호에 따른 이펙트를 실행
        StartCoroutine(SkillEffect(number, delay));

        
    }



}
//스킬이 나가고 히트 처리 될 경우 대미지 계산 및 이펙트 효과