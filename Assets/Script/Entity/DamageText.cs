using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    public float moveSpeed = 1.0f;
    public TextMeshProUGUI textMesh;
    Color color;


    private void Awake()
    {
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    public void Init(string damage, Color color)
    {
        textMesh.text = damage;
        this.color = color;
        Destroy(gameObject, 1.5f);
        transform.rotation = GameObject.FindWithTag("MainCamera").transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0));
        color.a -= (Time.deltaTime/2);

        textMesh.color = color;
        textMesh.fontSize -= (Time.deltaTime / 2);
    }

}
