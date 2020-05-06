using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassEntity : MonoBehaviour
{
    public ClassName className;

    public Entity.STATE[] type = {Entity.STATE.SKILL1, Entity.STATE.SKILL2, Entity.STATE.SKILL3, Entity.STATE.SKILL4};

    public GameObject attackEffect;
    public Transform attackTransfrom;
    public GameObject[] skillEffect;
    public Transform[] skillTransfrom;

    public enum ClassName
    {
        Archer,
        Berserker,
        Knight,
        Sorceress
    }

    public void Awake()
    {
        switch (className)
        {
            case ClassName.Archer:
                break;
            case ClassName.Berserker:
                break;
            case ClassName.Knight:
                break;
            case ClassName.Sorceress:
                type[0] = Entity.STATE.CASTING;
                type[2] = Entity.STATE.CASTING;
                type[3] = Entity.STATE.CASTING;
                break;
        }
    }
    public virtual void Attack(Entity entity)
    {

    }


    public virtual void Skill1(Entity entity)
    {

    }

    public virtual void Skill2(Entity entity)
    {

    }

    public virtual void Skill3(Entity entity)
    {

    }

    public virtual void Skill4(Entity entity)
    {

    }
    public virtual void CASTING(Entity entity, int num)
    {

    }

    public IEnumerator SkillEffectManage(GameObject skillEffect, Transform skillTransfrom, float[] deltaTime)
    {
        foreach(float t in deltaTime)
        {
            GameObject skill = Instantiate(skillEffect, skillTransfrom);
            skill.transform.parent = null;
            Destroy(skill, 5);
            yield return new WaitForSeconds(t);
        }

    }
    public IEnumerator SkillEffectManage(GameObject skillEffect, Transform skillTransfrom, bool isParent=true)
    {
        GameObject skill = Instantiate(skillEffect, skillTransfrom);
        if(!isParent)
            skill.transform.parent = null;
        Destroy(skill, 5);
        yield return new WaitForSeconds(0.1f);

    }

}
