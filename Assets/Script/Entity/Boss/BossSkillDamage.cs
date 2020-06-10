using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class BossSkillDamage : MonoBehaviourPun
{
    public float damage;
    public Boss boss;

    void Start()
    {
        
    }


    void Update()
    {
        
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
            float dmg = damage * boss.status.atk;
            if (temp != null)
                temp.Attacked(dmg);
        }
    }
}
