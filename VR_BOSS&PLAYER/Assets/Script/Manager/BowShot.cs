using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowShot : MonoBehaviour
{

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Bow"))
            Debug.Log("shot");
    }
}
