using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class LoginManager : MonoBehaviour
{

    public Text textInfo;


    public void Awake()
    {
        if (!PhotonNetwork.IsMasterClient)
            Destroy(this.gameObject);


        DontDestroyOnLoad(this.gameObject);

        textInfo.text = "Login";
    }


    public void CallRegister(string id, string pass)
    {
        StartCoroutine(Register(id, pass));
    }

    IEnumerator Register(string id, string pass)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", id);
        form.AddField("Password", pass);


        WWW www = new WWW("http://localhost/sqlconnect/register.php", form);
        yield return www;

        if(www.text.Contains("Success"))
        {
            textInfo.text = "User Register Success";

        }
        else
        {
            textInfo.text = www.text;

        }
    }




    public void CallLogin(string id, string pass)
    {
        StartCoroutine(Login(id, pass));
    }

    IEnumerator Login(string id, string pass)
    {
        WWWForm form = new WWWForm();
        form.AddField("Id", id);
        form.AddField("Password", pass);


        WWW www = new WWW("http://localhost/sqlconnect/login.php", form);
        yield return www;

        if (www.text.Contains("Success"))
        {
            textInfo.text = "User Login Success";

        }
        else
        {
            textInfo.text = www.text;

        }
    }
}
