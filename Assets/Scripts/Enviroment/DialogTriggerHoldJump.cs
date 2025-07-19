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
    public Transform _enemyTarget;
    public LevelManager _levelManager;

    private void Start()
    {
        _levelManager.OnSetEnemies += SetTarget;
    }

    public void SagarContinueWalking()
    {
        _playerController.SetFallingDrag(0);
        _cinemaCamera.Follow = _playerController.gameObject.transform;
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
        ManagerLocator.GetDialogsManager().OnFinishDialog = null;
        ManagerLocator.GetDialogsManager().OnFinishDialog += SagarContinueWalking;
        _cinemaCamera.Follow = _enemyTarget;
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }
}
