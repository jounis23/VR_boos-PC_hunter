using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossSkillUI : MonoBehaviour
{
    public GameObject explanationImg;
    public float MAXCooltime;
    public Image cooltimeImg;
    public float damage;

    public BossSkill bs;
    public Boss boss;
    public BossSkill.BossSkillState skillState;
    public GameObject skillObj;

    public Text cooltimeText;
    public Text damageText;

    protected float cooltime = 0.0f;
    protected bool isCooltime = false;

    private void Awake()
    {
        if(bs == null)
            bs = GameObject.Find("Controller (left)").GetComponent<BossSkill>();
        if(boss == null)
            boss = GameObject.Find("VRBoss").GetComponent<Boss>();
    }

    void Start()
    {
    }

    void Update()
    {
        if (cooltime > 0.0f)
        {
            cooltime -= Time.deltaTime;
            cooltimeImg.fillAmount = cooltime / MAXCooltime;
        }
        else if (cooltime < 0.0f) 
        {
            isCooltime = false;
            cooltime = 0.0f;
            cooltimeImg.fillAmount = 0.0f;
        }
    }

    public void SetMAXCooltime(float ct)
    {
        MAXCooltime = ct;
    }

    public void SetDamage(float dmg)
    {
        damage = dmg;
        if(skillObj != null)
        {
            BossSkillDamage skillDamage = skillObj.GetComponentInChildren<BossSkillDamage>();
            skillDamage.damage = damage;
            skillDamage.boss = boss;
        }
    }

    public virtual void ReadySkillCasting()
    {
        if (isCooltime)
        {
            return;
        }
        bs.enabled = true;
        bs.SkillStateChange(skillState);
        boss.SkillUIOff();
        cooltime = MAXCooltime;
        isCooltime = true;
    }

    public virtual void UpdateText()
    {
        cooltimeText.text = "재사용 대기시간 : " + MAXCooltime.ToString("N1") + "초";
        damageText.text = "공격력 : " + damage.ToString("N0");
    }

    public void SetExplanationImg(bool tf)
    {
        explanationImg.SetActive(tf);
    }
}
