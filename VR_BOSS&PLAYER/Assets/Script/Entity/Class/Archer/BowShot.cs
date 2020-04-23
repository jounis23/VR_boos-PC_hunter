using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShot : MonoBehaviour
{
    public Player player;
    public Arrow shotEffect;
    public Transform shotTransform;
    private bool enable = true;

    private void Update()
    {
        if(player.state == Entity.STATE.IDLE)
        {

            enable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("ready" + other.tag + " / " + other.name);
        if (!other.CompareTag("Bow") || !enable)
            return;

        enable = false;
        Transform shotSpwan = shotTransform;
        //shotSpwan.rotation = player.transform.rotation;


        Debug.Log("shot");

        float atk=0;
        float size=0;
        List<float> delay = new List<float>();

        switch (player.state)
        {
            case Entity.STATE.ATTACK:
                atk = player.status.atk;
                size = 4;
                delay.Add(0.2f);
                break;
            case Entity.STATE.SKILL1:
                atk = player.status.atk*3;
                size = 6;
                delay.Add(0.2f);
                break;
            case Entity.STATE.SKILL2:
                atk = player.status.atk*1.5f;
                size = 4;
                delay.Add(0.2f);
                delay.Add(0.2f);
                delay.Add(0.2f);
                break;
            case Entity.STATE.SKILL3:
                atk = player.status.atk*7;
                size = 8;
                delay.Add(0.2f);
                break;
            case Entity.STATE.SKILL4:
                break;
        }
        StartCoroutine(SkillEffectManage(shotEffect, shotSpwan, delay, atk, size));
    }



    public IEnumerator SkillEffectManage(Arrow skillEffect, Transform skillTransfrom, List<float> deltaTime, float atk, float size)
    {
        Vector3 dir = player.transform.forward;
        for (int i=0; i<deltaTime.Count ; i++)
        {
            yield return new WaitForSeconds(deltaTime[i]);
            Arrow arrow = Instantiate(skillEffect, skillTransfrom);
            arrow.Init(dir, atk, size);
            Destroy(arrow, 5);
        }

    }
}
