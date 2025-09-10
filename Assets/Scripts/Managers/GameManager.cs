using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public enum ENUM_GameState
{
    menu,
    inGame,
    gameOver
}

public enum Context
{
    Player,
    UI,
    Ehyal
}

public class GameManager : MonoBehaviour
{
    public static GameManager _sharedInstance;// singleton
    public ENUM_GameState _currentGameState = ENUM_GameState.menu;
    public List<GameObject> _uiContentsList = new();

    public Context _context;

    // Referencia al InputActions
    private InputActions _inputActions;

    // Propiedad con lazy initialization
    public InputActions InputActions
    {
        get
        {
            if (_inputActions == null)
            {
                _inputActions = new InputActions();
            }
            return _inputActions;
        }
    }

    private void Awake()
    {
        if(_sharedInstance == null)
        {
            _sharedInstance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else if (_sharedInstance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartGame();
    }

    /// <summary>
    /// Metodo para iniciar el juego
    /// </summary>
    public void StartGame()
    {
        SetContext(Context.Player);
        SetGamState(ENUM_GameState.inGame);
    }

    /// <summary>
    /// Metodo que acctiva el fin de la partida
    /// </summary>
    public void GameOver()
    {
        UIController _uiController = FindObjectOfType<UIController>();
        SetGamState(ENUM_GameState.gameOver);
        _uiController.InitElementBase("gameOver", _uiController._UIWindowsParentTransform);
        _uiController.GetUIElementWindow("gameOver").GetComponent<GameOverPanelUI>().FadeIn();
    }

    public void LoadScene(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// metodo que vuelve al menu
    /// </summary>
    public void BackToMenu()
    {
        SetGamState(ENUM_GameState.menu);
    }

    private void SetGamState(ENUM_GameState newGameState)
    {
        if(newGameState == ENUM_GameState.menu)
        {
            //TODO: COLOCAR LOGICA DE MENU
        }
        else if (newGameState == ENUM_GameState.inGame)
        {
            //TODO: COLOCAR LOGICA DE IN GAME
        }
        else if (newGameState == ENUM_GameState.gameOver)
        {
            //TODO: COLOCAR LOGICA DE GAME OVER
        }

        this._currentGameState = newGameState;
    }

    /// <summary>
    /// Método para cambiar el contexto de input activo
    /// </summary>
    /// <param name="context">El contexto a activar (Player, UI, o Ehyal)</param>
    public void SetContext(Context context)
    {
        // Usar la propiedad que maneja la inicialización automáticamente
        var inputActions = InputActions;

        // Deshabilitar todos los Action Maps
        foreach (var action in inputActions)
        {
            action.Disable();
        }

        // Habilitar solo el Action Map especificado
        switch (context)
        {
            case Context.Player:
                inputActions.PlayerControl.Enable();
                _context = Context.Player;
                break;
            case Context.UI:
                inputActions.UI.Enable();
                _context = Context.UI;
                break;
            case Context.Ehyal:
                _context = Context.Ehyal;
                break;
        }
    }

    /// <summary>
    /// Método para obtener la referencia a InputActions
    /// </summary>
    /// <returns>La instancia de InputActions</returns>
    public InputActions GetInputActions()
    {
        return InputActions;
    }

    private void OnDestroy()
    {
        if (_sharedInstance == this)
        {
            _inputActions?.Dispose();
        }
    }
}
