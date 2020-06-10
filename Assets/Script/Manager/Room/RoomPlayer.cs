using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System;

public class RoomPlayer : MonoBehaviourPun
{
    // player 관리
    // player : hunter(pc), boss(vr)
    public RoomManager roomManager;

    // 현재 player가 vr인지 확인
    public bool isVR;

    // 현재 player 정보
    public int hunterNumber;
    public RoomManager.CLASS hunterClass;
    public GameObject cam;
    public Canvas canvasVR;

    // 현재 player UI
    public Image myClassImage;
    public Text myClassText;
    public Text myPartText;

    // 자신의 ready 상태
    public bool isReady;

    // 다른 player UI
    public Image[] hunterUI;
    public Text allReadyText;
    public Text playerCountText;

    // boss UI
    public Image bossUI;

    // 자신이 boss일 경우
    public GameObject boss;



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 초기화
    private void Awake()
    {
        if (!photonView.IsMine)
        {
            cam.gameObject.SetActive(false);
            if(canvasVR != null)
                canvasVR.gameObject.SetActive(false);
        }


        hunterClass = RoomManager.CLASS.NONE;

    }

    private void Start()
    {
        if (photonView.IsMine && isVR)
        {
            photonView.RPC("SetVR", RpcTarget.All, isVR);
        }


        if (isVR)
        {
            this.name = "[Room] Boss ";
        }
        else
        {
            this.name = "[Room] Hunter ";
        }

        roomManager = FindObjectOfType<RoomManager>();

        if (PhotonNetwork.IsMasterClient)
        {
            if(!isVR)
                roomManager.AddPlayer(this);
            else
                roomManager.AddPlayerVR(this);
        }


    }


    [PunRPC]
    public void SetVR(bool isVR)
    {
        this.isVR = isVR;
    }


    [PunRPC]
    public void SetPlayerNumber(int number)
    {
        // hunter일 경우 들어온 순서에 따라 0~3 부여
        // boss일 경우 vr로서 5 부여

        if (PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetPlayerNumber", RpcTarget.Others, number);
        }


        hunterNumber = number;
        this.name += hunterNumber;
    }





    //#############################################################################
    //#############################################################################
    //#############################################################################

    // VR유저 입장에서 보이는 UI업데이트
    // PC유저의 Ready 한 숫자만 보임
    public void UpdateBossUI(int count, int number)
    {
        photonView.RPC("UpdateBossUICount", RpcTarget.All, count, number);
    }

    [PunRPC]
    public void UpdateBossUICount(int count, int number)
    {
        // 자신이 boss일경우 UI는 hunter들의 숫자만 표시

        playerCountText.text = "Hunter : " + number + "/4";
        allReadyText.text = "Ready : " + count + "/" + number;
    }



    //#############################################################################

    // PC유저 입장에서 보이는 UI업데이트
    // 다른 PC유저가 선택한 직업, Ready 여부를 보여줌
    public void UpdatePlayerUI(string data, bool isVRReady)
    {
        photonView.RPC("UpdateUIPC", RpcTarget.All, data);

        if(roomManager.bossPlayer!=null)
            photonView.RPC("UpdateUIVR", RpcTarget.All, isVRReady);
    }


    // PC유저 입장에서 보이는 다른 PC유저 UI 업데이트
    [PunRPC]
    private void UpdateUIPC(string data)
    {

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
            if(i == hunterNumber)
            {
                ex = 1;
                continue;
            }

            Debug.Log(i + " / " + ex + " / " + uiClass.Length);
            int classNum = (int)uiClass[i];
            if(classNum >= 0 )
                hunterUI[i-ex].sprite = roomManager.classImage[classNum];

            Text tmpText = hunterUI[i - ex].transform.GetChild(0).GetComponent<Text>();
            if (tmpText != null)
                tmpText.text = uiClass[i].ToString();

            tmpText = hunterUI[i - ex].transform.GetChild(1).GetComponent<Text>();
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


    // PC유저 입장에서 보이는 VR유저 UI 업데이트
    [PunRPC]
    public void UpdateUIVR(bool isReady)
    {
        bossUI.sprite = roomManager.bossImage;
        Text tmpText = bossUI.transform.GetChild(0).GetComponent<Text>();
        if (tmpText != null)
            tmpText.text = "Demon";

        tmpText = bossUI.transform.GetChild(1).GetComponent<Text>();
        if (tmpText != null)
        {
            if(isReady)
                tmpText.text = "Ready";
            else
                tmpText.text = "";
        }

    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 직업 선택
    // 버튼 클릭으로 선택
    public void SelectClass(bool nextPage)
    {
        if (isReady)
            return;

        if (nextPage)
        {
            hunterClass++;
            if ((int)hunterClass >= (int) RoomManager.CLASS.COUNT)
                hunterClass = 0;
        }
        else
        {
            hunterClass--;
            if ((int)hunterClass < 0)
                hunterClass = RoomManager.CLASS.COUNT - 1;
        }


        myClassImage.sprite = roomManager.classImage[ (int)hunterClass];
        myClassText.text = (hunterClass).ToString();


    }




    //#############################################################################

    // Ready버튼 클릭으로 Ready
    public void ReadyButtonClick(GameObject button)
    {
        if (!isVR && hunterClass < 0)
            return;

        isReady = !isReady;


        if (isReady)
            button.GetComponent<Image>().color = Color.green;
        else
            button.GetComponent<Image>().color = Color.red;

        Ready((int)hunterClass, isReady);

    }


    [PunRPC]
    public void Ready(int calssNumber, bool ready)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("Ready", RpcTarget.MasterClient, calssNumber, ready);
            return;
        }


        roomManager.PlayerReady(hunterNumber, calssNumber, ready, isVR);

    }




    //#############################################################################

    // 버튼에 마우스 커서가 올라 갔을때
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





    //#############################################################################
    //#############################################################################
    //#############################################################################

    // VR유저와 PC유저 모두 Ready가 되면 시작
    public void StartPlay()
    {

        photonView.RPC("StartPlayRPC", RpcTarget.All);
    }

    [PunRPC]
    public void StartPlayRPC()
    {


        if (!photonView.IsMine)
        {
            return;
        }


        // Hunter 시작
        if (!isVR)
        {
            this.transform.position = roomManager.spwanPoint[hunterNumber].position;
            PhotonNetwork.Instantiate(roomManager.playerPrefab[(int)hunterClass].name, this.transform.position, Quaternion.identity);


            Destroy(this.gameObject, 0.5f);
        }

        // Boss 시작
        else
        {
            this.transform.position = roomManager.spwanBoss.position;

            if (boss != null)
            {
                boss.GetComponent<VRPlayerLobbyScript>().StartGameRPC();
                boss.transform.parent = null;

                Destroy(this.gameObject, 0.5f);
            }
        }
        //GameManager.instance.Init();
        
        //Instantiate(GameManager.instance.soundManager.PlayLoopSound("BGM_Game1"));


    }

}
