using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum ENUM_GameState
{
    menu,
    inGame,
    gameOver
}
public class GameManager : MonoBehaviour
{
    UIController _uiController;
    public static GameManager _sharedInstance;// singleton
    public ENUM_GameState _currentGameState = ENUM_GameState.menu;
    public List<GameObject> _uiContentsList = new();

    private void Awake()
    {
        if(_sharedInstance == null)
        {
            _sharedInstance = this;
        }

        _uiController = this.GetComponent<UIController>();
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
        SetGamState(ENUM_GameState.inGame);
    }

    /// <summary>
    /// Metodo que acctiva el fin de la partida
    /// </summary>
    public void GameOver()
    {
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
}
