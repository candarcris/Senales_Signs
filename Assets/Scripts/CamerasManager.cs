using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamerasManager : MonoBehaviour
{
    public CinemachineVirtualCamera _cinemachineCam;

    private void Start()
    {
        _cinemachineCam.m_Lens.FieldOfView = 40f;
    }

    public void SetFieldOfView(float amount)
    {
        _cinemachineCam.m_Lens.FieldOfView = amount;
    }
}
