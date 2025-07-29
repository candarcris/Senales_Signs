using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button _botonEmpezarPartida;
    public Button _botonOpciones;
    public Button _botonAtrasToMenu;

    public Animator _carruselAnimator;

    private void Start()
    {
        _botonEmpezarPartida.onClick.AddListener(EmpezarPartida);
        _botonOpciones.onClick.AddListener(Opciones);
        _botonAtrasToMenu.onClick.AddListener(MainMenu);
    }

    public void EmpezarPartida()
    {
        SceneManager.LoadScene(1);
    }

    public void Opciones()
    {
        _carruselAnimator.SetTrigger("ToOptions");
    }

    public void MainMenu()
    {
        _carruselAnimator.SetTrigger("ToMain");
    }
}
