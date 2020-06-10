using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBuff : MonoBehaviour
{
    public float buffDuration;      // 버프 : 지속 시간
    public float changeAtk;         // 버프 : 공격력 변화
    public float changeDef;         // 버프 : 방어 변화
    public float changeSpeed;       // 버프 : 속도 변화

    public bool isTrueDamage;
}
