using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DialogTriggerEvent : TriggerEvent
{
    public PlayerController _playerController;
    [SerializeField] private bool _isGroundAfterFirstFall;
    //BoxCollider _boxCollider;
    public Animator _cinemaCameraDutchAnim;
    private float tiempo = 0;

    private void Start()
    {
        //_boxCollider = GetComponent<BoxCollider>();
    }

    protected override void DoTriggerEvent()
    {
        _isGroundAfterFirstFall = true;
        _playerController.SetFallingDrag(0);
        if (_cinemaCameraDutchAnim != null)
        {
            _cinemaCameraDutchAnim.speed = 5.0f;
        }
        _playerController.SetAnimation("FirstFall", false, true);
    }

    private void Update()
    {
        if(_isGroundAfterFirstFall == true)
        {
            tiempo += Time.deltaTime / 1f;
            tiempo = Mathf.Clamp01(tiempo); // asegura que no pase de 1
            ManagerLocator.GetHUDManager()._faithBar2.gameObject.SetActive(true);
            ManagerLocator.GetHUDManager().SetFaithAmount(Mathf.Lerp(0, _playerController._faithMaxAmount, tiempo));
            if (tiempo >= 1f)
            {
                _isGroundAfterFirstFall = false; // opcional: ya terminó
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }
}
