using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowExplosion : MonoBehaviour
{

    float atk;
    public ParticleSystem explosion;
    public SphereCollider coll;

    public void Init(float atk, float size)
    {
        this.atk = atk;
        coll.radius = size;
        explosion.startSize = size * 2.5f;
        Destroy(this.gameObject, 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ASDF");
        if (other.CompareTag("Boss"))
        {
            other.GetComponent<Entity>().Attacked(atk);
            Debug.Log(other.name + " : " + atk);
        }
    }
}
