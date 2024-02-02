using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventFallingIntro : MonoBehaviour
{
    private DialogEvent _dialogEvent;

    private void Start()
    {
        _dialogEvent = GetComponent<DialogEvent>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _dialogEvent.ExecuteDialogEvent();
            FindObjectOfType<PlayerController>().FallingIntro();
        }
    }
}
