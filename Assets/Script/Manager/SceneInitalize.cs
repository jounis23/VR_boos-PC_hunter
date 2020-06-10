using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class SceneInitalize : MonoBehaviour
{
    public bool isVR;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);

        if (XRDevice.isPresent)
        {
            XRSettings.enabled = true;
            isVR = true;
            SceneManager.LoadScene("Lobby(VR)");

        }
        else
        {
            XRSettings.enabled = false;
            isVR = false;
            SceneManager.LoadScene("Lobby");
        }

        Destroy(this.gameObject, 1F);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
