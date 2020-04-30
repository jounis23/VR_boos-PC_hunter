using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSorceres : ClassEntity
{
    public GameObject mark;
    public Transform markTransfrom;


    public override void Attack(Entity entity)
    {

    }


    public override void Skill1(Entity entity)
    {
        //casting
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
        //casting
        entity.animator.SetTrigger("SKILL4");
        StartCoroutine(SkillEffectManage(skillEffect[3], skillTransfrom[3]));
    }

    public override void CASTING(Entity entity, int num)
    {
        StartCoroutine(CastingState(entity, num));
    }

    IEnumerator CastingState(Entity entity, int num)
    {
        markTransfrom.position = new Vector3(0, 0, 0);
        GameObject marker = Instantiate(mark, markTransfrom);
        float moveX;
        float moveY;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
            Vector3 Position = marker.transform.position;
            Position.x += moveX * Time.deltaTime * 3;
            Position.z += moveY * Time.deltaTime * 3;
            marker.transform.position = Position;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        markTransfrom.position = marker.transform.position;
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