using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Initinalize : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.Instantiate("ClientManager", Vector3.zero, Quaternion.identity);

        if(PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("LoginManager", Vector3.zero, Quaternion.identity);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
