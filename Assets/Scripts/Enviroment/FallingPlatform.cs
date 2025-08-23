using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class FallingPlatform : MonoBehaviour
{
    private Rigidbody _rigidBody;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody>();
    }

    public void Fall()
    {
        StartCoroutine(WaitAndDeactivate());
    }

    public IEnumerator WaitAndDeactivate()
    {
        yield return new WaitForSeconds(1);

        _rigidBody.isKinematic = false;

        yield return new WaitForSeconds(4);

        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Fall();
        }
    }
}
