using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (gameObject.name == "Event_FirstFall")
            {
                other.GetComponent<PlayerController>().FallingIntro();
            }
        }
    }
}
