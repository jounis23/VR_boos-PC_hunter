using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorceresHit : MonoBehaviour
{
    private Player player;
    private Entity.STATE state;
    private Entity.Status status;
    private ClassSorceres.DetailStatus detailStatus;

    private void Awake()
    {
        player = this.transform.root.GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        state = player.state;
        status = player.status;
        detailStatus = player.GetComponent<ClassSorceres>().detailStatus;
        if (other.CompareTag("Boss"))
        {
            Debug.Log(state);
            Entity enemy = other.transform.root.GetComponent<Entity>();
            switch (state)
            {
                case Entity.STATE.ATTACK:
                    enemy.Attacked(status.atk);
                    break;
                case Entity.STATE.SKILL1:
                    enemy.RecoveryHp(detailStatus.skill1_healHp);
                    break;
                case Entity.STATE.SKILL2:
                    StartCoroutine(RecoveryBuff(enemy, detailStatus.skill2_healCount));
                    break;
                case Entity.STATE.SKILL3:
                    enemy.Attacked(detailStatus.skill3_damage);
                    enemy.SpeedDown(detailStatus.skill3_slow);
                    break;
                case Entity.STATE.SKILL4:
                    enemy.Attacked(detailStatus.skill4_damage);
                    break;
            }
            StartCoroutine(hit(3f));

        }
    }

    IEnumerator RecoveryBuff(Entity other, float count)
    {
        for(int i=0; i<count; i++) 
        { 
            other.RecoveryHp(detailStatus.skill2_healHp);
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
    IEnumerator hit(float t = 1f)
    {
        this.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(t);
        this.GetComponent<Collider>().enabled = true;
    }
}
