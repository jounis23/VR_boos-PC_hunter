using System.Collections;
using UnityEngine;
using Photon.Pun;



// 버서커의 기본 공격
// 무기의 콜라이더에 따라 TriggerEnter되면 공격
// 스킬 4번의 경우 바닥을 내리치는 스킬로 여기에서 처리하게됨
public class BerserkerHit : MonoBehaviourPun
{
    private Hunter player;
    private STATE state;

    private ClassEntity classEntity;


    private bool isNetwork;


    void Start()
    {
        player = this.transform.root.GetComponent<Hunter>();
        classEntity = this.transform.root.GetComponent<ClassEntity>();
        isNetwork = player.isNetwork;
    }
    



    // 기본 공격과 스킬4번 처리
    private void OnTriggerEnter(Collider other)
    {
        state = player.state;

        if (state != STATE.ATTACK && state != STATE.SKILL4)
            return;

        Debug.Log("[" + state.ToString() + "] " + player.name + " ->" + other.name + " / " + player.status.atk);

        // 스킬 4번
        // 땅에 닿아야 스킬4의 이펙트 발생과 공격 실행
        if (other.CompareTag("CanUseSkill") && state == STATE.SKILL4)
        {
            if (isNetwork)
                photonView.RPC("Skill4EffectRPC", RpcTarget.All);
            else
                Skill4EffectRPC();
        }


        // 기본 공격
        else if (other.CompareTag("Boss"))
        {
            Entity enemy = other.transform.root.GetComponentInChildren<Entity>();
            if (enemy != null)
            {
                enemy.Attacked(player.status.atk, true);
            }


        }
    }


    [PunRPC]
    public void Skill4EffectRPC()
    {
        StartCoroutine(classEntity.SkillEffect(3));
    }



}
