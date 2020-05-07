using System.Collections;
using UnityEngine;

public class BerserkerHit : MonoBehaviour
{
    private Player player;
    private Entity.STATE state;
    private Entity.Status status;
    private ClassBerserker.DetailStatus detailStatus;

   

    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.root.GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        state = player.state;
        status = player.status;
        detailStatus = this.transform.root.GetComponent<ClassBerserker>().detailStatus;

        float atk = 0;
        float heal = 0;
       
        if (other.CompareTag("Boss"))
        {
            Entity.Status enemy = other.transform.root.GetComponent<Player>().status;
            switch (state)
            {
                case Entity.STATE.ATTACK:
                    atk = status.atk;
                    StartCoroutine(hit());
                    break;

                case Entity.STATE.SKILL2:
                    atk = detailStatus.skill2_damage;
                    heal = detailStatus.skill2_healHp;
                    StartCoroutine(hit());
                    break;


                case Entity.STATE.SKILL3:
                    atk = detailStatus.skill3_damage;
                    heal = detailStatus.skill3_healHp;
                    StartCoroutine(skill3Hit());
                    StartCoroutine(hit());
                    break;

                case Entity.STATE.SKILL4:
                    atk = detailStatus.skill4_damage;
                    heal = detailStatus.skill4_healHp;
                    StartCoroutine(hit());
                    break;
            }
            player.Recovery(heal);
            if(detailStatus.skill1_trueDamageBuff)
                other.transform.root.GetComponent<Entity>().Attacked(atk, true);
            else
                other.transform.root.GetComponent<Entity>().Attacked(atk);

        }
    }
    IEnumerator hit(float t=1f)
    {
        this.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(t);
        this.GetComponent<Collider>().enabled = true;
    }

    IEnumerator skill3Hit()
    {
        detailStatus.skill3_damage += 200;
        detailStatus.skill3_healHp += 200;
        yield return new WaitForSeconds(2.0f);
        detailStatus.skill3_damage += 200;
        detailStatus.skill3_healHp -= 200;
    }
}
