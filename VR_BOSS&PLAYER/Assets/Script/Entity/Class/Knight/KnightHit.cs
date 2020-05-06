using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightHit : MonoBehaviour
{
    private Entity.STATE state;
    private Entity.Status status;
    private ClassKnight.DetailStatus detailStatus;

    // Start is called before the first frame update
    void Start()
    {
        state = this.transform.root.GetComponent<Player>().state;
        status = this.transform.root.GetComponent<Player>().status;
        detailStatus = this.transform.root.GetComponent<ClassKnight>().detailStatus;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity.Status enemy = other.transform.root.GetComponent<Player>().status;
        switch (state)
        {
            case Entity.STATE.ATTACK:
                enemy.hp -= status.atk - enemy.def;
                StartCoroutine(hit(1f));
                break;
            case Entity.STATE.SKILL1:
                enemy.hp -= detailStatus.skill1_damge - enemy.def;
                StartCoroutine(hit(1f));
                break;
            case Entity.STATE.SKILL4:
                enemy.hp -= detailStatus.skill4_damge - enemy.def;
                this.transform.root.GetComponent<ClassKnight>().Skill4Active();
                break;

        }
    }

    IEnumerator hit(float t)
    {
        this.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(t);
        this.GetComponent<Collider>().enabled = true;
    }

}
