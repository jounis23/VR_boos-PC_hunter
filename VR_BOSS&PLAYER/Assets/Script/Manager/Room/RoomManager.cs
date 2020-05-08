using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;

public class RoomManager : MonoBehaviourPun
{

    public GameObject player;
    public List<RoomPlayer> playerList;
    public bool[] playerReady;
    public CLASS[] playerClass;

    public bool isMaster;

    public enum CLASS
    {
        NONE = - 1,
        Archer = 0,
        Beserker= 1, 
        Knight = 2,
        Sorceres = 3,
        COUNT = 4
    }

    public Sprite[] classImage;





    private void Awake()
    {

        isMaster = PhotonNetwork.IsMasterClient;

        playerReady = new bool[4];
        playerClass = new CLASS[4];
        playerList = new List<RoomPlayer>();

        for (int i = 0; i < playerClass.Length; i++)
        {
            playerClass[i] = CLASS.NONE;
        }


        PhotonNetwork.Instantiate(player.name, Vector3.zero, Quaternion.identity);

    }



    
    public void AddPlayer(RoomPlayer player)
    {
        if (!isMaster)
            return;

        Debug.Log("Add : " + player.name);
        if (playerList.Contains(player))
            return;

        playerList.Add(player);


        player.SetPlayerNumber(playerList.Count - 1);
        player.name += player.playerNumber;


        UpdatePlayer();
    }



    



    [PunRPC]
    public void PlayerReady(int playerNumber, int classNumber, bool isReady)
    {
        Debug.Log("  Ready : " +playerNumber + " / " + (CLASS)classNumber + " / " + isReady);
        if (!isMaster)
        {
            photonView.RPC("PlayerReady", RpcTarget.MasterClient, playerNumber, classNumber, isReady);
            return;
        }

        playerReady[playerNumber] = isReady;
        playerClass[playerNumber] = (CLASS) classNumber;


        UpdatePlayer();
    }




    public void UpdatePlayer()
    {
        //Debug.Log("Update");
        string data = "";

        for(int i = 0; i < playerList.Count; i++)
        {
            data += ((int)playerClass[i]) + "_";
            data += playerReady[i].ToString() + "/";
        }

        Debug.Log("Update : "+ playerList.Count);
        
        foreach (RoomPlayer p in playerList)
        {
            //p.UpdateUI(data);
            p.UpdatePlayerUI(data);
        }
        
    }







    public void CheckReady()
    {
        /*
        if (!isMaster)
        {
            photonView.RPC("CheckReady", RpcTarget.Others);
            return;
        }
        */
        bool result = true;
        foreach(RoomPlayer p in playerList)
        {
            result = result && p.isReady;
        }


        if (result)
        {
            //isAllReady = result;
            StartCoroutine(AllReady());
        }


    }

    IEnumerator AllReady()
    {
        yield return new WaitForSeconds(3);
        photonView.RPC("CheckReady", RpcTarget.All);
    }


    [PunRPC]
    public void StartGame()
    {
        if (!photonView.IsMine)
            return;

        PhotonNetwork.LoadLevel("Main");
    }




}
