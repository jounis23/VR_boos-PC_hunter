using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassEntity : MonoBehaviour
{
    public ClassName className;
    public GameObject[] skillEffect;
    public Transform[] skillTransfrom;

    public enum ClassName
    {
        Archer,
        Berserker,
        Knight,
        Sorceress
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
    public IEnumerator SkillEffectManage(GameObject skillEffect, Transform skillTransfrom)
    {
        GameObject skill = Instantiate(skillEffect, skillTransfrom);
        yield return new WaitForSeconds(10.0f);
        Destroy(skill);

    }

}
