using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
    public string _colliderName;
    private DialogEvent _dialogEvent;

    private void Start()
    {
        _dialogEvent = GetComponent<DialogEvent>();
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == _colliderName)
        {
            _dialogEvent.ExecuteDialogEvent();
            FindObjectOfType<PlayerController>().DialogMeanTimeValues();
        }
        gameObject.SetActive(false);
    }
}
