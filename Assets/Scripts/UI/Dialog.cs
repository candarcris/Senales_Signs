using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dialog : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
    }
    public void ShowDialog()
    {
        this.gameObject.SetActive(true);
    }

    public void HideDialog()
    {
        this.gameObject.SetActive(false);
    }

    public void SetDialogText(string dialog)
    {
        _text.SetText(dialog);
    }

    public void NextDialog()
    {

    }
}
