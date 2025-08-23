using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalCombat : AparicibleObject
{
    public CapsuleCollider _signalCollider;
    public UIInstructiveCombat _uiCombat;

    public override void UpdateOpacityBasedOnDistance(Transform transform, Vector3 offset, float multiplyValue)
    {
        base.UpdateOpacityBasedOnDistance(_ehyalTransform, Vector3.up, -2);
        if (_evaluationDistance <= _closeDistance || _distance <= _closeDistance)
        {
            _signalCollider.enabled = true;

        }
        else if (_evaluationDistance >= _farDistance)
        {
            _signalCollider.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            _uiCombat.SetInstruction(true);
            other.TryGetComponent<PlayerController>(out PlayerController component);
            component._canCombat = true;
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
