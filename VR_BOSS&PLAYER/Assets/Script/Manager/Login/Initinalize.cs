using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Initinalize : MonoBehaviour
{
    public GameObject obj;



    void Start()
    {
        PhotonNetwork.Instantiate(obj.name, Vector3.zero, Quaternion.identity);

        //if(PhotonNetwork.IsMasterClient)
        //    PhotonNetwork.Instantiate("LoginManager", Vector3.zero, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
