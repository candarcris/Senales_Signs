using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanelUI : UIElement
{
    Animator _anim;
    public Button _continuarButton, _exitButton;

    private void Awake()
    {
        _anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        _continuarButton.gameObject.SetActive(false);
        _exitButton.gameObject.SetActive(false);
    }

    public void FadeIn()
    {
        _anim.SetTrigger("Dead");
        StartCoroutine(ButtonAppear());
    }

    public void Continuar()
    {
        GameManager._sharedInstance.LoadScene(0);
    }

    public IEnumerator ButtonAppear()
    {
        yield return new WaitForSeconds(1);
        _continuarButton.gameObject.SetActive(true);
        _exitButton.gameObject.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
        Debug.Log("Bye bye");
    }
}
