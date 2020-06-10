using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ClassSorceres : ClassEntity
{

    // 평타 이펙트, 위치
    public EffectExplosion[] attackEffect;
    public Transform attackTransfrom;
    public Transform skillTransfrom;

    public GameObject castingMark;


    private int attackNumber = 0;


    private void Start()
    {


        for (int i = 0; i < skill.Length; i++)
        {
            if (skill[i] == null)
                continue;


            EffectExplosion ex = skill[i].GetComponent<EffectExplosion>();

            if (ex != null)
                ex.InitSorceresEffect(skill[i].skillAttack.skillDamage, "Sorceres");


        }
    }

    public override void Attack()
    {


        StartCoroutine(MagicShot(player.status.atk));
    }

    IEnumerator MagicShot(float atk)
    {
        yield return new WaitForSeconds(0.7f);
        MagicShotRPC(attackNumber, atk, "Sorceres");
        attackNumber++;

        if (attackNumber >= attackEffect.Length)
            attackNumber = 0;

    }

    [PunRPC]
    public void MagicShotRPC(int number,  float atk, string className)
    {

        attackEffect[number].gameObject.SetActive(true);
        attackEffect[number].InitSorceresEffect(atk, className);

    }



    public override void Skill(int number)
    {

        // 스킬의 쿨타임 적용
        SetSkillCoolTime(number);

        player.UpdateMp(-skill[number].spendMp);

        if (skill[number].isCasting)
        {
            StartCoroutine(CastingState(number));
        }
        else
        {
            StartCoroutine(SkillEffect(number, 0.3f));
        }

    }




    IEnumerator CastingState(int number)
    {
        
        //player.state = STATE.CASTING;
        player.ActAnimation(STATE.CASTING);

        castingMark.transform.position = skillTransfrom.transform.position;
        castingMark.transform.GetChild(0).gameObject.SetActive(true);
        float moveX;
        float moveY;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
            castingMark.transform.Translate(new Vector3(moveX, 0, moveY) * 5 * Time.deltaTime);
            yield return null;
        }


        castingMark.transform.GetChild(0).gameObject.SetActive(false);
        skill[number].transform.position = castingMark.transform.position;

        player.state = STATE.IDLE;

        StartCoroutine(SkillEffect(number, 0.5f));

        switch (number)
        {
            case 0:
            case 2:
            case 3:
                player.ActAnimation(skill[number].state);
                break;
        }


    }



}