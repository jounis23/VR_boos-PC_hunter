using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSorceres : ClassEntity
{
    public GameObject mark;
    public Transform castingTransform;

    public Transform hitTransform;
    [System.Serializable]
    public struct DetailStatus
    {
        public float skill1_healHp;
        public float skill1_spendMp;

        public float skill2_healHp;
        public float skill2_healCount;
        public float skill2_spendMp;

        public float skill3_damage;
        public float skill3_slow;
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
        entity.GetComponent<Player>().Act(Entity.STATE.SKILL3);
        StartCoroutine(SkillEffectManage(skillEffect[2], skillTransfrom[2], false));
    }

    public override void Skill4(Entity entity)
    {
        entity.GetComponent<Player>().Act(Entity.STATE.SKILL4);
        StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3], false));
    }

    public override void CASTING(Entity entity, int num)
    {
        float spendMp = 0;
        switch (num)
        {
            case 0:
                spendMp = detailStatus.skill1_spendMp;
                break;
            case 2:
                spendMp = detailStatus.skill3_spendMp;
                break;
            case 3:
                spendMp = detailStatus.skill4_spendMp;
                break;
        }
        if (!entity.SpendMp(spendMp))
            return;
        StartCoroutine(CastingState(entity, num));
    }

    IEnumerator CastingState(Entity entity, int num)
    {
        castingTransform.position = entity.player.transform.position;
        castingTransform.rotation = entity.player.transform.rotation;
        GameObject marker = Instantiate(mark, castingTransform);
        float moveX;
        float moveY;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
            marker.transform.Translate(new Vector3(moveX, 0, moveY) * 3 * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        castingTransform.position = marker.transform.position;
        Destroy(marker);
        entity.state = Entity.STATE.IDLE;
        switch (num)
        {
            case 0:
                Skill1(entity);
                break;
            case 1:
                Skill2(entity);
                break;
            case 2:
                Skill3(entity);
                break;
            case 3:
                Skill4(entity);
                break;
        }
    }

}