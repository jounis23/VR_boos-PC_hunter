using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShot : MonoBehaviour
{
    public Arrow shotEffect;
    public Transform shotTransform;
    private bool enable = true;

    private Player player;
    private Entity.STATE state;
    private Entity.Status status;
    private ClassArcher.DetailStatus detailStatus;

    private void Start()
    {
        player = this.transform.root.GetComponent<Player>();
        
    }
    private void Update()
    {
        if(state == Entity.STATE.IDLE)
        {

            enable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        state = player.state;
        status = player.status;
        detailStatus = this.transform.root.GetComponent<ClassArcher>().detailStatus;
        Debug.Log("ready" + other.tag + " / " + other.name);
        if (!other.CompareTag("Bow") )
            return;

        enable = false;
        Transform shotSpwan = shotTransform;
        //shotSpwan.rotation = player.transform.rotation;


        Debug.Log("shot");
        Debug.Log("state: " + state);

        float atk=0;
        float size=0;
        List<float> delay = new List<float>();

        switch (state)
        {
            case Entity.STATE.ATTACK:
                atk = status.atk;
                size = 4;
                delay.Add(0.2f);
                break;
            case Entity.STATE.SKILL1:
                atk = detailStatus.skill1_damge;
                size = 6;
                delay.Add(0.2f);
                break;
            case Entity.STATE.SKILL2:
                atk = detailStatus.skill2_damge;
                size = 4;
                delay.Add(0.2f);
                delay.Add(0.2f);
                delay.Add(0.2f);
                break;
            case Entity.STATE.SKILL3:
                atk = detailStatus.skill3_damge;
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
        Vector3 dir = player.player.transform.forward;
        for (int i=0; i<deltaTime.Count ; i++)
        {
            yield return new WaitForSeconds(deltaTime[i]);
            Arrow arrow = Instantiate(skillEffect, skillTransfrom);
            arrow.Init(dir, atk, size);
            Destroy(arrow, 5);
        }

    }
}
