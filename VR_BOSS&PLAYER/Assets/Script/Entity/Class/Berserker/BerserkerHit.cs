using System.Collections;
using UnityEngine;

public class BerserkerHit : MonoBehaviour
{
    private Entity.STATE state;
    private Entity.Status status;
    private ClassBerserker.DetailStatus detailStatus;

    // Start is called before the first frame update
    void Start()
    {
        state = this.transform.root.GetComponent<Player>().state;
        status = this.transform.root.GetComponent<Player>().status;
        detailStatus = this.transform.root.GetComponent<ClassBerserker>().detailStatus;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Boss"))
        {
            Entity.Status enemy = other.transform.root.GetComponent<Player>().status;
            switch (state)
            {
                case Entity.STATE.ATTACK:
                    if(detailStatus.skill1_staticDamageBuff)
                        enemy.hp -= status.atk;
                    else
                        enemy.hp -= status.atk - enemy.def;
                    StartCoroutine(hit(1f));
                    break;

                case Entity.STATE.SKILL1:
                    break;

                case Entity.STATE.SKILL2:
                    if (detailStatus.skill1_staticDamageBuff)
                        enemy.hp -= status.atk;
                    else
                        enemy.hp -= detailStatus.skill2_damage - enemy.def;
                    status.hp += detailStatus.skill2_healHp;
                    break;

                case Entity.STATE.SKILL3:
                    if (detailStatus.skill1_staticDamageBuff)
                        enemy.hp -= status.atk;
                    else
                        enemy.hp -= detailStatus.skill3_damage - enemy.def;
                    status.hp += detailStatus.skill3_healHp;
                    StartCoroutine(hit(0.5f));
                    break;

                case Entity.STATE.SKILL4:
                    if (detailStatus.skill1_staticDamageBuff)
                        enemy.hp -= status.atk;
                    else
                        enemy.hp -= detailStatus.skill4_damage - enemy.def;
                    status.hp += detailStatus.skill4_healHp;
                    this.transform.root.GetComponent<ClassBerserker>().Skill4Active();
                    StartCoroutine(hit(1f));
                    break;
            }

        }
    }
    IEnumerator hit(float t)
    {
        this.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(t);
        this.GetComponent<Collider>().enabled = true;
    }
}
