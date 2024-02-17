using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            PlayerSkills controller = collision.GetComponent<PlayerSkills>();
            controller.Die();
        }
    }
}
