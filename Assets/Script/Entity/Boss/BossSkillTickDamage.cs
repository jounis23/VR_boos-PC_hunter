using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillTickDamage : BossSkillDamage
{
    public float MAXTickTime = 0.2f;
    private float tickTime = 0;


    void Start()
    {
        
    }

    void Update()
    {
        if(tickTime > 0.0f)
        {
            tickTime -= Time.deltaTime;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if (!photonView.IsMine)
            {
                return;
            }
            Hunter temp = other.transform.root.GetComponent<Hunter>();
            if (temp != null && tickTime <= 0.0f)
            {
                float dmg = damage * boss.status.atk;
                temp.Attacked(dmg);
                tickTime = MAXTickTime;
            }
        }
    }
}
