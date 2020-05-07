using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSorceres : ClassEntity
{
    public Casting casting;
    public GameObject mark;

    public Transform castingTransform;
    private Transform skillTransform;

    public struct DetailStatus
    {
        public float skill1_healHp;
        public float skill1_spendMp;

        public float skill2_healHp;
        public float skill2_spendMp;

        public float skill3_damage;
        public float skill3_spendMp;

        public float skill4_damage;
        public float skill4_spendMp;
    }
    public DetailStatus detailStatus;
    public override void Attack(Entity entity)
    {
        StartCoroutine(SkillEffectManage(attackEffect, attackTransfrom, false));
    }


    public override void Skill1(Entity entity)
    {
        //casting
        if (!entity.SpendMp(detailStatus.skill1_spendMp))
            return;
        
        entity.animator.SetTrigger("SKILL1");
        StartCoroutine(SkillEffectManage(skillEffect[0], skillTransfrom[0], false));
    }

    public override void Skill2(Entity entity)
    {
        if (!entity.SpendMp(detailStatus.skill2_spendMp))
            return;
        StartCoroutine(SkillEffectManage(skillEffect[1], skillTransfrom[1], false));
    }

    public override void Skill3(Entity entity)
    {
        if (!entity.SpendMp(detailStatus.skill3_spendMp))
            return;
        entity.animator.SetTrigger("SKILL3");
        StartCoroutine(SkillEffectManage(skillEffect[2], skillTransfrom[2], false));
    }

    public override void Skill4(Entity entity)
    {
        if (!entity.SpendMp(detailStatus.skill4_spendMp))
            return;
        //casting
        entity.animator.SetTrigger("SKILL4");
        //StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3], false));
        CastingManage(casting, castingTransform ,skillTransform, detailStatus.skill4_damage);
    }

    public override void CASTING(Entity entity, int num)
    {
        CastingManage(casting, castingTransform, skillTransform, detailStatus.skill4_damage);
        //StartCoroutine(CastingState(entity, num));
    }

    //IEnumerator CastingState(Entity entity, int num)
    //{
    //    markTransfrom.position = entity.player.transform.position;
    //    markTransfrom.rotation = entity.player.transform.rotation;
    //    GameObject marker = Instantiate(mark, markTransfrom);
    //    float moveX;
    //    float moveY;
    //    while (true)
    //    {
    //        if (Input.GetKeyDown(KeyCode.Space))
    //            break;

    //        moveX = Input.GetAxis("Horizontal");
    //        moveY = Input.GetAxis("Vertical");
    //        marker.transform.Translate(new Vector3(moveX, 0, moveY) * 3 * Time.deltaTime);
    //        yield return null;
    //    }
    //    yield return new WaitForSeconds(0.5f);
    //    markTransfrom.position = marker.transform.position;
    //    Destroy(marker);
    //    entity.state = Entity.STATE.IDLE;
    //    switch (num)
    //    {
    //        case 0:
    //            Skill1(entity);
    //            break;
    //        case 1:
    //            Skill2(entity);
    //            break;
    //        case 2:
    //            Skill3(entity);
    //            break;
    //        case 3:
    //            Skill4(entity);
    //            break;
    //    }
    //}

    void CastingManage(Casting castingEffect, Transform castingTransform,  Transform skillTransform, float atk)
    {
        Casting casting = Instantiate(castingEffect, castingTransform);
        casting.Init(skillTransform, atk);
        //Destroy(casting, 5);
    }

}