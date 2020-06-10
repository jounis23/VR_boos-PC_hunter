using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class CameraSetting : MonoBehaviourPun
{

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
        {
            //Destroy(this.gameObject);
            GetComponent<Camera>().enabled = false;
            GetComponent<AudioListener>().enabled = false;
        }


    }

    // Update is called once per frame
    void Update()
    {

    }

}