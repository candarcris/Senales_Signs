using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionZone : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerSkills controller = collision.GetComponent<PlayerSkills>();
            controller.Die();
        }
    }
}
