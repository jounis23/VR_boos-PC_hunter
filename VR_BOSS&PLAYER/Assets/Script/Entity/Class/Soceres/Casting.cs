using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Casting : MonoBehaviour
{
    float atk;
    Transform skillTransform;
    private void Awake()
    {
        this.transform.parent = null;
    }
    private void Start()
    {
        StartCoroutine(CastingState());
    }
    public void Init(Transform skillTransform, float atk)
    {
        this.atk = atk;
        this.skillTransform = skillTransform;
    }
    
    
    IEnumerator CastingState()
    {
        float moveX;
        float moveY;
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                break;

            moveX = Input.GetAxis("Horizontal");
            moveY = Input.GetAxis("Vertical");
            this.transform.Translate(new Vector3(moveX, 0, moveY) * 3 * Time.deltaTime);
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        skillTransform = this.transform;
        Destroy(this.gameObject);
    }
}
