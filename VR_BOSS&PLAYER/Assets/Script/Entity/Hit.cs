using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hit : MonoBehaviour
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
                
                break;
            case Entity.STATE.SKILL2:

                break;
            case Entity.STATE.SKILL3:
                player.GetComponent<ClassEntity>().Skill3(player);
                Debug.Log("하이루");
                break;
            case Entity.STATE.SKILL4:
                player.GetComponent<ClassEntity>().Skill4(player);
                Debug.Log("하이루");
                break;

        }
    }
}
