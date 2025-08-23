using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Dialogs))]
public class DialogTriggerHoldJump : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public PlayerController _playerController;
    public CinemachineVirtualCamera _cinemaCamera;
    [SerializeField] private Transform _enemyTarget;
    public LevelManager _levelManager;
    public BoxCollider _boxCollider;

    private void Awake()
    {
        _levelManager.OnSetEnemies += SetTarget;
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void SagarContinueWalking()
    {
        _playerController.SetFallingDrag(0);
        _cinemaCamera.Follow = _playerController.transform;
    }

    private void OnDisable()
    {
        _levelManager.OnSetEnemies -= SetTarget;
    }

    public void SetTarget()
    {
        _enemyTarget = _levelManager._enemyList[0].gameObject.transform;
    }

    protected override void DoTriggerEvent()
    {
        //_playerController.StopState();
        _cinemaCamera.Follow = _enemyTarget;
        ManagerLocator.GetDialogsManager().OnFinishDialog = null;
        ManagerLocator.GetDialogsManager().OnFinishDialog += SagarContinueWalking;
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
        //gameObject.SetActive(false);
        _boxCollider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }
}
