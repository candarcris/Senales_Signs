using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public enum ENUM_CharTypeDialogs
{
    mainChar,
    secondChar,
    thirdChar
}
public class DialogPanelUI : MonoBehaviour
{
    public Button _nextButton;
    public Color _mainDialogColor, _secondDialogColor, _thirdDialogColor;
    public TextMeshProUGUI _screenText;
    public ENUM_CharTypeDialogs charType;
}
