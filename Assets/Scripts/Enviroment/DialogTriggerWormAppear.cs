using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerWormAppear : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    public PlayerController _playerController;
    public CinemachineVirtualCamera _cinemaCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    public Transform _wormPosition;
    public WormBossEnemy _worm;
    private BoxCollider _boxCollider;
    public GameObject _trigerExitDream;
    private bool _isWormAppearing = false;

    void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        noise = _cinemaCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void StartInfiniteRoar(float amplitude, float frequency)
    {
        noise.m_AmplitudeGain = amplitude;
        noise.m_FrequencyGain = frequency;
    }

    public void StopRoar()
    {
        noise.m_AmplitudeGain = 0f;
        noise.m_FrequencyGain = 0f;
    }

    void Continue()
    {
        _playerController._sePuedeMover = true;
        _boxCollider.enabled = false;
        _isWormAppearing = false;
        _worm._moveSpeed = 0.3f;
        _worm._isWormFollowing = true;
        StopRoar();
    }

    protected override void DoTriggerEvent()
    {
        _playerController.StopState();
        StartInfiniteRoar(2f, 1.5f);
        _isWormAppearing = true;
        _trigerExitDream.SetActive(true);
        ManagerLocator.GetDialogsManager().OnFinishDialog = null;
        ManagerLocator.GetDialogsManager().OnFinishDialog += Continue;
        ManagerLocator.GetDialogsManager().DoDialog(_type, _dialogs);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            DoTriggerEvent();
        }
    }

    private void FixedUpdate()
    {
        if(_isWormAppearing)
        {
            _worm.transform.position = Vector3.Lerp(
                _worm.transform.position,
                _wormPosition.position,
                3f * Time.deltaTime
            );
        }

        if (_worm._isWormFollowing)
        {
            Vector3 targetPosition = _playerController.transform.position + Vector3.left * 4f;
            _worm.transform.position = Vector3.Lerp(_worm.transform.position, targetPosition, Time.deltaTime * _worm._moveSpeed);
        }
    }
}
