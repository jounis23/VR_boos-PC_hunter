using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class RoomPlayer : MonoBehaviourPun
{


    public int playerNumber;
    public RoomManager.CLASS palyerClass;
    public Camera cam;

    public Image myClassImage;
    public Text myClassText;
    public Text myPartText;

    public bool isReady;

    public RoomManager roomManager;
    public Image[] playerUI;


    private void Awake()
    {
        
        if (!photonView.IsMine)
            cam.gameObject.SetActive(false);

        this.name ="[Room] Player ";

        palyerClass = RoomManager.CLASS.NONE;
        
    }

    private void Start()
    {

        roomManager = FindObjectOfType<RoomManager>();
        if (PhotonNetwork.IsMasterClient)
        {
            roomManager.AddPlayer(this);
            //roomManager.playerList.Add(this);
            //roomManager.UpdatePlayer();
        }
        /*

        RoomPlayer[] o = FindObjectsOfType<RoomPlayer>();


        playerNumber = o.Length-1;
        name += playerNumber;
        */
    }




    [PunRPC]
    public void SetPlayerNumber(int number)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetPlayerNumber", RpcTarget.Others, number);
        }


        playerNumber = number;
    }




    // Update is called once per frame
    void Update()
    {
        
    }


    public void UpdatePlayerUI(string data)
    {

        photonView.RPC("UpdateUI", RpcTarget.All, data);
    }

    [PunRPC]
    public void UpdateUI(string data)
    {


        Debug.Log(this.name + " : UpdataUI : "+data);
        string[] uiData = data.Split('/');
        int size = uiData.Length - 1;        

        RoomManager.CLASS[] uiClass = new RoomManager.CLASS[size];
        bool[] uiReady = new bool[size];


        for (int i = 0; i< size; i++)
        {
            string[] tmp = uiData[i].Split('_');
            uiClass[i] = (RoomManager.CLASS) Convert.ToInt32(tmp[0]);
            uiReady[i] = Convert.ToBoolean(tmp[1]);
        }

        int ex = 0;
        for (int i = 0; i < size; i++)
        {
            if(i == playerNumber)
            {
                Debug.Log(this.name + "#@!$");
                ex = 1;
                continue;
            }

            Debug.Log(i + " / " + ex + " / " + uiClass.Length);
            int classNum = (int)uiClass[i];
            if(classNum >= 0 )
                playerUI[i-ex].sprite = roomManager.classImage[classNum];

            Text tmpText = playerUI[i - ex].transform.GetChild(0).GetComponent<Text>();
            if (tmpText != null)
                tmpText.text = uiClass[i].ToString();

            tmpText = playerUI[i - ex].transform.GetChild(1).GetComponent<Text>();
            if (tmpText != null)
            {
                tmpText.text = uiReady[i].ToString();
                if (uiReady[i])
                    tmpText.color = Color.green;
                else
                    tmpText.color = Color.red;
            }


        }
        

    }





    public void SelectClass(bool nextPage)
    {
        if (isReady)
            return;

        if (nextPage)
        {
            palyerClass++;
            if ((int) palyerClass >= (int) RoomManager.CLASS.COUNT)
                palyerClass = 0;
        }
        else
        {
            palyerClass--;
            if ((int) palyerClass < 0)
                palyerClass = RoomManager.CLASS.COUNT - 1;
        }


        myClassImage.sprite = roomManager.classImage[ (int) palyerClass ];
        myClassText.text = (palyerClass).ToString();


    }




    public void ReadyButtonClick(GameObject button)
    {
        if (palyerClass < 0)
            return;

        isReady = !isReady;


        if (isReady)
            button.GetComponent<Image>().color = Color.green;
        else
            button.GetComponent<Image>().color = Color.red;

        Debug.Log(this.name + " ready "+ isReady);


        Ready((int)palyerClass, isReady);
    }


    [PunRPC]
    public void Ready(int calssNumber, bool ready)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("Ready", RpcTarget.MasterClient, calssNumber, ready);
            return;
        }


        roomManager.PlayerReady(playerNumber, calssNumber, ready);

    }



    public void ReadyButtonHover(GameObject button)
    {
        if (!photonView.IsMine)
            return;

        button.GetComponent<Image>().color = Color.yellow;
        
    }
    public void ReadyButtonHoverEnd(GameObject button)
    {
        if (!photonView.IsMine)
            return;

        if (button.name.Contains("Ready"))
        {

            if (!isReady)
                button.GetComponent<Image>().color = Color.red;
            else
                button.GetComponent<Image>().color = Color.green;
        }
        else
        {

            button.GetComponent<Image>().color = Color.white;
        }
    }


}
