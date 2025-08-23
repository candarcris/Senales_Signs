using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerLanding : TriggerEvent
{
    GameManager gameManager;
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public PlayerController _playerController;
    public CinemachineVirtualCamera _cinemaCamera;
    [SerializeField] private Transform _platformTarget;
    public FallingPlatform _enemyPlatform;
    public BoxCollider _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        gameManager = ManagerLocator.GetGameManager();
    }

    void SagarContinueWalking()
    {
        _cinemaCamera.Follow = _playerController.transform;
        _playerController._sePuedeMover = true;
        gameObject.SetActive(false);
    }

    protected override void DoTriggerEvent()
    {
        _playerController.StopState();
        _enemyPlatform.Fall();
        _cinemaCamera.Follow = _platformTarget;
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
        ManagerLocator.GetDialogsManager().OnFinishDialog = null;
        ManagerLocator.GetDialogsManager().OnFinishDialog += SagarContinueWalking;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }
}
