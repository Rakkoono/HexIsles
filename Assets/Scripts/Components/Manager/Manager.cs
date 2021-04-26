using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : SingletonMonoBehaviour<Manager>
{
    #region Serialized fields

    // Config file
    [SerializeField] private Config config;

    #region Audio
    [Space(2), Header("Audio")]

    [SerializeField] private AudioSource musicSource;
    public static AudioSource MusicSource => Instance.musicSource;

    [SerializeField] private AudioSource sfxSource;
    public static AudioSource SfxSource => Instance.sfxSource;
    #endregion

    #region UI
    [Space(2), Header("UI")]
    public TMP_Text turnDisplay;

    [Space(2)]

    [SerializeField] private Menu overlay;
    public static Menu Overlay => Instance.overlay;

    [SerializeField] private GameOverMenu gameOverMenu;
    public static GameOverMenu GameOverMenu => Instance.gameOverMenu;

    [SerializeField] private MainMenu mainMenu;
    public static MainMenu MainMenu => Instance.mainMenu;

    [SerializeField] private LevelSelectMenu levelSelectMenu;
    public static LevelSelectMenu LevelSelectMenu => Instance.levelSelectMenu;

    [SerializeField] private Menu credits;
    public static Menu Credits => Instance.credits;

    [SerializeField] private Menu optionsMenu;
    public static Menu OptionsMenu => Instance.optionsMenu;

    [SerializeField] private DialogBox dialogBox;
    public static DialogBox DialogBox => Instance.dialogBox;
    #endregion
    #endregion

    #region Hidden fields
    private bool onStartup = true;

    private MainInput input;
    private CameraController cameraController;

    private Flag[] flags;
    public static bool AllFlagsAreReached => Instance.flags.All(flag => flag.IsReached);

    #region Undo
    private Stack<PlayerData[]> undoStack = new Stack<PlayerData[]>();
    public Stack<PlayerData[]> UndoStack => undoStack;
    #endregion

    [HideInInspector] public MouseSelectable[] validTargets;

    #region Levels
    public LevelData Level { get; private set; }
    public int LevelIndex { get; private set; }
    public int CompletedLevels { get; private set; }
    #endregion

    #region Settings
    public bool ReselectAfterMove { get; private set; }
    #endregion

    #region Players & selected objects
    public Player[] Players { get; private set; }
    public Player SelectedPlayer { get; private set; }
    [HideInInspector] public Player lastSelectedPlayer;

    private MouseSelectable selectedObject;
    public MouseSelectable SelectedObject
    {
        get => selectedObject;
        set
        {
            // Deselect last object
            if (selectedObject)
            {
                selectedObject.ResetColor();
                selectedObject.OnDeselect();
            }

            // Update last selected player and selected object
            lastSelectedPlayer = SelectedPlayer;
            selectedObject = value;

            // Select new object
            selectedObject?.OnSelect();
            if (selectedObject)
            {
                selectedObject.Renderer.material.color = selectedObject.Color + Config.SelectionTint;
                SelectedPlayer = selectedObject.GetComponent<Player>();
            }
            else
                SelectedPlayer = null;
        }
    }
    #endregion

    private int turnsLeft = 0;
    public int TurnsLeft
    {
        get => turnsLeft;
        set
        {
            turnsLeft = value;
            turnDisplay.text = turnsLeft + (turnsLeft == 1 ? " Turn" : " Turns");
        }
    }

    #endregion

    #region Initialization
    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        // if not on mobile, deactivate mobile only objects
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MobileOnly"))
            obj.SetActive(false);
#endif

        // start playing music
        musicSource.clip = Config.Music;
        musicSource.Play();

        // Keep manager loaded during scene change, execute OnLoadCallback instead;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLoadCallback;

        cameraController = GetComponentInChildren<CameraController>();

        //Initialize input actions
        InitializeInputActions();

        // Load saved data
        if (PlayerPrefs.HasKey("completedLevels"))
            CompletedLevels = PlayerPrefs.GetInt("completedLevels");

        // Load next or latest level
        if (CompletedLevels >= Config.Levels.Length)
            LoadNextLevel();
        else
            LoadLatestLevel();
    }

    private void InitializeInputActions()
    {
        input = new MainInput();

        input.Hotkeys.Menu.performed += _ => MainMenu.Toggle(true);
        input.Hotkeys.Restart.performed += _ => LoadCurrentLevel();
        input.Hotkeys.Undo.performed += _ => UndoMove();

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

    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        // Save current build index
        LevelIndex = scene.buildIndex;
        if (LevelIndex > 0)
        {
            Level = Config.Levels[LevelIndex - 1];
            TurnsLeft = Level.Turns;
        }

        GridUtility.FindOrCreateMap();

        // Reset players, flags and undo list
        flags = FindObjectsOfType<Flag>();
        Players = FindObjectsOfType<Player>();
        foreach (var player in Players)
            player.position = GridUtility.WorldToGridPos(player.transform.position);
        undoStack = new Stack<PlayerData[]>();

        // Open main menu on startup
        if (LevelIndex > 0)
        {
            if (onStartup)
            {
                MainMenu.Open();
                onStartup = false;
            }
            else
                Overlay.Open();
        }
    }
    #endregion

    #region Levels & Level Loading
    public void UnlockNextLevel() => CompletedLevels++;

    public void UnlockAllLevels()
    {
        CompletedLevels = Config.Levels.Length;
        levelSelectMenu.Open();
    }

    public void LoadLevel(int index)
    {
        if (index <= Config.Levels.Length)
            SceneManager.LoadScene(index);
        else
            credits.Open();
    }

    public void LoadCurrentLevel() => LoadLevel(LevelIndex);
    public void LoadNextLevel() => LoadLevel(LevelIndex + 1);
    public void LoadLatestLevel() => LoadLevel(CompletedLevels + 1);

    public void RestartGame()
    {
        CompletedLevels = 0;
        LoadLatestLevel();
    }
    public void QuitGame() => Application.Quit();

    private void OnApplicationQuit()
    {
        // Save data on quit
        PlayerPrefs.SetInt("completedLevels", CompletedLevels);
        PlayerPrefs.Save();
    }
    #endregion

    #region Undo
    public void UndoMove()
    {
        if (Menu.Current is GameOverMenu)
            Menu.Current.Close();
        else if (Menu.Current.StopGameplay)
            return;

        if (undoStack.Count == 0)
            return;

        PlayerData[] undo = undoStack.Pop();
        if (undo == default(PlayerData[]))
            return;
        TurnsLeft++;

        SelectedObject = null;

        PlayMoveSound();

        foreach (var playerData in undo)
            playerData.Apply();
    }
    #endregion

    #region Settings
    // TODO Reference to Audio mixer groups
    public void SetSFXVolume(int value) => SfxSource.volume = value / 100;

    public void SetMusicVolume(int value) => MusicSource.volume = value / 100;

    public void SetQualityLevel(int value) => QualitySettings.SetQualityLevel(value);

    public void SetReselectAfterMove(bool value) => ReselectAfterMove = value;
    #endregion

    #region Audio
    public void PlayMenuSound() => SfxSource.PlayOneShot(Config.MenuSounds.RandomElement());

    public void PlayMoveSound() => SfxSource.PlayOneShot(Config.MoveSounds.RandomElement());
    #endregion
}