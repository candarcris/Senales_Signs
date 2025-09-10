using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTriggerFortressKing : TriggerEvent
{
    public Dialogs _dialogs;
    public ENUM_CharTypeDialogs _type;
    private BoxCollider _boxCollider;
    public CinemachineVirtualCamera _cinemaCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    public EyalController _ehyalController;
    public PlayerController _playerController;
    public GameObject _fortressKing;
    public GameObject _fadeOff;

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
        _playerController.DeactiveInputs();
        _boxCollider.enabled = false;
        _fadeOff.SetActive(true);
    }

    protected override void DoTriggerEvent()
    {
        StopRoar();
        _fortressKing.SetActive(true);
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
