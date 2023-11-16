using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour
{
    private Animator _signAnim;

    private void Start()
    {
        _signAnim = GetComponent<Animator>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            PlayerController controller = collision.GetComponent<PlayerController>();

            _signAnim.SetTrigger("Obtained");
            this.gameObject.SetActive(false);
        }
    }
}
