using Photon.Pun; // 유니티용 포톤 컴포넌트들
using Photon.Realtime; // 포톤 서비스 관련 라이브러리
using UnityEngine;
using UnityEngine.UI;

// 마스터(매치 메이킹) 서버와 룸 접속을 담당
public class LobbyManager : MonoBehaviourPunCallbacks {
    private string gameVersion = "1"; // 게임 버전

    public Text connectionInfoText; // 네트워크 정보를 표시할 텍스트
    //public Button joinButton; // 룸 접속 버튼

    public GameObject joinText;
    public bool isConnected;

    // 게임 실행과 동시에 마스터 서버 접속 시도
    private void Start() {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.ConnectUsingSettings();

        isConnected = false;
        joinText.SetActive(false);
        //joinButton.interactable = false;
        connectionInfoText.text = "Connecting Server...";
    }

    // 마스터 서버 접속 성공시 자동 실행
    public override void OnConnectedToMaster() {
        //joinButton.interactable = true;
        isConnected = true;
        joinText.SetActive(true);
        connectionInfoText.text = "Server Connected";
    }

    // 마스터 서버 접속 실패시 자동 실행
    public override void OnDisconnected(DisconnectCause cause)
    {
        //joinButton.interactable = false;
        isConnected = false;
        joinText.SetActive(false);
        connectionInfoText.text = "Server Disconnected";

    }

    // 룸 접속 시도
    public void Connect() {
        //joinButton.interactable = false;
        isConnected = false;
        if (PhotonNetwork.IsConnected)
        {
            connectionInfoText.text = "Join Room...";
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            connectionInfoText.text = "Server Disconnection Error";
            PhotonNetwork.ConnectUsingSettings();

        }
    }

    // (빈 방이 없어)랜덤 룸 참가에 실패한 경우 자동 실행
    public override void OnJoinRandomFailed(short returnCode, string message) {

        connectionInfoText.text = "Create Room";
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 4});
    }

    // 룸에 참가 완료된 경우 자동 실행
    public override void OnJoinedRoom() {

        connectionInfoText.text = "Success Join Room";
        PhotonNetwork.LoadLevel("Room");
    }

    public void Update()
    {

        if(isConnected && Input.anyKey)
        {
            Connect();
        }
    }
}