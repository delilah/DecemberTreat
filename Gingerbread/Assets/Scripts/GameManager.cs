using UnityEngine;
using UnityEngine.InputSystem;
using System.IO;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public enum GameState { MainMenu, Game };

    public GameState gameState;

    [SerializeField] private GameObject _mainMenuPanel;
    [SerializeField] private GameObject _pedestalCharacter;
    [SerializeField] private GameObject _backToMenuButton;

    // List my input actions
    private PlayerInput _playerInput;
    private InputAction _backToMenuAction;
    private InputAction _exitGameAction;

    private GameObject _worldPrefab;
    private GameObject _worldInstance;
    private AssetBundle myLoadedAssetBundle;
    private Coroutine _loadAssetBundleCoroutine;



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

        //ChangeState(gameState);
       // ChangeState(GameState.MainMenu);
        StartGame();
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
            _pedestalCharacter.SetActive(true);
            _backToMenuButton.SetActive(false);

            if (_worldInstance != null)
            {
                Destroy(_worldInstance);
                _worldInstance = null;
            }

            if (myLoadedAssetBundle != null)
            {
                myLoadedAssetBundle.Unload(false);
                myLoadedAssetBundle = null;
            }

            if (_loadAssetBundleCoroutine != null)
            {
                StopCoroutine(_loadAssetBundleCoroutine);
                _loadAssetBundleCoroutine = null;
            }
        }

        if (state == GameState.Game)
        {
            // Currently not managing Pause/Start New Game, just switching states
 _loadAssetBundleCoroutine = StartCoroutine(LoadAssetBundleCoroutine());

            _mainMenuPanel.SetActive(false);
            _pedestalCharacter.SetActive(false);

            _backToMenuButton.SetActive(true);

           

        }
    }


    /// <summary>
    /// This is used to load the assetbundle
    /// As it's a proof of concept, only the world prefab is being loaded/unloaded
    /// </summary>
    /// <returns></returns>
    private IEnumerator LoadAssetBundleCoroutine()
    {
        string bundlePath = Path.Combine(Application.streamingAssetsPath, "worldassetbundle");
        AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(bundlePath);

        yield return request;

        myLoadedAssetBundle = request.assetBundle;

        if (myLoadedAssetBundle == null)
        {
            Debug.Log("Failed to load AssetBundle!");
            yield break;
        }

        AssetBundleRequest prefabRequest = myLoadedAssetBundle.LoadAssetAsync<GameObject>("World");

        yield return prefabRequest;

        _worldPrefab = prefabRequest.asset as GameObject;

        if (_worldPrefab == null)
        {
            Debug.Log("Failed to load the World!");
            yield break;
        }

        _worldInstance = Instantiate(_worldPrefab);
    }
}