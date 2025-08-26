using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class DialogTriggerExitDream : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    private BoxCollider _boxCollider;
    public CinemachineVirtualCamera _cinemaCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    public EyalController _ehyalController;
    public PlayerController _playerController;
    public WormBossEnemy _wormBoss;

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
        _boxCollider.enabled = false;
    }

    protected override void DoTriggerEvent()
    {
        StartInfiniteRoar(2, 2);
        _playerController._canPray = false;
        _playerController._canCombat = false;
        _wormBoss._isWormFollowing = false;
        _ehyalController.Movement(true);
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
}
