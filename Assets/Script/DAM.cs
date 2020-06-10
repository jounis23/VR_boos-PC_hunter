using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DAM : MonoBehaviour
{
    Hunter h;
    // Start is called before the first frame update
    void Start()
    {
        h = GetComponent<Hunter>();
        StartCoroutine(t());
    }

    // Update is called once per frame
    void Update()
    {
    }


    IEnumerator t()
    {
        while (true)
        {
            float ran = Random.Range(0, 3);
            yield return new WaitForSeconds(1f);
            h.Attacked(ran*100);
        }
    }
}
