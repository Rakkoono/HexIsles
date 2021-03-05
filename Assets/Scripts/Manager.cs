using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    [SerializeField] private Config config;

    private bool onStartup = true;
    private Transform sun;

    public static int TurnsLeft { get; set; }
    public static CameraHandler Camera { get; private set; }
    public static UIHandler UI { get; private set; }
    public static DialogHandler Dialogs { get; private set; }
    public static PlayerHandler Players { get; private set; }
    public static LevelHandler Levels { get; private set; }


    void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        // if not on mobile, deactivate mobile only objects
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("MobileOnly"))
            obj.SetActive(false);

#endif
        // Find sun transform
        sun = GetComponentInChildren<Light>().transform;

        // Keep manager loaded during scene change
        DontDestroyOnLoad(gameObject);
        // Execute OnLoadCallback() on scene change
        SceneManager.sceneLoaded += OnLoadCallback;

        // Get Handlers
        Camera = GetComponent<CameraHandler>();
        UI = GetComponent<UIHandler>();
        Dialogs = GetComponent<DialogHandler>();
        Players = GetComponent<PlayerHandler>();
        Levels = GetComponent<LevelHandler>();

        // Get saved data
        if (PlayerPrefs.HasKey("completedLevels"))
            Levels.completed = PlayerPrefs.GetInt("completedLevels");

        // Load next or latest level
        if (Levels.completed >= Levels.count)
            Levels.LoadNext();
        else
            Levels.LoadLatest();
    }

    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        Levels.CurrentIndex = scene.buildIndex;
        if (Levels.CurrentIndex != 0)
        {
            Levels.current = Config.Instance.Levels[Levels.CurrentIndex - 1];
            TurnsLeft = Levels.current.Turns;
        }

        Players.players = FindObjectsOfType<Player>();
        Players.undoList = new List<PlayerInfo[]>();

        if (Levels.CurrentIndex > 0 && onStartup)
        {
            UI.MainMenu();
            onStartup = false;
        }
        else UI.ExitMenu();

    }

    [ContextMenu("Reset Player Prefs")]
    public void ResetPlayerPrefs() => PlayerPrefs.DeleteAll();

    private void Update() => sun.RotateAround(transform.position, Vector3.up, 8 * Time.deltaTime);

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("completedLevels", Levels.completed);
        PlayerPrefs.Save();
    }
}