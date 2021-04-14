using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Menu { None, GameOver, MainMenu, LevelSelect, Credits, Options }

public class Manager : SingletonMonoBehaviour<Manager>
{
    #region Serialized fields
    [SerializeField] private Config config;

    #region Audio
    [Space(2), Header("Audio")]

    [SerializeField] private AudioSource musicSource;
    public AudioSource MusicSource => musicSource;

    [SerializeField] private AudioSource sfxSource;
    public AudioSource SfxSource => sfxSource;
    #endregion

    #region UI
    [Space(2), Header("UI")]

    [SerializeField] private GameObject[] levelDisplays = new GameObject[9];
    public TMP_Text turnDisplay;

    [Space(2)]

    [SerializeField] private Transform overlay;
    [SerializeField] private Transform gameOverScreen;
    [SerializeField] private Transform mainMenuScreen;
    [SerializeField] private Transform creditsScreen;
    [SerializeField] private Transform levelSelectScreen;
    [SerializeField] private Transform thxForPlayingMessage;
    #endregion
    #endregion

    #region Hidden fields
    private bool onStartup = true;

    private MainInput input;
    private CameraController cameraController;

    public Flag[] Flags { get; private set; }

    #region Undo
    private Stack<PlayerState[]> undoStack = new Stack<PlayerState[]>();
    public Stack<PlayerState[]> UndoStack => undoStack;
    #endregion

    [HideInInspector] public MouseSelectable[] validTargets;

    #region UI
    private Animator overlayAnimator;
    private Animator gameOverAnimator;
    private Animator mainMenuAnimator;
    private Animator creditsAnimator;
    private Animator levelSelectAnimator;
    private Animator thxForPlayingAnimator;

    private Button gameOverNextButton;
    private TMP_Text gameOverMessage;

    private TMP_Text mainMenuContinueButtonLabel;
    private Button mainMenuResetButton;

    [HideInInspector] public Menu menu = Menu.None;
    [HideInInspector] public bool inEscapeMenu;
    public bool InMainOrSubMenu
        => menu == Menu.MainMenu
        || menu == Menu.Credits
        || menu == Menu.LevelSelect
        || menu == Menu.Options;

    private readonly int[] displayedLevels = new int[9];
    private Image[] levelDisplayImages = new Image[9];
    private TMP_Text[] levelDisplayTexts = new TMP_Text[9];
    private Button[] levelDisplayButtons = new Button[9];

    [HideInInspector] private int levelSelectPage;
    public int LevelSelectPage
    {
        get => levelSelectPage;
        set
        {
            levelSelectPage = value;
            for (int i = 0; i < levelDisplays.Length; i++)
            {
                displayedLevels[i] = levelSelectPage * levelDisplays.Length + i + 1;

                if (displayedLevels[i] > Config.Current.Levels.Length)
                    levelDisplays[i].SetActive(false);
                else
                {
                    levelDisplays[i].SetActive(true);
                    levelDisplayTexts[i].text = (Manager.Current.CompletedLevels >= displayedLevels[i] - 1) ? displayedLevels[i] + " " + Config.Current.Levels[displayedLevels[i] - 1].DisplayName : "???";
                    levelDisplayImages[i].sprite = (Manager.Current.CompletedLevels >= displayedLevels[i] - 1) ? Config.Current.Levels[displayedLevels[i] - 1].PreviewImage : null;
                    levelDisplayButtons[i].interactable = Manager.Current.CompletedLevels >= displayedLevels[i] - 1;
                }
            }
        }
    }

    public DialogBox DialogBox { get; private set; }
    #endregion

    #region Levels
    public Level Level { get; private set; }
    public int LevelIndex { get; private set; }
    public int CompletedLevels { get; private set; }
    #endregion

    #region Players & selected objects
    public Player[] Players { get; private set; }
    public Player SelectedPlayer { get; private set; }
    [HideInInspector] public Player LastSelectedPlayer;

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
            LastSelectedPlayer = SelectedPlayer;
            selectedObject = value;

            // Select new object
            selectedObject?.OnSelect();
            if (selectedObject)
            {
                selectedObject.Renderer.material.color = selectedObject.Color + Config.Current.SelectionTint;
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
        musicSource.clip = Config.Current.Music;
        musicSource.Play();

        // Keep manager loaded during scene change, execute OnLoadCallback instead;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnLoadCallback;

        cameraController = GetComponentInChildren<CameraController>();

        // Get dialog box
        DialogBox = GetComponentInChildren<DialogBox>();

        //Initialize input & UI
        InitializeInputActions();
        InitializeUI();

        // Load saved data
        if (PlayerPrefs.HasKey("completedLevels"))
            CompletedLevels = PlayerPrefs.GetInt("completedLevels");

        // Load next or latest level
        if (CompletedLevels >= Config.Current.Levels.Length)
            LoadNextLevel();
        else
            LoadLatestLevel();
    }

    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        // Save current build index
        LevelIndex = scene.buildIndex;
        if (LevelIndex > 0)
        {
            Level = Config.Current.Levels[LevelIndex - 1];
            TurnsLeft = Level.Turns;
        }

        GridUtility.FindOrCreateMap();

        // Reset players, flags and undo list
        Flags = FindObjectsOfType<Flag>();
        Players = FindObjectsOfType<Player>();
        foreach (var player in Players)
            player.position = GridUtility.WorldToGridPos(player.transform.position);
        undoStack = new Stack<PlayerState[]>();

        // Open main menu on startup
        if (LevelIndex > 0 && onStartup)
        {
            ShowMainMenu();
            onStartup = false;
        }
        else
            ExitMenus();
    }

    private void InitializeInputActions()
    {
        input = new MainInput();

        input.Hotkeys.Menu.performed += _ => ToggleMainMenu();
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

    private void InitializeUI()
    {
        for (int i = 0; i < 9; i++)
        {
            levelDisplayImages[i] = levelDisplays[i].transform.Find("PreviewImage").GetComponent<Image>();
            levelDisplayTexts[i] = levelDisplays[i].GetComponentInChildren<TMP_Text>();
            levelDisplayButtons[i] = levelDisplays[i].GetComponent<Button>();
        }

        overlayAnimator = overlay.GetComponent<Animator>();
        mainMenuAnimator = mainMenuScreen.GetComponent<Animator>();
        gameOverAnimator = gameOverScreen.GetComponent<Animator>();
        creditsAnimator = creditsScreen.GetComponent<Animator>();
        levelSelectAnimator = levelSelectScreen.GetComponent<Animator>();

        thxForPlayingAnimator = thxForPlayingMessage.GetComponent<Animator>();

        gameOverNextButton = gameOverScreen.Find("NextButton").GetComponent<Button>();
        gameOverMessage = gameOverScreen.Find("GameOverMessage").GetComponent<TMP_Text>();

        mainMenuContinueButtonLabel = mainMenuScreen.Find("ContinueButton").GetComponentInChildren<TMP_Text>();
        mainMenuResetButton = mainMenuScreen.Find("ResetButton").GetComponent<Button>();

        overlay.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);
        creditsScreen.gameObject.SetActive(false);
        levelSelectScreen.gameObject.SetActive(false);
        menu = Menu.None;
    }
    #endregion

    #region Levels & Level Loading
    public void UnlockNextLevel() => CompletedLevels++;

    public void OnPressContinue()
    {
        if (inEscapeMenu)
            ExitMenus();
        else if (CompletedLevels >= Config.Current.Levels.Length)
            RestartGame();
        else
            LoadLatestLevel();
    }

    public void UnlockAllLevels()
    {
        CompletedLevels = Config.Current.Levels.Length;
        if (menu != Menu.LevelSelect)
            ShowLevelSelect();
    }

    public void LoadLevel(int index)
    {
        if (index <= Config.Current.Levels.Length)
            SceneManager.LoadScene(index);
        else
            ShowCredits();
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

    #region Petrify
    public void PetrifyLonePlayers()
    {
        foreach (var player in Players)
        {
            if (GridUtility.GetPlayersAt(player.position, false).Length > 1) continue;
            bool playersNearby = false;
            foreach (var f in GridUtility.GetAdjacentFields(player.position))
                if (GridUtility.GetPlayersAt(f, false).Length > 0)
                {
                    playersNearby = true;
                    break;
                }
            if (playersNearby) continue;
            player.IsPetrified = true;
        }
    }

    #endregion

    #region Undo
    public void UndoMove()
    {
        if (menu == Menu.GameOver)
            ExitMenus();
        else if (menu != Menu.None)
            return;

        if (undoStack.Count == 0)
            return;

        PlayerState[] undo = undoStack.Pop();
        if (undo == default(PlayerState[]))
            return;
        Manager.Current.TurnsLeft++;

        SelectedObject = null;

        Manager.Current.SfxSource.PlayOneShot(Config.Current.MoveSounds[Random.Range(0, Config.Current.MoveSounds.Length)]);

        foreach (var playerState in undo)
        {
            var instance = playerState.Instance;

            instance.IsPetrified = playerState.IsPetrified;

            if (playerState.Position == instance.transform.position)
                continue;

            playerState.Instance.position = GridUtility.WorldToGridPos(playerState.Position);
            playerState.Instance.targetPosition = playerState.Position;
        }
    }
    #endregion

    #region UI

    public static void UIAnimateOut(Animator animator) => animator.Play("Out");

    public void ShowGameOver(GameOver gameOverType)
    {
        if (menu != Menu.None) return;

        Manager.Current.DialogBox.HideDialog();
        if (Manager.Current.SelectedObject && Manager.Current.SelectedObject.GetComponent<Player>())
            Manager.Current.SelectedObject.ToggleSelect();

        if (gameOverType.UnlockNextLevel && Manager.Current.CompletedLevels == Manager.Current.LevelIndex - 1)
            Manager.Current.UnlockNextLevel();

        gameOverNextButton.interactable = Manager.Current.CompletedLevels >= Manager.Current.LevelIndex;

        menu = Menu.GameOver;
        overlayAnimator.gameObject.SetActive(true);
        gameOverScreen.gameObject.SetActive(true);
        gameOverMessage.text = gameOverType.Messages[Random.Range(0, gameOverType.Messages.Length)];

        // Play sound effect
        Manager.Current.SfxSource.PlayOneShot(gameOverType.Sound, .7f);
    }

    public void ToggleMainMenu()
    {
        if (menu == Menu.None) ShowMainMenu(true);
        else if (inEscapeMenu) ExitMenus();
    }

    public void ShowMainMenu(bool byEsc)
    {
        inEscapeMenu = byEsc;

        switch (menu)
        {
            case Menu.None:
                if (overlayAnimator.gameObject.activeSelf)
                    UIAnimateOut(overlayAnimator);
                break;

            case Menu.LevelSelect:
                UIAnimateOut(levelSelectAnimator);
                break;

            case Menu.Credits:
                UIAnimateOut(creditsAnimator);
                break;

            case Menu.GameOver:
                UIAnimateOut(overlayAnimator);
                UIAnimateOut(gameOverAnimator);
                break;
        }

        menu = Menu.MainMenu;
        mainMenuScreen.gameObject.SetActive(true);

        if (Manager.Current.SelectedObject)
            Manager.Current.SelectedObject.ToggleSelect();

        mainMenuContinueButtonLabel.text = (Manager.Current.CompletedLevels == 0 || Manager.Current.CompletedLevels >= Config.Current.Levels.Length) && !inEscapeMenu ? "New Game" : "Continue";
        mainMenuResetButton.interactable = inEscapeMenu;
        thxForPlayingMessage.gameObject.SetActive(Manager.Current.CompletedLevels >= Config.Current.Levels.Length);
    }

    public void ShowMainMenu() => ShowMainMenu(false);

    public void ShowCredits()
    {
        if (menu == Menu.Credits)
        {
            menu = Menu.MainMenu;
            UIAnimateOut(creditsAnimator);
        }
        else
        {
            if (!InMainOrSubMenu) ShowMainMenu();
            else if (menu == Menu.LevelSelect)
                UIAnimateOut(levelSelectAnimator);

            menu = Menu.Credits;
            creditsScreen.gameObject.SetActive(true);
        }
    }

    public void ShowLevelSelect()
    {
        if (menu == Menu.LevelSelect)
        {
            menu = Menu.MainMenu;
            UIAnimateOut(levelSelectAnimator);
        }
        else
        {
            if (!InMainOrSubMenu) ShowMainMenu();
            else if (menu == Menu.Credits) UIAnimateOut(creditsAnimator);

            menu = Menu.LevelSelect;
            levelSelectScreen.gameObject.SetActive(true);
            LevelSelectPage = LevelSelectPage;
        }
    }
    public void OnLevelSelectNext()
    {
        if ((LevelSelectPage + 1) * levelDisplays.Length <= Config.Current.Levels.Length)
            LevelSelectPage++;
    }

    public void OnLevelSelectBack()
    {
        if (LevelSelectPage > 0)
            LevelSelectPage--;
    }

    public void SelectLevel(int display) => Manager.Current.LoadLevel(displayedLevels[display]);

    public void ExitMenus()
    {
        if (InMainOrSubMenu)
        {
            UIAnimateOut(mainMenuAnimator);

            if (menu == Menu.LevelSelect)
                UIAnimateOut(levelSelectAnimator);
            else if (menu == Menu.Credits)
                UIAnimateOut(creditsAnimator);
        }
        else if (menu == Menu.GameOver)
            UIAnimateOut(gameOverAnimator);

        menu = Menu.None;

        if (thxForPlayingMessage.gameObject.activeSelf)
            UIAnimateOut(thxForPlayingAnimator);

        if (Manager.Current.LevelIndex > 0)
            overlayAnimator.gameObject.SetActive(true);

        Manager.Current.cameraController.zoomAfterMenu = true;
    }
    #endregion
}

public struct PlayerState
{
    public PlayerState(Player player, Vector3 position, bool isPetrified)
    {
        Instance = player;
        Position = position;
        IsPetrified = isPetrified;
    }

    public Player Instance { get; }
    public Vector3 Position { get; }
    public bool IsPetrified { get; }
}