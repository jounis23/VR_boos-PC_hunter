using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HunterUI : MonoBehaviour
{
    public Entity entity;


    // 자신 UI표시
    public Text HpText;
    public Image HpImage;
    public Text MpText;
    public Image MpImage;


    // 팀원 UI표시
    public Image[] TeamHpImage;
    public Image[] TeamMpImage;
    public Text[] TeamClassText;

    // 보스 UI표시
    public Image BossHpImage;
    public Text BossHpText;


    // 스킬 아이콘 UI
    public Image[] skillImage = new Image[4];

    // 스킬 남은 쿨타임 UI 
    public Text[] skillText = new Text[4];

    public Image startImage;


    public DamageText damageText;
    public Canvas damageCanvas;


    public void Initialized()
    {
        startImage.gameObject.SetActive(false);
        UpdateHpUI();
        UpdateMpUI();
    }


    // 스킬 쿨타임 제어
    // UI에 스킬 쿨타임 표시
    public void UpdateSkillCoolTime(float[] skillresetTime, float[] skillCoolTime)
    {
        for (int i = 0; i < skillresetTime.Length; i++)
        {
            skillImage[i].fillAmount = 1 - (skillresetTime[i] / skillCoolTime[i]);
            skillText[i].text = skillresetTime[i].ToString("F1");
        }
    }



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // HP,MP에 대한 UI 업데이트
    // HP,MP관련 함수가 실행되면 적용됨
    public void UpdateHpUI()
    {
        HpText.text = ((int)entity.status.hp).ToString() + "/" + entity.status.hpMax;
        HpImage.fillAmount = entity.status.hp / entity.status.hpMax;
    }

    public void UpdateMpUI()
    {
        MpText.text = ((int)entity.status.mp).ToString() + "/" + entity.status.mpMax;
        MpImage.fillAmount = entity.status.mp / entity.status.mpMax;
    }

    public void UpdateTeamStatusUI(float[] teamhp, float[] teammp, float bossHp)
    {
        for (int i = 0; i < teamhp.Length; i++)
        {
            TeamHpImage[i].fillAmount = teamhp[i];
        }

        for (int i = 0; i < teammp.Length; i++)
        {
            TeamMpImage[i].fillAmount = teammp[i];
        }

        BossHpImage.fillAmount = bossHp;
        BossHpText.text = (bossHp*100).ToString("F2")+"%";
    }
    public void UpdateTeamStatusUIClass(string[] data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            TeamClassText[i].text = data[i];
        }

    }



    public void AttackedText(float damage)
    {
        Quaternion rot = this.transform.rotation;
        float ranX = Random.Range(-0.5f, 0.5f);

        Vector3 pos = this.transform.position + new Vector3(ranX, 2f, -0.5f);
        DamageText dt = Instantiate(damageText, pos, rot);
        dt.transform.parent = damageCanvas.transform;

        if (damage == 0)
            dt.Init("Miss", Color.blue);
        else
            dt.Init(damage.ToString(), Color.red);
    }
}
