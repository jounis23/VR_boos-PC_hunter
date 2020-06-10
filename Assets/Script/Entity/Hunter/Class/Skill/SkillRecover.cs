using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillRecover : MonoBehaviour
{
    public bool isAttackRecover = false;
    public bool isRangeRecover = false;
    public float recoverHp;
    public float recoverHpPer;
    public float recoverMp;
    public float recoverMpPer;

    private float recoverTime;
    public float recoverTimeSet;

    public List<Entity> recoverTarget;

    private void Start()
    {
        recoverTarget = new List<Entity>();
    }

    private void Update()
    {
        if (recoverTarget.Count > 0)
        {
            recoverTime += Time.deltaTime;

            if(recoverTime > recoverTimeSet)
            {
                recoverTime = 0;

                foreach(Entity e in recoverTarget)
                {

                    float hpRecover = recoverHp + (recoverHpPer/100 * e.status.hpMax);
                    float mpRecover = recoverMp + (recoverMpPer/100 * e.status.mpMax);
                    e.UpdateHp(hpRecover);
                    e.UpdateMp(mpRecover);
                }

            }
        }
    }

}
