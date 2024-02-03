using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateRutePoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            FindObjectOfType<EyalController>().StartMovement();
        }
        gameObject.SetActive(false);
    }
}
