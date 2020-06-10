using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkillUI_4 : BossSkillUI
{
    override public void ReadySkillCasting()
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
        bs.SkillCasting(skillState);
    }
    override public void UpdateText()
    {
        cooltimeText.text = "재사용 대기시간 : " + MAXCooltime.ToString("N1") + "초";
        damageText.text = "";
    }

}

