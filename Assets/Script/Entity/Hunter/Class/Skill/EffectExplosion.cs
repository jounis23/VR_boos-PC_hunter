using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


// 폭발 이펙트
// 이 이펙트가 적(boss)에 TriggerEnter하면 Attack 실행
// 오브젝트 풀링 구조로 5초 뒤에 제자리로 돌아가며 SetActive(false)
public class EffectExplosion : MonoBehaviourPun
{
    // 폭발 이펙트
    // 아쳐의 화살에만 사용 되며, 폭발의 크기를 변경 할때 사용
    public ParticleSystem explosion;

    // 콜라이더
    // 폭발의 범위 변경시 사용(아쳐만 사용)
    public SphereCollider coll;

    // 이펙트 사용 후 돌아갈 위치
    public Transform parent;

    // 이 이펙트를 사용한 직업
    // 이 이펙트 자체가 적(boss)를 공격하는 형태
    // 보스 입장에서 누가 공격(Attack)을 얼마나 했는지를 알기 위해 직업이름도 같이 넘김
    public string className;

    // 이 이펙트의 공격력
    // 각 직업은 공격력이 변할 수 있다 가정하고 공격 이펙트 실행시 매번 공격력을 설정하게 구현
    public float atk;

    // 아쳐만 사용
    // 스킬에 따라 이펙트 범위 변경
    private float size;

    // 이펙트 지속 시간
    // 기본적으로 모두 5초간 유지 됨
    private float duration = 0;
    public float durationTime = 5;


    // 포톤 사용 여부
    private bool isNetwork = true;



    //#############################################################################
    //#############################################################################
    //#############################################################################


    private void Start()
    {
        Entity entity = this.transform.root.GetComponent<Entity>();
        if(entity !=null)
            isNetwork = entity.isNetwork;
    }


    //#############################################################################
    //#############################################################################
    //#############################################################################
    // 이 이펙트에 대한 공격력 등을 초기화

    // 아쳐 일반공격, 스킬 이펙트
    // 공격력, 폭발크기, 직업이름을 받아옴
    public void InitArcherEffect(float atk, float size, string className = "None")
    {
        if (isNetwork)
            photonView.RPC("InitArcherEffectRPC", RpcTarget.All, atk, size, className);
        else
            InitArcherEffectRPC(atk, size, className);

    }

    [PunRPC]
    public void InitArcherEffectRPC(float atk, float size, string className)
    {
        this.atk = atk;
        this.size = size;
        coll.radius = size;
        explosion.startSize = size * 2.5f;
        this.transform.parent = null;
        this.className = className;

    }




    //#############################################################################

    // 소서러스 폭발 이펙트 (스킬 3,4번)
    // 공격력, 직업 이름
    public void InitSorceresEffect(float atk, string className)
    {
        if (isNetwork)
            photonView.RPC("InitSorceresEffectRPC", RpcTarget.All, atk, className);
        else
            InitSorceresEffectRPC(atk, className);

    }


    [PunRPC]
    public void InitSorceresEffectRPC(float atk, string className)
    {
        this.atk = atk;
        this.transform.rotation = parent.transform.rotation;
        this.className = className;
        this.transform.parent = null;
    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 업데이트
    // 이펙트는 5초간 유지 됨
    void Update()
    {
        duration += Time.deltaTime;
        if (duration > durationTime)
        {
            Reset();
        }

    }



    //#############################################################################
    
    // 리셋
    // 이펙트 일정시간 유지 후 안보이게 재설정
    private void Reset()
    {
        duration = 0;
        this.transform.parent = parent;
        this.transform.position = parent.position;
        this.transform.rotation = parent.rotation;

        /*
        Transform[] child = gameObject.transform.GetComponentsInChildren<Transform>();

        for(int i = 0; i < child.Length; i++)
        {
            child[i].position = this.transform.position;
            child[i].rotation = this.transform.rotation;
        }
        */

        if (explosion != null)
            explosion.transform.position = this.transform.position;

        this.gameObject.SetActive(false);
    }




    //#############################################################################
    //#############################################################################
    //#############################################################################

    // 폭발 이펙트로 보스가 맞으면 공격 실행
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[Skill Hit] : " + this.name + " ->" + other.name);

        if (!photonView.IsMine)
            return;

        // 공격 대상은 Boss
        if (other.CompareTag("Boss"))
        {
            Entity enemy = other.transform.root.GetComponentInChildren<Entity>();
            if (enemy != null)
            {
                // 적 공격 (공격력, 방무뎀, 직업이름)
                enemy.Attacked(atk, false, className);
            }
        }
        else if (other.CompareTag("MapObject"))
        {
            Rigidbody rigid = other.GetComponent<Rigidbody>();
            if (rigid != null)
                rigid.AddExplosionForce(atk, this.transform.position, size);
        }

    }
}
