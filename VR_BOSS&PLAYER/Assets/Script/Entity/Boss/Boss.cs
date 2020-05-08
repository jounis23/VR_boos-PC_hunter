using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using DG.Tweening;

public class Boss : Entity
{
    public SteamVR_Input_Sources handType;
    public SteamVR_Action_Boolean grabGrib;
    public GameObject blade;

    void Start()
    {
    }


    void Update()
    {
        if (grabGrib.GetStateDown(handType))
        {
            BladeOn();
        }
        if (grabGrib.GetStateUp(handType))
        {
            BladeOff();
        }
    }

    
    void BladeOn()
    {
        blade.transform.DOKill();
        blade.SetActive(true);
        blade.transform.DOScale(2.0f, 0.7f).SetEase(Ease.InQuad);
    }
    void BladeOff()
    {
        blade.transform.DOKill();
        Tweener bladeTween = blade.transform.DOScale(0.0f, 0.2f).SetEase(Ease.InQuad);
        bladeTween.OnComplete(() => {
            blade.SetActive(false);
        });
    }

}
