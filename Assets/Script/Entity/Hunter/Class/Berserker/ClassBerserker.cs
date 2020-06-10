using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassBerserker : ClassEntity
{


    public override void Attack()
    {
        player.soundEntity.AttackSound();
    }


    public override void Skill(int number)
    {

        // 스킬의 쿨타임 적용
        SetSkillCoolTime(number);


        // hp감소(mp대신 hp감소)
        player.UpdateHp(-skill[number].spendMp);

        // 스킬 이펙트 실행 시작 딜레이
        float delay=0;


        // 스킬 번호 0,1,2,3번
        if (number == 3)        //4번은 따로 처리 (BerserkerHit.cs)
            return;    
        else if (number == 0)   //1번의 딜레이 설정
            delay = 0.3f;


        player.soundEntity.ClipSound(number);

        // 스킬 번호에 따른 이펙트를 실행
        StartCoroutine(SkillEffect(number, delay));




           
    }

}

