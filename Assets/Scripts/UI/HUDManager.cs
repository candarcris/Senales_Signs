using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum FaithBallState
{
    Empty = 0,      // _emptyFaithBall
    MidFull = 1,    // _midFullFaithBall
    Full = 2        // _fullFaithBall
}

public class HUDManager : MonoBehaviour
{
    public Image _faithBarAmount;
    public Image _faithBall1, _faithBall2, _faithBall3, _faithBall4;
    public Sprite _emptyFaithBall, _midFullFaithBall, _fullFaithBall;
    public GameObject _faithBar2;
    public PlayerController _playerController;

    private void Start()
    {
        _faithBar2.SetActive(false);
    }

    public float GetFaithFillAmount()
    {
        return _faithBarAmount.fillAmount;
    }

    public void SetFaithAmount(float amount)
    {
        _faithBarAmount.fillAmount = amount;
    }

    public void ChargeFaithAmount(float amountToAdd)
    {
        float currentAmount = _faithBarAmount.fillAmount;
        float newAmount = Mathf.Clamp01(currentAmount + amountToAdd);
        _faithBarAmount.fillAmount = newAmount;
    }

    // Método para llenar completamente la barra
    public void FillFaithBar()
    {
        _faithBarAmount.fillAmount = 1f;
    }

    // Método para vaciar completamente la barra
    public void EmptyFaithBar()
    {
        _faithBarAmount.fillAmount = 0f;
    }

    // Método para establecer un porcentaje específico
    public void SetFaithPercentage(float percentage)
    {
        _faithBarAmount.fillAmount = Mathf.Clamp01(percentage);
    }

    public void FillBall(int faithBall, FaithBallState state)
    {
        Image targetBall = GetFaithBall(faithBall);
        if (targetBall != null)
        {
            targetBall.sprite = GetSpriteForState(state);
        }
    }

    // Método sobrecargado para usar int
    public void FillBall(int faithBall, int value)
    {
        FaithBallState state = (FaithBallState)Mathf.Clamp(value, 0, 2);
        FillBall(faithBall, state);
    }

    private Image GetFaithBall(int faithBall)
    {
        switch (faithBall)
        {
            case 1: return _faithBall1;
            case 2: return _faithBall2;
            case 3: return _faithBall3;
            case 4: return _faithBall4;
            default:
                Debug.LogError($"Índice de bola de fe inválido: {faithBall}. Debe ser 1-4.");
                return null;
        }
    }

    private Sprite GetSpriteForState(FaithBallState state)
    {
        switch (state)
        {
            case FaithBallState.Empty: return _emptyFaithBall;
            case FaithBallState.MidFull: return _midFullFaithBall;
            case FaithBallState.Full: return _fullFaithBall;
            default: return _emptyFaithBall;
        }
    }
}
