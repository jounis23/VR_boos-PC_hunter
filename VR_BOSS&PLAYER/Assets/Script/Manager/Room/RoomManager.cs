using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomManager : MonoBehaviourPun
{

    public List<RoomPlayer> playerList;
    public bool isMaster;
    public bool isAllReady;


    private void Awake()
    {
        isMaster = PhotonNetwork.IsMasterClient;

        if (!isMaster)
            Destroy(this.gameObject);

        isAllReady = false;
    }



    public void AddPlayer(RoomPlayer player)
    {
        playerList.Add(player);
    }


    public void CheckReady()
    {
        if (playerList.Count != 5)
            return;

        foreach(RoomPlayer p in playerList)
        {
            isAllReady = isAllReady && p.isReady;
        }


        if (isAllReady)
            StartCoroutine(StartGame());


    }

    IEnumerator StartGame()
    {
        yield return new WaitForSeconds(3);
    }
}
