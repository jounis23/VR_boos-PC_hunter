using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



// 활 쏘는 애니메이션과 화살 이펙트 생성 타이밍을 위해 따로 구성
// 3d모델의 손과 화살에 콜라이더를 두어 두개가 TriggerExit일때 화살이 나가게 됨 (화살 쏘는 경우가 아니면 둘이 겹칠 일이 없음)
// 이 스크립트는 손에 있으며 콜라이더도 같이 존재함

// 화살 이펙트 오브젝트를 오브젝트 풀로 구성
// 총 5개를 미리 SetActive(false)
public class ArrowShot : MonoBehaviourPun
{
    // 화살 이펙트를 담는 객체
    public GameObject quiver;

    // 화살 리스트
    public List<Arrow> shotEffect;

    // 화살 번호
    // 화살은 5개로 0~4의 번호를 가짐
    // 화살을 쏠때 순차적으로 쏘게 됨 SetActive(true)
    // 쏜 화살은 일정 시간후 다시 SetActive(false)가 되어 리셋됨
    private int arrowNumber;



    // 화살에 대한 공격력 등의 수치를 받을 객체 정보
    // 스킬은 기본공격력 x 스킬 공격력으로 계산됨
    // 기본 공격력은 player의 status에 있음
    // 스킬 공격력은 classEntity의 skillinfo에 있음
    private Hunter player;
    private STATE state;
    private STATUS status;
    private ClassArcher classEntity;



    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 초기화
    private void Start()
    {
        arrowNumber = 0;
        player = this.transform.root.GetComponent<Hunter>();
        classEntity = this.transform.root.GetComponent<ClassArcher>();
        shotEffect = new List<Arrow>();

        for (int i = 0; i < quiver.transform.childCount; i++)
        {
            shotEffect.Add(quiver.transform.GetChild(i).GetComponent<Arrow>());
        }

    }



    //#############################################################################
    //#############################################################################
    //#############################################################################
    // 화살 발사 Trigger

    // 화살과 손이 떼어졌을 때 실행
    // 화살 한개를 SetActive(ture)
    private void OnTriggerExit(Collider other)
    {
        // TriggerExit가 활일 경우에만 실행
        if (!other.CompareTag("Bow") )
            return;

        // 케릭터의 스테이터스와 상태를 고려
        state = player.state;
        status = player.status;

        // 화살의 공격력과 폭발 범위
        // 화살이 무언가에 맞으면 폭발함 (공격력을 실제로 폭발에 적용됨)
        float atk = status.atk;
        float size = 1;

        // 화살 쏘는 딜레이 시간(애니메이션과 타이밍을 맞추기 위해)
        // delay가 2개 이상인 경우에는 화살 연사의 경우 (각 화살 사이 시간 간격을 의미)
        List<float> delay = new List<float>();


        // 상태에 따라 스킬 공격력, 범위, 딜레이를 설정
        switch (state)
        {
            case STATE.ATTACK:
                size *= 3;
                delay.Add(0.2f);
                break;
            case STATE.SKILL1:
                atk *= classEntity.skill[0].skillAttack.skillDamage / 100;
                size *= 5;
                delay.Add(0.2f);
                break;
            case STATE.SKILL2:
                atk *= classEntity.skill[1].skillAttack.skillDamage / 100;
                size *= 2;
                delay.Add(0.2f);
                delay.Add(0.2f);
                delay.Add(0.2f);
                break;
            case STATE.SKILL3:
                atk = classEntity.skill[2].skillAttack.skillDamage / 100;
                size *= 8;
                delay.Add(0.2f);
                break;

            default:
                return;
        }


        // 스피드 스테이터스에 따라 크리티컬 적용
        int critical = Random.Range(0, 100);
        if (critical <= (status.speed*2))
            atk *= 1.25f;


        // 화살 발사 (화살 오브젝트를 SetActive(true) )
        // 발사 타이밍을 맞추기 위해 delay를 넣어 코루틴으로 실행
        StartCoroutine(ArrowShoot(delay, atk, size));
    }



    //#############################################################################

    // 화살 발사
    public IEnumerator ArrowShoot(List<float> deltaTime, float atk, float size)
    {
        // 화살의 방향
        Vector3 dir = player.player.transform.forward;
        //Debug.DrawRay(player.player.transform.position, dir, Color.red, 5);


        // 화살 발사 타이밍 조절
        // delay가 여러개 들어온 경우에는 화살 여러개 발사 됨
        // ex) delay가 2개인 경우는 화살 2개 발사
        for (int i=0; i<deltaTime.Count ; i++)
        {
            // 초기 대기시간
            yield return new WaitForSeconds(deltaTime[i]);

            // 포톤 사용 여부에 따라 화살 동기화 설정
            // 혼자 테스트일땐 포톤 사용x
            if(player.isNetwork)
                photonView.RPC("ArrowShotRPC", RpcTarget.All, arrowNumber, dir, atk, size);
            else
                ArrowShotRPC( arrowNumber, dir, atk, size);

            // 화살 넘버
            // 0~4 까지 총 5개가 있어 순차적으로 발사됨
            arrowNumber++;

            // 화살 넘버가 0~4까지 순환되도록 설정
            if (arrowNumber >= shotEffect.Count)
                arrowNumber = 0;
        }

    }


    //#############################################################################


    // 화살 동기화
    // 각각 모든 유저 게임 씬의 아쳐에 있어서 화살을 SetActive(true)를 동기화 시킴
    [PunRPC]
    public void ArrowShotRPC(int number, Vector3 dir, float atk, float size)
    {
        Arrow arrow = shotEffect[number];

        // 화살을 씬에서 보이게 함
        // SetActive(true)되면 바로 파티클 시스템이 실행되어 이펙트가 보이게됨
        arrow.gameObject.SetActive(true);

        // 화살에 방향, 공격력, 범위 설정
        arrow.Init(dir, atk, size);

    }




}
