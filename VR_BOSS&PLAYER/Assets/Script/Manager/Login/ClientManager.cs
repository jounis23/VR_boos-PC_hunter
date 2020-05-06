using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class ClientManager : MonoBehaviourPun
{
    string userId;
    int userLevel;
    int userRating;

    public bool isMaster;

    public InputField idFiled;
    public InputField passFiled;

    public Button button;
    public Button buttonRegist;

    public Canvas canvas;

    public bool isRegist;

    public LoginManager loginManager;

    public void Awake()
    {

        DontDestroyOnLoad(this.gameObject);
        isMaster = PhotonNetwork.IsMasterClient;

        GameObject master = GameObject.FindGameObjectWithTag("Host");
        if (master == null && isMaster)
        {
            this.gameObject.name = "Host";
            this.gameObject.tag = "Host";
        }
        else
        {
            this.transform.name = "Client_" + photonView.ViewID;
        }

        if (!photonView.IsMine)
            gameObject.SetActive(false);



        using (StreamWriter outputFile = new StreamWriter(@"Log.txt", true))
        {
            outputFile.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + " = " + this.name + " : Entry Room");
        }
    }

    public void Start()
    {
        isRegist = false;
        button.interactable = false;
        loginManager = FindObjectOfType<LoginManager>();


    }


    [PunRPC]
    public void Regist()
    {
        if (!photonView.IsMine)
            return;


        isRegist = (buttonRegist.GetComponentInChildren<Text>().text == "Regist");

        //Text textInfo = GameObject.FindGameObjectWithTag("Info").GetComponent<Text>();

        if (isRegist)
        {
            //if(textInfo!=null)
            //    textInfo.text = "Input Id & Password To Regist";

            buttonRegist.GetComponentInChildren<Text>().text = "Close";
            button.GetComponentInChildren<Text>().text = "Regist";
            idFiled.text = "";
            passFiled.text = "";
        }
        else
        {
            //if (textInfo != null)
            //    textInfo.text = "Login";

            buttonRegist.GetComponentInChildren<Text>().text = "Regist";
            button.GetComponentInChildren<Text>().text = "Login";
            idFiled.text = "";
            passFiled.text = "";
        }
    }



    public void OnButton()
    {
        if (!photonView.IsMine)
            return;

        AccessDB(isRegist, idFiled.text, passFiled.text);
    }


    [PunRPC]
    public void AccessDB(bool isRegi, string id, string pass)
    {

        using (StreamWriter outputFile = new StreamWriter(@"Log.txt", true))
        {
            string access = "";
            if (isRegi)
                access = "Regist";
            else
                access = "Login";

            outputFile.WriteLine(System.DateTime.Now.ToString("yyyy-MM-dd") + " = " + this.name + " : Access DataBase ( " + access +" = Id : " + id +" / Password : " + pass +")");
        }

        if (!isMaster)
        {
            photonView.RPC("AccessDB", RpcTarget.MasterClient, isRegi, id, pass);
        }
        else
        {

            loginManager = FindObjectOfType<LoginManager>();

            if (isRegi)
            {
                loginManager.CallRegister(id, pass);
            }
            else
            {
                loginManager.CallLogin(id, pass, this.gameObject);
            }
        }

    }


    public void VerifyInputs()
    {
        if (!photonView.IsMine)
            return;

        // 아이디와 패스워드 8글자 이상
        button.interactable = (idFiled.text.Length >= 4 && passFiled.text.Length >= 8);

    }



    [PunRPC]
    public void LoadPlayScene()
    {
        if (isMaster)
        {
            photonView.RPC("LoadPlayScene", RpcTarget.Others);
            return;
        }
        else if (!photonView.IsMine)
        {
            return;
        }

        PhotonNetwork.LoadLevel("Main");
    }

}
