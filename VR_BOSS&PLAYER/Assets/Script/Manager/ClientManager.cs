using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

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

    private LoginManager loginManager;

    public void Awake()
    {

        DontDestroyOnLoad(this.gameObject);
        isMaster = PhotonNetwork.IsMasterClient;

    }

    public void Start()
    {
        isRegist = false;
        button.interactable = false;
        loginManager = FindObjectOfType<LoginManager>();

        if(loginManager == null)
        {
            ClientManager[] clients = FindObjectsOfType<ClientManager>();
            foreach (ClientManager c in clients)
            {
                if (c.isMaster)
                    loginManager = c.loginManager;

                if (!c.photonView.IsMine)
                    c.canvas.gameObject.SetActive(false);
            }
        }

    }


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

        if (isRegist)
        {
            loginManager.CallRegister(idFiled.text, passFiled.text);
        }
        else
        {
            loginManager.CallLogin(idFiled.text, passFiled.text);
        }
    }


    public void VerifyInputs()
    {
        if (!photonView.IsMine)
            return;

        // 아이디와 패스워드 8글자 이상
        button.interactable = (idFiled.text.Length >= 4 && passFiled.text.Length >= 8);

    }
}
