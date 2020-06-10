using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillAttack : MonoBehaviour
{

    public float skillDamage = 0;
    public bool isTrueDamage = false;




    public void SetTureDamage(bool active)
    {
        isTrueDamage = active;
    }
}
