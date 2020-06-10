using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks {

    public SteamVR_Action_Boolean action_Boolean;
    public SteamVR_Input_Sources handType;
    public bool isVR;

    private string gameVersion = "1"; // 게임 버전

    //public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    //public Button joinButton; // 룸 접속 버튼

    public bool isConnected;

    public GameObject LoadingCanvers;
    public GameObject LobbyPanel;
    public GameObject SearchCanvers;
    public GameObject LoadingWait;
    public GameObject MenualPanel;

    private bool gameStart = false;
    private bool isStart = false;

    public Image progressBar;
    public Text progressText;
    private float barFillAmount;

    public int maxPlayer = 5;

    private void Awake()
    {
        Screen.SetResolution(1280, 720, false);
        
    }

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start() {

        SceneInitalize lnit = FindObjectOfType<SceneInitalize>();
        if (lnit != null)
            this.isVR = lnit.isVR;

        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        isConnected = false;
        //joinButton.interactable = false;
        //connectionInfoText.text = "Connecting Server...";
        barFillAmount = 0.05f;
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster() {
        //joinButton.interactable = true;
        isConnected = true;
        //connectionInfoText.text = "Server Connected";
        barFillAmount = 1f;
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        //joinButton.interactable = false;
        isConnected = false;
        //connectionInfoText.text = "Server Disconnected";
        
    }

    // 룸 접속 시도
    public void Connect() {
        //joinButton.interactable = false;
        isConnected = false;
        if (PhotonNetwork.IsConnected)
        {
            //connectionInfoText.text = "Join Room...";
            PhotonNetwork.JoinRandomRoom();

        }
        else
        {
            //connectionInfoText.text = "Server Disconnection Error";
            PhotonNetwork.ConnectUsingSettings();

        }
    }



    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message) {

        //connectionInfoText.text = "Create Room";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 5 });
    }




    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom() {
        //connectionInfoText.text = "Success Join Room";

        StartCoroutine(WaitPlayer());
    }






    //#############################################################################
    //#############################################################################
    //#############################################################################

    // Room 참가 후 5명이 모일때까지 기다림
    IEnumerator WaitPlayer()
    {
        Debug.Log("Current Player : " + PhotonNetwork.CurrentRoom.PlayerCount);
        SearchCanvers.SetActive(true);
        
        while (true)
        {

            if (PhotonNetwork.CurrentRoom.PlayerCount > maxPlayer || isStart)
            {

                LoadingWait.SetActive(true);
                yield return new WaitForSeconds(PhotonNetwork.CurrentRoom.PlayerCount*0.1f);
                break;
                
            }
            yield return null;
        }
        Screen.SetResolution(1920, 1080, true);
        PhotonNetwork.LoadLevel("Room");
        yield return null;
    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 업데이트
    public void Update()
    {
        // 강제 시작
        if (Input.GetKeyDown(KeyCode.F5))
            isStart = true;


        //#############################################################################
        // 프로그래스 바 업데이트
        if (barFillAmount < 1f)
        {
            barFillAmount += 0.05f;
            progressBar.fillAmount = barFillAmount;

            if (barFillAmount < 0.2f)
            {
                progressText.text = "한림대학교 캡스톤 디자인...";
            }
            else if (barFillAmount < 0.5f)
            {
                progressText.text = "노트북 들고오는 중...";
            }
            else if (barFillAmount < 0.75f)
            {
                progressText.text = "바이브 먼지 터는 중...";
            }
            else if (barFillAmount < 1f)
            {
                progressText.text = "룰루랄라 게임 하는 중...";
            }

        }



        //#############################################################################
        // 서버와 연결된 상태가 아니면 리턴
        if (!isConnected)
            return;

        //progressText.text = progressMesage;



        //#############################################################################
        // 서버가 연결 된 후 조작을 통해 Room 참가

        // VR 플레이어 경우
        if (isVR)
        {
            // 트리거 누르면 Room 참가
            if (isConnected && action_Boolean.GetStateDown(handType))
            {
                // Room 참가
                Connect();
            }
        }

        // PC 플레이어 경우
        else
        {
            if (isConnected && !LobbyPanel.activeSelf)
            {
                LoadingCanvers.SetActive(false);
                LobbyPanel.SetActive(true);
            }

            // Start버튼 클릭하면 Room 참가
            if (gameStart)
            {
                // Room 참가
                Connect();

                //Instantiate(GameManager.instance.soundManager.PlayLoopSound("BGM_Lobby2"));
            }
        }


    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    public void OnStartButtonClick()
    {
        gameStart = true;
    }

    public void OnMenualButtonClick()
    {
        MenualPanel.SetActive(true);
    }

}