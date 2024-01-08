using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogsManager : MonoBehaviour
{
    public string _text;
    public Dialog _dialogPrefab;
    [SerializeField] private Dialog _dialogInstanced;
    public List<string> _queueDialogsList = new();
    public Transform _parent;
    private float _typingSpeed = 0.05f;
    private string _currentDialog;
    private int _dialoglettersIndex;
    [SerializeField] private int _dialogIndex;
    public event Action OnNextAction;
    public Color _mainDialogColor, _secondDialogColor, _thirdDialogColor;

    public enum ENUM_CharTypeDialogs
    {
        mainChar,
        secondChar,
        thirdChar
    }

    private void Start()
    {
        _dialogIndex = 0;
        _text = _queueDialogsList[_dialogIndex];
    }

    public void setDialog()
    {
        if(_dialogInstanced == null)
        {
            _dialogInstanced = Instantiate(_dialogPrefab, _parent);
            _dialogInstanced.OnNextDialog += HandleNextDialog;
        }

        _currentDialog = _queueDialogsList[_dialogIndex];
        _dialoglettersIndex = 0;
        StartCoroutine(TypeText());
    }

    IEnumerator TypeText()
    {
        _text = ""; // Limpiar el texto antes de empezar
        _dialogInstanced.ShowDialog();

        while (_dialoglettersIndex < _currentDialog.Length)
        {
            _text += _currentDialog[_dialoglettersIndex];
            _dialogInstanced.SetDialogText(_text);
            _dialoglettersIndex++;
            yield return new WaitForSeconds(_typingSpeed);
        }

        if(_dialoglettersIndex == _currentDialog.Length)
        {
            _dialogInstanced._nextButton.gameObject.SetActive(true);
        }
    }

    private void HandleNextDialog()
    {
        if((_dialogIndex + 1) >= _queueDialogsList.Count)
        {
            _dialogInstanced.HideDialog();
            NextAction();
        }
        if(_queueDialogsList.Count > 1 && (_dialogIndex + 1) < _queueDialogsList.Count)
        {
            _dialogIndex++;
            _text = _queueDialogsList[_dialogIndex];
            setDialog();
        }
    }

    public void ChangeCharacterDialogs(ENUM_CharTypeDialogs type)
    {
        _queueDialogsList.Clear();

        switch (type)
        {
            case ENUM_CharTypeDialogs.mainChar:
                _dialogPrefab._text.color = _mainDialogColor;
                break;
            case ENUM_CharTypeDialogs.secondChar:
                _dialogPrefab._text.color = _secondDialogColor;
                break;
            case ENUM_CharTypeDialogs.thirdChar:
                _dialogPrefab._text.color = _thirdDialogColor;
                break;
        }

    }

    private void NextAction()
    {
        if (OnNextAction != null)
        {
            OnNextAction.Invoke();
        }
    }

    private void OnDestroy()
    {
        if (_dialogInstanced != null)
        {
            _dialogInstanced.OnNextDialog -= HandleNextDialog;

        }
    }
}
