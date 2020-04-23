using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    Rigidbody rigid;
    float atk;
    float size;
    public ArrowExplosion collEffect;


    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        this.transform.parent = null;
    }
    public void Init(Vector3 direction, float atk, float size)
    {
        this.atk = atk;
        this.size = size;
        direction = direction.normalized;
        rigid.AddForce(direction * 1000);
        Destroy(this.gameObject, 5);
        Debug.Log(direction);
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bow"))
            return;

        if (other.CompareTag("MapObject") || other.CompareTag("Boss"))
        {
            ArrowExplosion effect = Instantiate(collEffect, this.transform.position, Quaternion.identity);
            effect.Init(atk, size);
            Destroy(effect.gameObject, 3);
            Destroy(this.gameObject);
            Debug.Log("hit" + other.name);
        }
    }
}
