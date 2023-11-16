using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EhyalController : MonoBehaviour
{
    private Animator _anim;
    public PlayerController _sagar;
    private void Start()
    {

    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    public void GoFirstFollow()
    {
        _anim.SetTrigger("Follow1");
        _anim.SetLayerWeight(1, 1f);
    }

    public void GoSecondFollow()
    {
        _anim.SetTrigger("Follow2");
        _anim.SetLayerWeight(1, 1f);
    }


}
