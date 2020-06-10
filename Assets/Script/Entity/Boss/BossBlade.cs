using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Photon.Pun;

public class BossBlade : MonoBehaviourPun
{
    public SteamVR_Behaviour_Pose behaviour_Pose;
    public Boss boss;
    
    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            float damage = boss.status.atk;
            Vector3 vel = behaviour_Pose.GetVelocity();
            int mag = (int)(100 * vel.magnitude);
            if (mag < 100)
            {
                damage = 100.0f;
            }
            else if(mag > 500)
            {
                damage = 500 * damage;
            }
            else
            {
                damage = mag * damage;
            }
            other.transform.root.GetComponent<Hunter>().Attacked(damage);
        }
    }

}
