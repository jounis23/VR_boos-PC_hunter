using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Valve.VR;
using Photon.Pun;
using ExitGames.Client.Photon;

public class BossSkill : MonoBehaviourPun
{
    public GameObject mPointer;
    public SteamVR_Action_Boolean skillAction;
    public GameObject boss;
    public GameObject[] skillEffects;
    public GameObject[] skillUI;
    public float[] skillsCooltime;
    public float[] skillsDamage;


    public enum BossSkillState
    {
        NONE,
        SKILL1,
        SKILL2,
        SKILL3,
        SKILL4,
        SKILL5,
        SKILL6,
        SKILL7,
        SKILL8
    }

    public BossSkillState skillState;

    private Boss bossScript;
    private SteamVR_Behaviour_Pose mPose = null;
    private bool mHasPosition = false;
    private bool mIsUseSkill = false;
    private float mFadeTime = 0.5f;

    private void Awake()
    {
    }

    private void Start()
    {
        bossScript = boss.GetComponent<Boss>();
        mPose = GetComponent<SteamVR_Behaviour_Pose>();
        skillState = BossSkillState.NONE;
        bossScript.enabled = false;
        this.enabled = false;
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }

        mHasPosition = UpdatePointer();
        mPointer.SetActive(mHasPosition);

        if (skillAction.GetStateUp(mPose.inputSource))
        {
            if (!mHasPosition || mIsUseSkill)
                return;
            SkillCasting(skillState);
        }
    }

    public void SkillCasting(BossSkillState bossSkillState)
    {
        if(bossSkillState == BossSkillState.NONE)
        {
            this.enabled = false;
            return;
        }
        else if(bossSkillState == BossSkillState.SKILL1)
        {
            TrySkill_1();
        }
        else if (bossSkillState == BossSkillState.SKILL2)
        {
            TrySkill_2();
        }
        else if (bossSkillState == BossSkillState.SKILL3)
        {
            TrySkill_3();
        }
        else if (bossSkillState == BossSkillState.SKILL4)
        {
            TrySkill_4();
        }
        bossScript.SetPointer(false);
        SkillStateChange(BossSkillState.NONE);
        this.enabled = false;
    }


    private void TrySkill_1()
    {
        Vector3 translation = mPointer.transform.position;
        photonView.RPC("MoveRigRPC", RpcTarget.All, translation);
    }

    [PunRPC]
    void MoveRigRPC(Vector3 translation)
    {
        StartCoroutine(MoveRig(translation));
    }

    private IEnumerator MoveRig(Vector3 translation)
    {
        mIsUseSkill = true;

        SteamVR_Fade.Start(Color.black, mFadeTime, true);

        yield return new WaitForSeconds(mFadeTime);

        boss.transform.position = translation;

        SteamVR_Fade.Start(Color.clear, mFadeTime, true);

        mIsUseSkill = false;
    }

    private void TrySkill_2()
    {
        Vector3 skillCastPositionVector = new Vector3(mPointer.transform.position.x, 7.0f, mPointer.transform.position.z);
        Vector3 skillCastRotationVector = new Vector3(-90.0f, 0.0f, 0.0f);

        photonView.RPC("Skill_2_RPC", RpcTarget.All, skillCastPositionVector, skillCastRotationVector);
    }

    [PunRPC]
    void Skill_2_RPC(Vector3 skillCastPositionVector, Vector3 skillCastRotationVector)
    {
        StartCoroutine(Skill_2(skillCastPositionVector, skillCastRotationVector));
    }

    private IEnumerator Skill_2(Vector3 skillCastPositionVector, Vector3 skillCastRotationVector)
    {
        skillEffects[0].transform.position = skillCastPositionVector;
        skillEffects[0].transform.rotation = Quaternion.Euler(skillCastRotationVector);
        skillEffects[0].SetActive(true);

        yield return new WaitForSeconds(5.5f);
        skillEffects[0].SetActive(false);

    }

    private void TrySkill_3()
    {
        photonView.RPC("Skill_3_RPC", RpcTarget.All);
    }

    [PunRPC]
    void Skill_3_RPC()
    {
        StartCoroutine(Skill_3());
    }

    private IEnumerator Skill_3()
    {
        mIsUseSkill = true;

        skillEffects[1].SetActive(true);
        AudioSource audioSource = skillEffects[1].GetComponent<AudioSource>();
        if(audioSource != null)
        {
            audioSource.Play();
        }
        yield return new WaitForSeconds(13.5f);
        skillEffects[1].SetActive(false);

        mIsUseSkill = false;
    }

    private void TrySkill_4()
    {
        photonView.RPC("Skill_4_RPC", RpcTarget.All);
    }

    [PunRPC]
    void Skill_4_RPC()
    {
        StartCoroutine(Skill_4());
    }

    private IEnumerator Skill_4()
    {
        float temp = bossScript.status.atk;
        bossScript.status.atk = 2 * temp;
        yield return new WaitForSeconds(10.0f);
        bossScript.status.atk = temp;
    }

    public void SkillStateChange(BossSkillState i)
    {
        skillState = i;
    }

    private bool UpdatePointer()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;
        int layerMask = (1 << LayerMask.NameToLayer("Map")) + (1 << LayerMask.NameToLayer("UI"));
        if (Physics.Raycast(ray, out hit, 1000.0f, layerMask))
        {
            if (hit.transform.CompareTag("CanUseSkill"))
            {
                mPointer.transform.position = hit.point;
                return true;
            }
        }

        return false;
    }



    private void OnEnable()
    {
        mPointer.SetActive(true);
    }

    private void OnDisable()
    {
        mPointer.SetActive(false);
    }

    
    public void SetDamageRPC()
    {
        photonView.RPC("SetDamage", RpcTarget.All);
    }

    [PunRPC]
    public void SetDamage()
    {
        for (int i = 0; i < skillUI.Length; i++)
        {
            Debug.Log(skillUI[i].name.ToString());
            BossSkillUI tempBSU = skillUI[i].GetComponent<BossSkillUI>();
            tempBSU.SetMAXCooltime(skillsCooltime[i]);
            tempBSU.SetDamage(skillsDamage[i]);
            tempBSU.UpdateText();
        }
        bossScript.SkillUIOff();
        this.enabled = false;
    }
}
