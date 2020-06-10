using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SorceresHit : MonoBehaviour
{
    private Hunter player;
    private STATE state;
    private STATUS status;

    private ClassEntity classEntity;


    private void Awake()
    {
        player = this.transform.root.GetComponent<Hunter>();
        classEntity = this.transform.root.GetComponent<ClassEntity>();
    }


    public void Initialized(Hunter player, ClassEntity classEntity)
    {
        this.player = player;
        this.classEntity = classEntity;

    }



    private void OnTriggerEnter(Collider other)
    {
        state = player.state;
        status = player.status;

        if (other.CompareTag("Boss"))
        {
            Entity enemy = other.transform.root.GetComponent<Entity>();
            if (enemy == null)
                return;


            switch (state)
            {
                case STATE.ATTACK:
                    enemy.Attacked(status.atk);
                    break;
                case STATE.SKILL1:
                    enemy.UpdateHp(classEntity.skill[0].skillAttack.skillDamage);
                    break;
                case STATE.SKILL2:
                    StartCoroutine(RecoveryBuff(enemy, classEntity.skill[1].skillAttack.skillDamage));
                    break;
                case STATE.SKILL3:
                    enemy.Attacked(classEntity.skill[2].skillAttack.skillDamage);
                    //enemy.SpeedDown(classEntity.skill[2].changeSpeed);
                    break;
                case STATE.SKILL4:
                    enemy.Attacked(classEntity.skill[3].skillAttack.skillDamage);
                    break;
            }
            StartCoroutine(hit(3f));

        }
    }

    IEnumerator RecoveryBuff(Entity other, float count)
    {
        for(int i=0; i<count; i++) 
        { 
            //other.RecoveryHp(detailStatus.skill2_healHp);
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
    IEnumerator hit(float t = 1f)
    {
        this.GetComponent<Collider>().enabled = false;
        yield return new WaitForSeconds(t);
        this.GetComponent<Collider>().enabled = true;
    }
}
