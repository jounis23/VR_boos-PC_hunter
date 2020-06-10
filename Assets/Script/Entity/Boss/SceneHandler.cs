using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR.Extras;

public class SceneHandler : MonoBehaviour
{
    //public SteamVR_LaserPointer rightLaserPointer;
    public SteamVR_LaserPointer leftLaserPointer;

    void Awake()
    {
        //rightLaserPointer.PointerIn += PointerInside;
        //rightLaserPointer.PointerOut += PointerOutside;
        //rightLaserPointer.PointerClick += PointerClick;
        leftLaserPointer.PointerIn += PointerInside;
        leftLaserPointer.PointerOut += PointerOutside;
        leftLaserPointer.PointerClick += PointerClick;
    }

    public void PointerClick(object sender, PointerEventArgs e)
    {
        if (e.target.CompareTag("SkillUI"))
        {
            e.target.GetComponent<BossSkillUI>().ReadySkillCasting();
        }
        else if (e.target.name == "ButtonReady")
        {
            e.target.GetComponent<Button>().onClick.Invoke();
        }
        
    }

    public void PointerInside(object sender, PointerEventArgs e)
    {
        if (e.target.CompareTag("SkillUI"))
        {
            e.target.GetComponent<BossSkillUI>().SetExplanationImg(true);
        }
    }

    public void PointerOutside(object sender, PointerEventArgs e)
    {
        if (e.target.CompareTag("SkillUI"))
        {
            e.target.GetComponent<BossSkillUI>().SetExplanationImg(false);
        }
    }
}