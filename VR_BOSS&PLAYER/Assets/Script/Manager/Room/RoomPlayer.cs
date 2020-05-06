using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomPlayer : MonoBehaviourPun
{
    public Image myClassImage;
    public Text myClassText;
    public bool isReady;

    private int pageClass;
    public string[] className = { "Archer", "Beserker", "Knight", "Sorceres" };
    public Sprite[] classImage;


    public List<RoomPlayer> playerList;
    public bool isMaster;
    public bool isAllReady;


    private void Awake()
    {
        isMaster = PhotonNetwork.IsMasterClient;

        this.name = "Player_" + photonView.ViewID;
        if (isMaster)
        {
            this.name += " (Host)";
            playerList = new List<RoomPlayer>();
            isAllReady = false;
        }
        else
            this.name += " (Client)";

        isReady = false;
        pageClass = -1;
    }

    private void Start()
    {
        if (photonView.IsMine)
            AddPlayer(this);
    }




    // Update is called once per frame
    void Update()
    {
        
    }








    [PunRPC]
    public void AddPlayer(RoomPlayer player)
    {
        if (!isMaster)
        {
            photonView.RPC("AddPlayer", RpcTarget.MasterClient, this);
            return;
        }

        playerList.Add(player);
    }



    [PunRPC]
    public void CheckReady()
    {

        if (!isMaster)
        {
            photonView.RPC("CheckReady", RpcTarget.MasterClient);
            return;
        }

        if (playerList.Count != 5)
            return;

        foreach (RoomPlayer p in playerList)
        {
            isAllReady = isAllReady && p.isReady;
        }


        if (isAllReady)
            StartCoroutine(AllReady());
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




    public void SelectClass(bool nextPage)
    {
        if (!photonView.IsMine)
            return;

        if (nextPage)
        {
            pageClass++;
            if (pageClass >= className.Length)
                pageClass = 0;
        }
        else
        {
            pageClass--;
            if (pageClass < 0)
                pageClass = className.Length - 1;
        }


        myClassImage.sprite = classImage[pageClass];
        myClassText.text = className[pageClass];
    }




    public void ReadyButtonClick(GameObject button)
    {

        if (!photonView.IsMine)
            return;

        if (pageClass < 0)
            return;

        isReady = !isReady;

        if(isReady)
            button.GetComponent<Image>().color = Color.green;
        else
            button.GetComponent<Image>().color = Color.white;
    }

    public void ReadyButtonHover(GameObject button)
    {
        if (!photonView.IsMine)
            return;

        if (!isReady)
            button.GetComponent<Image>().color = Color.red;
    }
    public void ReadyButtonHoverEnd(GameObject button)
    {
        if (!photonView.IsMine)
            return;

        if (!isReady)
            button.GetComponent<Image>().color = Color.white;
    }
}
