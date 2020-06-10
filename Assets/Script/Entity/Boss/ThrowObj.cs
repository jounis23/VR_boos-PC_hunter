using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using DG.Tweening;
using Valve.VR.Extras;

public class ThrowObj : MonoBehaviour
{
    public SteamVR_Input_Sources hand;
    public SteamVR_Action_Boolean grabPinch;
    public SteamVR_Behaviour_Pose mPose = null;

    private Transform obj;
    private bool isChildren = false;
    private bool isTemp = false;
    void Start()
    {
        mPose = GetComponent<SteamVR_Behaviour_Pose>();
    }

    void Update()
    {
        if (grabPinch.GetStateDown(hand) && obj != null)
        {
            MakeChildren();
        }
        if (grabPinch.GetStateUp(hand) && obj != null)
        {
            OutChildren();
        }
    }

    private void MakeChildren()
    {
        obj.parent = this.transform;
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        rig.useGravity = false;
        rig.isKinematic = true;
        isChildren = true;
    }

    private void OutChildren()
    {
        obj.parent = null;
        Rigidbody rig = obj.GetComponent<Rigidbody>();
        rig.useGravity = true;
        rig.isKinematic = false;
        rig.velocity = 5.0f * mPose.GetVelocity();
        rig.angularVelocity = mPose.GetAngularVelocity();
        isChildren = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("enter : " + other.name.ToString());
        if (other.CompareTag("Throwable"))
        {
            Debug.Log("!!");
            isTemp = true;
            obj = other.transform.parent.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit : " + other.name.ToString());
        if (other.CompareTag("Throwable"))
        {
            Debug.Log("!!!");
            isTemp = false;
            obj = null;
        }
    }
}
