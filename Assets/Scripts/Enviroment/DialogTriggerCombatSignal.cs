using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerCombatSignal : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public BoxCollider _boxCollider;
    public CinemachineVirtualCamera _cinemaCamera;
    public EyalController _eyalController;
    public PlayerController _playerController;
    public Transform _signalCombat;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    public void EhyalShowSign()
    {
        StartCoroutine(WaitForEhyalShowSign());
    }

    public IEnumerator WaitForEhyalShowSign()
    {
        _playerController.StopState();

        _cinemaCamera.Follow = _signalCombat;
        _cinemaCamera.m_Lens.FieldOfView = 30;

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
