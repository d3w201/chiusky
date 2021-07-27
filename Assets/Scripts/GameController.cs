using Doublsb.Dialog;
using InputSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    private GameStatus _gameStatus;
    private InputsHandler _inputHandler;
    private GameObject _chiusky;
    private GameObject _pauseUI;
    private GameObject _dialog;
    private DialogManager _dialogManager;

    private void Awake()
    {
        if (!_pauseUI)
        {
            _pauseUI = GameObject.FindGameObjectWithTag("PauseUI");
        }

        if (!_chiusky)
        {
            _chiusky = GameObject.FindGameObjectWithTag("Chiusky");
        }

        if (!_dialog)
        {
            _dialog = GameObject.FindWithTag("DialogAsset");
        }
    }

    private void Start()
    {
        _inputHandler = _chiusky.GetComponent<InputsHandler>();
        _dialogManager = _dialog.GetComponent<DialogManager>();
        _dialogManager.Hide();
        Resume();
    }

    public void HandlePause(InputValue value)
    {
        if (_gameStatus.Equals(GameStatus.pause))
        {
            Resume();
        }
        else
        {
            Pause();
        }
    }

    private void Pause()
    {
        _pauseUI.SetActive(true);
        Time.timeScale = 0f;
        SetStatus(GameStatus.pause);
    }

    private void Resume()
    {
        _pauseUI.SetActive(false);
        Time.timeScale = 1f;
        SetStatus(GameStatus.play);
    }

    public void SetStatus(GameStatus status)
    {
        _gameStatus = status;
    }
    
    public GameStatus GetStatus()
    {
        return _gameStatus;
    }
}

public enum GameStatus
{
    pause,
    play,
    interact
}