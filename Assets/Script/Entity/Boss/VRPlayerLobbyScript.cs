using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class VRPlayerLobbyScript : MonoBehaviourPun
{
    public GameObject conR;
    public GameObject conL;
    public GameObject bossModel;
    public Boss boss;
    public BossSkill bossSkill;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartGameRPC()
    {
        photonView.RPC("StartGame", RpcTarget.All);
    }

    [PunRPC]
    public void StartGame()
    {
        conR.SetActive(false);
        conL.SetActive(false);
        bossModel.SetActive(true);
        boss.enabled = true;
        bossSkill.enabled = true;
        bossSkill.SetDamageRPC();
        boss.SetPointer(false);
    }

}
