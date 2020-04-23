using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightHit : MonoBehaviour
{
    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        Player enemy = other.GetComponent<Player>();
        switch (player.state)
        {
            case Entity.STATE.ATTACK:
                enemy.status.hp -= player.status.atk - enemy.status.def;
                break;
            case Entity.STATE.SKILL1:
                Debug.Log("hit");
                player.GetComponent<ClassKnight>().Skill1Active();
                break;
            case Entity.STATE.SKILL4:
                player.GetComponent<ClassKnight>().Skill4Active();
                break;

        }
    }

}
