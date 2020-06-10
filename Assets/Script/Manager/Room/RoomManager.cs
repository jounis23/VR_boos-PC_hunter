using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using ExitGames.Client.Photon;
using UnityEngine.XR;
using System.Globalization;
using Photon.Realtime;

public class RoomManager : MonoBehaviourPun
{

    // 자신이 Host인지 체크
    public bool isMaster;

    // 자신이 VR인지 체크
    public bool isVR;

    // 보스 플레이어 (VR로 진행)
    public RoomPlayer bossPlayer;

    // 헌터 플레이어 (PC로 진행)
    public List<RoomPlayer> hunterPlayerList;

    // 보스와 헌터 Ready여부
    public bool bossReady;
    public bool[] hunterReady;

    // 헌터 플레이어가 선택한 직업 리스트
    public CLASS[] hunterClass;


    // 게임 시작할 맵
    public GameObject map;

    // 화면에 직업, Ready를 표시할 UI
    public GameObject ui;


    public GameManager gameManager;

    // 직업 종류
    public enum CLASS
    {
        NONE = - 1,
        Archer = 0,
        Berserker= 1, 
        Knight = 2,
        Sorceress = 3,
        COUNT = 4
    }


    // UI에 표시 할 직업과 보스 이미지
    public Sprite[] classImage;
    public Sprite bossImage;



    // 게임 시작 시 생성 할 헌터 플레잉어 케릭터
    public GameObject[] playerPrefab;

    // 게임 시작 시 적용 할 플레이어들의 위치
    public Transform[] spwanPoint;
    public Transform spwanBoss;




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 초기화
    private void Awake()
    {
        if (!XRDevice.isPresent)
        {
            XRSettings.enabled = false;
            isVR = false;
        }
        else
        {
            XRSettings.enabled = true;
            isVR = true;
        }

        isMaster = PhotonNetwork.IsMasterClient;

        hunterReady = new bool[4];
        hunterClass = new CLASS[4];
        hunterPlayerList = new List<RoomPlayer>();

        for (int i = 0; i < hunterClass.Length; i++)
        {
            hunterClass[i] = CLASS.NONE;
        }

        GameObject p;
        if (!isVR)
        {
            p = PhotonNetwork.Instantiate("[Room]Player", Vector3.zero, Quaternion.identity);
        }
        else
        {
            p = PhotonNetwork.Instantiate("[Room]VR_Player", Vector3.zero, Quaternion.identity);
        }
        p.GetComponent<RoomPlayer>().isVR = this.isVR;

    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 업데이트
    // 강제 시작
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            hunterReady[0] = true;
            hunterReady[1] = true;
            hunterReady[2] = true;
            hunterReady[3] = true;
            bossReady = true;

            CheckReady();
        }
    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // PC플레이어 리스트에 추가 (최대4명)
    public void AddPlayer(RoomPlayer player)
    {
        if (!isMaster)
            return;


        if (!hunterPlayerList.Contains(player))
            hunterPlayerList.Add(player);

        player.SetPlayerNumber(hunterPlayerList.Count - 1);



        UpdatePlayer();
    }



    //#############################################################################

    // VR플레이어 추가 (최대 1명)
    public void AddPlayerVR(RoomPlayer player)
    {
        if (!isMaster)
            return;

        bossPlayer = player;
        player.SetPlayerNumber(5);


        UpdatePlayer();
    }





    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 누군가 Ready를 눌렀을 때
    [PunRPC]
    public void PlayerReady(int playerNumber, int classNumber, bool isReady, bool isVRSetting)
    {
        Debug.Log("[Ready] Number : " +playerNumber + " / " + (CLASS)classNumber + " / " + isReady);
        if (!isMaster)
        {
            photonView.RPC("PlayerReady", RpcTarget.MasterClient, playerNumber, classNumber, isReady);
            return;
        }

        if (!isVRSetting)
        {
            hunterReady[playerNumber] = isReady;
            hunterClass[playerNumber] = (CLASS)classNumber;
        }
        else
        {
            bossReady = isReady;
        }


        UpdatePlayer();
    }



    // Ready를 눌렀을 땐, 직업 선택도 완료되어 모든 유저의 UI를 업데이트 시킴
    [PunRPC]
    public void UpdatePlayer()
    {
        if (!isMaster)
        {
            photonView.RPC("UpdatePlayer", RpcTarget.MasterClient);
            return;
        }

        string data = "";

        for (int i = 0; i < hunterPlayerList.Count; i++)
        {
            data += ((int)hunterClass[i]) + "_";
            data += hunterReady[i].ToString() + "/";
        }



        foreach (RoomPlayer p in hunterPlayerList)
        {
            p.UpdatePlayerUI(data, bossReady);
        }
        


        if (bossPlayer != null)
        {

            int count = 0;
            for (int i = 0; i < hunterPlayerList.Count; i++)
            {
                if (hunterReady[i])
                {
                    count++;
                }
            }
            bossPlayer.UpdateBossUI(count, hunterPlayerList.Count);
        }




        CheckReady();
    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 모든 유저가 Ready인지 체크
    // 인원도 모두 차 있어야 함 (PC:4, VR:1)
    public void CheckReady()
    {
        if (!isMaster)
            return;
        
        bool result = true;
        int count = 0;
        for(int i =0; i< hunterPlayerList.Count; i++)
        {
            result = result && hunterReady[i];
            if (hunterReady[i])
                count++;
            if (!result)
                break;
        }
        result = result && bossReady;

        if (result)
        {
            foreach (RoomPlayer p in hunterPlayerList)
            {
                p.StartPlay();
            }

            if(bossPlayer!=null)
                bossPlayer.StartPlay();

            Room room = PhotonNetwork.CurrentRoom;
            room.IsOpen = false;
            room.MaxPlayers = room.PlayerCount;

            //Debug.Log(room.IsOpen +"/"+ room.MaxPlayers);

            PhotonNetwork.Instantiate("[Room] Temple", Vector3.zero, Quaternion.identity);

            photonView.RPC("LetsPlay", RpcTarget.All);
        }


    }



    //#############################################################################

    // 준비가 완료되면 게임 시작
    [PunRPC]
    public void LetsPlay()
    {
        gameManager.Init();

        // 가비지 컬렉터 적용
        //System.GC.Collect();


        Destroy(ui.gameObject);
        Destroy(this.gameObject, 0.5f);

    }


}
