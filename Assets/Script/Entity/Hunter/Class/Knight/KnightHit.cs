using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightHit : MonoBehaviour
{
    private Hunter player;
    private STATE state;
    private STATUS status;

    private ClassKnight classEntity;
    private Collider coll;


    void Start()
    {
        player = this.transform.root.GetComponent<Hunter>();
        classEntity = this.transform.root.GetComponent<ClassKnight>();
        coll = this.GetComponent<Collider>();
        coll.enabled = false;
    }

    private void Update()
    {

        if (!coll.enabled)
        {
            if(player.state == STATE.ATTACK || player.state.ToString().Contains("SKILL"))
                coll.enabled = true;
        }
        else
        {
            if (player.state == STATE.IDLE || player.state == STATE.DIE)
                coll.enabled = false;
        }
    }



    private void OnTriggerEnter(Collider other)
    {
        state = player.state;
        switch (state)
        {
            case STATE.ATTACK:
            case STATE.SKILL4:
                break;
            default:
                return;
        }


        Debug.Log("[" + state.ToString() + "] " + player.name + " ->" + other.name + " / " + player.status.atk);

        if (other.CompareTag("Boss"))
        {

            status = player.status;

            Entity enemy = other.transform.root.GetComponent<Entity>();

            if (enemy == null)
                return;

            float atk = status.atk;
            float[] delay;

            switch (state)
            {
                case STATE.ATTACK:
                    StartCoroutine(hit(0.5f));
                    break;
                case STATE.SKILL4:
                    atk *= classEntity.skill[3].skillAttack.skillDamage / 100;
                    delay = new float[3];
                    delay[0] = 0.2f;
                    delay[1] = 0.2f;
                    delay[2] = 0.4f;
                    StartCoroutine(hit(delay));
                    break;

            }

            enemy.Attacked(atk, false);

        }
    }

    IEnumerator hit(float t)
    {
        yield return new WaitForSeconds(t);
        coll.enabled = true;
        yield return new WaitForSeconds(0.5f);
        coll.enabled = false;
    }

    IEnumerator hit(float[] delay)
    {
        for(int i = 0; i<delay.Length; i++)
        {
            yield return new WaitForSeconds(delay[i]);
            coll.enabled = true;
            yield return new WaitForSeconds(0.2f);
            coll.enabled = false;
        }
    }


}
