using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerFirstSignal : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public BoxCollider _boxCollider;
    public CinemachineVirtualCamera _cinemaCamera;
    public EyalController _eyalController;
    public PlayerController _playerController;
    public Transform _signalPray;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void EhyalShowSign()
    {
        // Verificar si el GameObject está activo antes de iniciar la corrutina
        if (gameObject.activeInHierarchy)
        {
            StartCoroutine(WaitForEhyalShowSign());
        }
        else
        {
            Debug.LogWarning("GameObject inactivo, no se puede iniciar corrutina");
        }
    }

    public IEnumerator WaitForEhyalShowSign()
    {
        _playerController.StopState();

        _cinemaCamera.Follow = _signalPray;
        _cinemaCamera.m_Lens.FieldOfView = 30;

        _eyalController.transform.position = new Vector3(_playerController.transform.position.x - 10, _playerController.transform.position.y + 10);
        _eyalController.Movement(false);

        yield return new WaitForSeconds(5);

        _cinemaCamera.Follow = _playerController.transform;
        _cinemaCamera.m_Lens.FieldOfView = 40;
        _playerController._sePuedeMover = true;
    }

    protected override void DoTriggerEvent()
    {
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
        ManagerLocator.GetDialogsManager().OnFinishDialog = null;
        ManagerLocator.GetDialogsManager().OnFinishDialog += EhyalShowSign;
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
