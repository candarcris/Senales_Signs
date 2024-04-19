using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class button : MonoBehaviour
{
    public Button _boton;
    public void Start()
    {
        _boton.onClick.AddListener(FuncionBoton);
    }
    public void FuncionBoton()
    {
        gameObject.SetActive(false);
    }
    
}
