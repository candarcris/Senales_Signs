using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SignalPray : AparicibleObject
{
    public HUDManager _hudManager;
    public UIInstructivePray _uiPray;

    public override void UpdateOpacityBasedOnDistance(Transform transform, Vector3 offset, float multiplyValue)
    {
        base.UpdateOpacityBasedOnDistance(_ehyalTransform, Vector3.up, -2);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _uiPray.SetInstruction(true);
            other.TryGetComponent<PlayerController>(out PlayerController component);
            component._canPray = true;

            if (_hudManager != null)
            {
                //_hudManager.ChargeFaithAmount(0.5f);
                _hudManager.FillBall(1, 1);
            }

            this.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (_ehyalTransform != null)
        {
            UpdateOpacityBasedOnDistance(_ehyalTransform, Vector3.up, -2);
            UpdateOpacity();
        }
    }
}
