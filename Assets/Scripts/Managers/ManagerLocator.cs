using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerLocator : MonoBehaviour
{
    // Declaraci�n de las instancias de tus administradores
    public GameManager _gameManager;
    public CamerasManager _camerasManager;
    public SoundManager _soundManager;
    public DialogsManager _dialogsManager;
    public HUDManager _hudManager;

    // Singleton para acceder a esta clase desde cualquier lugar
    private static ManagerLocator instance;

    private void Awake()
    {
        // Aseg�rate de que solo haya una instancia de ManagerLocator en la escena
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // M�todos para obtener referencias a los administradores
    public static GameManager GetGameManager()
    {
        return instance._gameManager;
    }

    public static CamerasManager GetCamerasManager()
    {
        return instance._camerasManager;
    }

    public static SoundManager GetSoundManager()
    {
        return instance._soundManager;
    }

    public static DialogsManager GetDialogsManager()
    {
        return instance._dialogsManager;
    }

    public static HUDManager GetHUDManager()
    {
        return instance._hudManager;
    }
}
