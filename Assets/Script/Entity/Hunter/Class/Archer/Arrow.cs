using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Arrow : MonoBehaviourPun
{
    private bool isNetwork;
    public Transform quiver;
    public Transform hand;
    public EffectExplosion collEffect;
    private Rigidbody rigid;

    private float atk;
    private float size;
    private float duration = 0;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        isNetwork = transform.root.GetComponent<Hunter>().isNetwork;
    }

    public void Init(Vector3 direction, float atk, float size)
    {
        this.transform.rotation = Quaternion.identity;
        this.transform.position = hand.transform.position;
        this.transform.parent = null;
        this.atk = atk;
        this.size = size;
        direction = direction.normalized;

        rigid.AddForce(direction * 1000);
    }

    // Update is called once per frame
    void Update()
    {
        duration += Time.deltaTime;
        if (duration > 3)
        {
            ArrowReset();
        }

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bow"))
            return;

        if (other.CompareTag("MapObject") || other.CompareTag("Boss"))
        {
            //Debug.Log(other.tag + " / " + other.name);

            if (isNetwork)
                photonView.RPC("ArrowExplosionRPC", RpcTarget.All);
            else
                ArrowExplosionRPC();

            collEffect.InitArcherEffect(atk, size, "Archer");

            ArrowReset();
        }
    }

    private void ArrowReset()
    {
        rigid.velocity = Vector3.zero;
        this.atk = 0;
        this.size = 1;
        duration = 0;
        this.transform.position = quiver.position;
        this.transform.parent = quiver;

        this.gameObject.SetActive(false);
    }



    [PunRPC]
    public void ArrowExplosionRPC()
    {
        collEffect.gameObject.SetActive(true);
    }


}
