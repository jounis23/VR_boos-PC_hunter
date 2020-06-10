using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using DG.Tweening;
using Valve.VR.Extras;
using Photon.Pun;
using ExitGames.Client.Photon;

public class Boss : Entity
{
    public SteamVR_Input_Sources anyHand;
    public SteamVR_Input_Sources rightHand;
    public SteamVR_Input_Sources leftHand;
    public SteamVR_Action_Boolean grabGrib;
    public SteamVR_Action_Boolean touchPadClick;
    public SteamVR_LaserPointer leftLaserPointer;
    //public SteamVR_LaserPointer rightLaserPointer;
    public GameObject blade;
    public GameObject skillUI;
    public GameObject leftController;
    public Camera vrCamera;
    public GameObject[] indexFinger;
    public GameObject[] middleFinger;
    public GameObject[] pinkyFinger;
    public GameObject[] ringFinger; 
    public GameObject[] thumbFinger; 

    private Vector3[] indexVec3s = { new Vector3(-5.917f, -57.698f, -43.249f),
            new Vector3(-5.17f, -27.299f, -17.572f), new Vector3(-16.197f, -36.48f, -43.353f) };
    private Vector3[] middleVec3s = { new Vector3(-40.265f, -37.857f, -57.804f),
            new Vector3(17.196f, -1.612f, -30.923f), new Vector3(8.661f, -7.301f, -28.931f) };
    private Vector3[] pinkyVec3s = { new Vector3(-30.591f, 7.983f, -70.916f),
            new Vector3(1.823f, -1.812f, -44.83f), new Vector3(0.0f, 0.004f, -36.853f) };
    private Vector3[] ringVec3s = { new Vector3(-33.437f, -24.277f, -56.051f),
            new Vector3(-8.074f, -5.578f, -32.129f), new Vector3(-27.176f, -29.602f, -37.478f) };
    private Vector3[] thumbVec3s = { Vector3.zero,
            new Vector3(41.384f, -16.683f, 46.755f), new Vector3(32.436f, 0.0f, 0.0f) };

    private bool onBlade = false;
    private bool isSkillCasting = false;
    private BossSkillUI[] bossSkillUIs;
    private Image[] skillUIImgs;
    private Collider[] skillUIColliders;
    private int bSUsLenth;
    private int imgsLenth;
    private int collidersLenth;

    public int phase;

    private Vector3 skillUICanvasLocalPosition;
    private Vector3 skillUICanvasLocalRotation;

    private void Awake()
    {
        bossSkillUIs = skillUI.GetComponentsInChildren<BossSkillUI>();
        skillUIImgs = skillUI.GetComponentsInChildren<Image>();
        skillUIColliders = skillUI.GetComponentsInChildren<BoxCollider>();
        bSUsLenth = bossSkillUIs.Length;
        imgsLenth = skillUIImgs.Length;
        collidersLenth = skillUIColliders.Length;
        skillUICanvasLocalPosition = skillUI.transform.localPosition;
        skillUICanvasLocalRotation = skillUI.transform.localEulerAngles;
        SkillUIOff();
    }


    void Start()
    {
    }


    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (!onBlade && grabGrib.GetStateDown(rightHand))
        {
            photonView.RPC("BladeOn", RpcTarget.All);
        }
        if (onBlade && grabGrib.GetStateUp(rightHand))
        {
            photonView.RPC("BladeOff", RpcTarget.All);
        }
        if (touchPadClick.GetStateDown(anyHand))
        {
            if (!isSkillCasting)
            {
                SkillUIOn();
            }
            else
            {
                SkillUIOff();
                SetPointer(false);
            }
        }
        
    }


    [PunRPC]
    void BladeOn()
    {
        MoveFinger(indexFinger, indexVec3s);
        MoveFinger(middleFinger, middleVec3s);
        MoveFinger(pinkyFinger, pinkyVec3s);
        MoveFinger(ringFinger, ringVec3s);
        MoveFinger(thumbFinger, thumbVec3s);
        blade.transform.DOKill();
        blade.GetComponent<BoxCollider>().enabled = true;
        blade.transform.DOScale(2.0f, 0.4f).SetEase(Ease.InQuad);
        onBlade = true;
    }

    [PunRPC]
    void BladeOff()
    {
        Vector3[] zeroVec3 = { Vector3.zero, Vector3.zero, Vector3.zero };
        MoveFinger(indexFinger, zeroVec3);
        MoveFinger(middleFinger, zeroVec3);
        MoveFinger(pinkyFinger, zeroVec3);
        MoveFinger(ringFinger, zeroVec3);
        MoveFinger(thumbFinger, zeroVec3);
        blade.transform.DOKill();
        blade.GetComponent<BoxCollider>().enabled = false;
        blade.transform.DOScale(0.0f, 0.2f).SetEase(Ease.InQuad);
        onBlade = false;
    }

    void MoveFinger(GameObject[] finger, Vector3[] vec3)
    {
        finger[0].transform.DOLocalRotate(vec3[0], 0.4f);
        finger[1].transform.DOLocalRotate(vec3[1], 0.4f);
        finger[2].transform.DOLocalRotate(vec3[2], 0.4f);
    }

    public void SkillUIOn()
    {
        skillUI.transform.localRotation = Quaternion.Euler(Vector3.zero);
        skillUI.transform.parent = null;
        skillUI.transform.LookAt(vrCamera.transform);
        skillUI.transform.Rotate(new Vector3(0, 1, 0), 180.0f);
        for (int i = 0; i < imgsLenth; i++)
        {
            skillUIImgs[i].enabled = true;
        }
        for (int i = 0; i < collidersLenth; i++)
        {
            skillUIColliders[i].enabled = true;
        }
        isSkillCasting = true; 
        SetPointer(true);
    }
    public void SkillUIOff()
    {
        skillUI.transform.parent = leftController.transform;
        skillUI.transform.localPosition = skillUICanvasLocalPosition;
        //skillUI.transform.localRotation = Quaternion.Euler(skillUICanvasLocalRotation);
        for (int i = 0; i < bSUsLenth; i++)
        {
            bossSkillUIs[i].SetExplanationImg(false);
        }
        for (int i = 0; i < imgsLenth; i++)
        {
            skillUIImgs[i].enabled = false;
        }
        for (int i = 0; i < collidersLenth; i++)
        {
            skillUIColliders[i].enabled = false;
        }
        isSkillCasting = false;
    }

    public void SetPointer(bool tf)
    {
        leftLaserPointer.enabled = tf;
        leftLaserPointer.holder.SetActive(tf);
    }

    public void UpdateHpUI()
    {

    }

    public void UpdateMpUI()
    {

    }

}
