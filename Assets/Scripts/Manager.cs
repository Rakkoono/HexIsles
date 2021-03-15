using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : SingletonMonoBehaviour<Manager>
{
    // Serialized fields
    [SerializeField] private Config config;

    [Space(2), Header("Audio")]

    [SerializeField] private AudioSource musicSource;
    public AudioSource MusicSource => musicSource;

    [SerializeField] private AudioSource sfxSource;
    public AudioSource SfxSource => sfxSource;

    // Hidden fields
    private bool onStartup = true;
    
    private MainInput input;

    private int turnsLeft = 0;
    public int TurnsLeft
    {
        get => turnsLeft;
        set
        {
            turnsLeft = value;
            UI.turnDisplay.text = turnsLeft + (turnsLeft == 1 ? " Turn" : " Turns");
        }
    }

    [HideInInspector] public CameraController cameraController;

    public static UIHandler UI { get; private set; }
    public static DialogHandler Dialogs { get; private set; }
    public static PlayerHandler Players { get; private set; }

    [HideInInspector] public Level level;
    [HideInInspector] public int levelIndex;
    [HideInInspector] public int completedLevels = 0;

    void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        // if not on mobile, deactivate mobile only objects
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MobileOnly"))
            obj.SetActive(false);

#endif
        // start playing music
        musicSource.clip = Config.Current.Music;
        musicSource.Play();

        // Keep manager loaded during scene change, execute OnLoadCallback instead;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLoadCallback;

        cameraController = GetComponentInChildren<CameraController>();

        // Get Handlers
        UI = GetComponent<UIHandler>();
        Dialogs = GetComponent<DialogHandler>();
        Players = GetComponent<PlayerHandler>();

        InitializeInputActions();
        // Initialize UI and dialogs
        UI.Initialize();

        // Load saved data
        if (PlayerPrefs.HasKey("completedLevels"))
            completedLevels = PlayerPrefs.GetInt("completedLevels");

        // Load next or latest level
        if (completedLevels >= Config.Current.Levels.Length)
            LoadNextLevel();
        else
            LoadLatestLevel();
    }

    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        // Save current build index
        levelIndex = scene.buildIndex;
        if (levelIndex > 0)
        {
            level = Config.Current.Levels[levelIndex - 1];
            TurnsLeft = level.Turns;
        }

        // Reset players and undo list
        Players.players = FindObjectsOfType<Player>();
        foreach (var player in Players.players)
            player.position = GridUtility.WorldToGridPos(player.transform.position);
        Players.undoStack = new Stack<PlayerState[]>();

        // Open main menu on startup
        if (levelIndex > 0 && onStartup)
        {
            UI.ShowMainMenu();
            onStartup = false;
        }
        else
            UI.ExitMenus();
            
    }

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    private void InitializeInputActions()
    {
        input = new MainInput();

        input.Hotkeys.Menu.performed += _ => Manager.UI.ToggleMainMenu();
        input.Hotkeys.Restart.performed += _ => LoadCurrentLevel();
        input.Hotkeys.Undo.performed += _ => Manager.Players.Undo();
        
        // Cheat code only in debug builds
        if (Debug.isDebugBuild)
            input.Hotkeys.UnlockAllLevels.performed += _ => UnlockAllLevels();

        input.Camera.Zoom.performed += ctx => cameraController.zoomAmount = ctx.ReadValue<float>();
        input.Camera.Zoom.canceled += _ => cameraController.zoomAmount = 0f;

        input.Camera.Pan.performed += ctx => cameraController.panAmount = ctx.ReadValue<float>();
        input.Camera.Pan.canceled += _ => cameraController.panAmount = 0f;

        input.Camera.Pinch.started += _ => cameraController.startPinch = true;
        input.Camera.Pinch.canceled += _ => cameraController.pinch = false;
    }

    public void OnPressContinue()
    {
        if (Manager.UI.inEscapeMenu)
            Manager.UI.ExitMenus();
        else if (completedLevels >= Config.Current.Levels.Length)
            RestartGame();
        else
            LoadLatestLevel();
    }

    //
    // Level Loading
    //

    public void UnlockAllLevels()
    {
            completedLevels = Config.Current.Levels.Length;
            if (Manager.UI.currentMenu != UIHandler.Menu.LevelSelect)
                Manager.UI.ShowLevelSelect();
    }

    public void LoadLevel(int index)
    {
        if (index <= Config.Current.Levels.Length)
            SceneManager.LoadScene(index);
        else
            Manager.UI.ShowCredits();
    }

    public void LoadCurrentLevel() => LoadLevel(levelIndex);
    public void LoadNextLevel() => LoadLevel(levelIndex + 1);
    public void LoadLatestLevel() => LoadLevel(completedLevels + 1);

    public void RestartGame()
    {
        completedLevels = 0;
        LoadLatestLevel();
    }
    public void QuitGame() => Application.Quit();

    private void OnApplicationQuit()
    {
        // Save data on quit
        PlayerPrefs.SetInt("completedLevels", completedLevels);
        PlayerPrefs.Save();
    }
}