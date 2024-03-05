using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DarkAura : MonoBehaviour
{
    private Animator _darkAnim;
    private BoxCollider _darkCollider;

    private void Start()
    {
        _darkAnim = GetComponent<Animator>();
        _darkCollider = GetComponent<BoxCollider>();
    }

    public void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {


        }
    }

    public void OnTriggerExit(Collider collision)
    {
        if (collision.tag == "Player")
        {

        }
    }
}
