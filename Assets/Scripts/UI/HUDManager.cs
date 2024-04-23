using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public Image _faithBar;
    public Image _healthBar;

    private void Start()
    {
        _faithBar.fillAmount = FindObjectOfType<PlayerController>()._faithMaxAmount;
    }

    public float GetFaithFillAmount()
    {
        return _faithBar.fillAmount;
    }

    public void SetFaithAmount(float amount)
    {
        _faithBar.fillAmount = amount;
    }

    public void ChargeFaithAmount(bool charging)
    {
        float amountToCharge = ReEscale.Normalize(10, 0, 100, 0, 1);
        while (charging)
        {
            _faithBar.fillAmount += amountToCharge;
        }
    }
}
