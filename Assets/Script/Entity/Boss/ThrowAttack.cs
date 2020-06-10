using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttack : MonoBehaviour
{
    private Rigidbody rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }


    private void OnCollisionEnter(Collision collision)
    {
        Hunter hunter = collision.gameObject.GetComponent<Hunter>();

        if (rigid == null || hunter == null)
            return;

        if(rigid.velocity.magnitude > 2)
        {
            hunter.Attacked(rigid.velocity.magnitude * 100, true);
        }
    }
}
