using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerEvent : MonoBehaviour
{
    protected abstract void OnTriggerEnter(Collider other);
    protected abstract void DoTriggerEvent();
}
