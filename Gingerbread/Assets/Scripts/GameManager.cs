using UnityEngine;
using UnityEngine.InputSystem;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { MainMenu, Game };

    public GameState gameState;

    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _backToMenuButton;

    // List my input actions
    private PlayerInput _playerInput;
    private InputAction _backToMenuAction;
    private InputAction _exitGameAction;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogError("Multiple instances of GameManager detected. Oh no, this should not happen!");
            Destroy(this.gameObject);
        }
    }

    public void Start()
    {
        _playerInput = GetComponent<PlayerInput>();
        _backToMenuAction = _playerInput.actions["BackToMenu"];
        _exitGameAction = _playerInput.actions["ExitGame"];

        ChangeState(gameState);
    }

    public void Update()
    {
        // go back to menu by pressing M, debug
        if (gameState == GameState.Game && _backToMenuAction.triggered)
        {
            GoToMainMenu();
        }

        // exit by pressing esc, debug
        if (_exitGameAction.triggered)
        {
            ApplicationQuit();
        }
    }

    /// <summary>
    /// Used to quit the application (Esc)
    /// </summary>
    void ApplicationQuit()
    {
        // Quit the application
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// These are functions linked to buttons. Kept them separate from what actually happens when the states change.
    /// </summary>
    public void StartGame()
    {
        ChangeState(GameState.Game);
    }

    public void GoToMainMenu()
    {
        ChangeState(GameState.MainMenu);
    }

    /// <summary>
    /// ChangeState: takes care of what happens when the state changes
    /// </summary>
    /// <param name="state"></param>
    public void ChangeState(GameState state)
    {
        gameState = state;

        if (state == GameState.MainMenu)
        {
            _mainMenuPanel.SetActive(true);
            _backToMenuButton.SetActive(false);
        }

        if (state == GameState.Game)
        {
            // Currently not managing Pause/Start New Game, just switching states
            _mainMenuPanel.SetActive(false);
            _backToMenuButton.SetActive(true);
        }
    }

}

