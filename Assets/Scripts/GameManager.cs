using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Serialized variables
    [Space(2), Header("Colors")]
    public Color highlightTint = new Color(40, 40, 40, 10);
    public Color selectionTint = new Color(80, 80, 80, 10);
    public Color nextMoveTint = new Color(100, 100, 40, 10);
    public Color petrifiedColor = new Color(120, 120, 120);
    [Space(2), Range(.1f, 10)]
    public float playerMovementSpeed = 1;
    [Space(2), Header("UI"), SerializeField]
    private Transform mainMenuScreen;
    [SerializeField]
    private Transform gameOverScreen;
    public TMP_Text movesPerTurnDisplay;
    public TMP_Text movesLeftDisplay;
    public TMP_Text turnDisplay;
    [SerializeField]
    private TMP_Text dialogueBox;
    public Transform sun;

    // Hidden variables
    [HideInInspector]
    public LevelData lvl;
    [HideInInspector]
    public int completedLevels = 0;
    [HideInInspector]
    public HexField[] coloredFields;
    [HideInInspector]
    public bool inMenu = false;
    private bool onStartup = true;
    [HideInInspector]
    public string dialogueMSG;
    [HideInInspector]
    public float lightRotationAngle;
    [HideInInspector]
    public float extra90deg = 0;

    [HideInInspector]
    public List<Player> movedPlayers;

    MouseSelectable selected;
    [HideInInspector]
    public Player selectedPlayer;
    public MouseSelectable Selected
    {
        get => selected;
        set
        {
            if (selected && selected.GetComponent<Player>()) selectedPlayer = selected.GetComponent<Player>();
            else selectedPlayer = null;
            selected = value;
        }
    }

    // Singleton instance
    public static GameManager instance;
    void Awake()
    {
        if (instance)
            Destroy(this);
        else
            instance = this;
        DontDestroyOnLoad(this);
        LoadFirstLevel();
        SceneManager.sceneLoaded += OnLoadCallback;
    }

    private void OnLoadCallback(Scene scene, LoadSceneMode sceneMode)
    {
        lvl = FindObjectOfType<LevelData>();
        if (lvl)
        {
            movedPlayers = new List<Player>();
            movesPerTurnDisplay.text = "";
            for (int i = 0; i < lvl.movesPerTurn; i++) movesPerTurnDisplay.text += "o ";
            movesLeftDisplay.text = "";
            for (int i = 0; i < lvl.MovesLeft; i++) movesLeftDisplay.text += "o ";
            foreach (TMP_Text txt in turnDisplay.GetComponentsInChildren<TMP_Text>())
                txt.text = lvl.TurnsLeft + " Day" + (lvl.TurnsLeft == 1 ? "" : "s");
            lightRotationAngle = 40;
        }

        if (scene.buildIndex > 0 && onStartup)
        { 
            MainMenu();
            onStartup = false;
        }
        else
        {
            ExitMenu();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!inMenu) MainMenu(true);
            else if (mainMenuScreen.gameObject.activeSelf) ExitMenu();
        }

        if (sun.rotation == Quaternion.Euler(lightRotationAngle, 45, 0))
        {
            if (extra90deg > 0) { extra90deg--; lightRotationAngle += 90; lightRotationAngle %= 360; }
        }
        else
            sun.rotation = Quaternion.RotateTowards(sun.rotation, Quaternion.Euler(lightRotationAngle, 45, 0), (extra90deg == 1 ? 1000 : 300) * Time.deltaTime);
    }

    public void PetrifyLonePlayers()
    {
        Player[] players = FindObjectsOfType<Player>();
        foreach (Player p in players)
        {
            if (HexGrid.GetPlayersAt(p.position, false).Length > 1) continue;
            bool playersNearby = false;
            foreach (Vector2Int f in HexGrid.GetAdjacentFields(p.position))
                if (HexGrid.GetPlayersAt(f, false).Length > 0) { playersNearby = true; break; }
            if (playersNearby) continue;
            Petrify(p);
        }
    }

    void Petrify(Player p)
    {
        p.rend.material.SetColor("_Color", petrifiedColor);
        p.initialColor = petrifiedColor;
        p.tag = "Petrified";
    }

    public void GameOver(string msg, bool complete = false)
    {
        HideDialog();
        if (inMenu) return;
        if (complete)
        {
            completedLevels++;
            gameOverScreen.Find("Continue").GetComponent<Button>().interactable = true;
        }
        else gameOverScreen.Find("Continue").GetComponent<Button>().interactable = false;
        inMenu = true;
        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.Find("GameOverMSG").GetComponent<TMP_Text>().text = msg;
        turnDisplay.gameObject.SetActive(false);
    }

    public void MainMenu(bool byEsc = false)
    {
        HideDialog();
        Button button = mainMenuScreen.Find("Continue").GetComponent<Button>();
        if (byEsc)
        {
            mainMenuScreen.Find("EscToReturn").gameObject.SetActive(true);
            button.GetComponentInChildren<TMP_Text>().text = "Reset";
            button.interactable = true;
        }
        else
        {
            turnDisplay.gameObject.SetActive(false);
            mainMenuScreen.Find("EscToReturn").gameObject.SetActive(false);
            button.GetComponentInChildren<TMP_Text>().text = "Continue";
            if (completedLevels == 0)
            {
                button.interactable = false;
                mainMenuScreen.Find("Start").GetComponentInChildren<TMP_Text>().text = "Start";
            }
            else
            {
                button.interactable = true;
                mainMenuScreen.Find("Start").GetComponentInChildren<TMP_Text>().text = "Restart";
            }
        }
        if (completedLevels >= SceneManager.sceneCount)
            mainMenuScreen.Find("ThxForPlaying").gameObject.SetActive(true);
        inMenu = true;
        mainMenuScreen.gameObject.SetActive(true);
        gameOverScreen.gameObject.SetActive(false);
    }

    public void ExitMenu()
    {
        inMenu = false;
        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);
        turnDisplay.gameObject.SetActive(true);
    }

    public void Reload()
        => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void LoadFirstLevel()
        => SceneManager.LoadScene(1);
    public void LoadNextLevel()
    {
        if (completedLevels >= SceneManager.GetActiveScene().buildIndex + 1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else LoadLatestLevel();
    }

    public void LoadLatestLevel()
    {
        if (completedLevels >= SceneManager.sceneCount)
        {
            MainMenu();
        }
        else SceneManager.LoadScene(completedLevels + 1);
    }

    public void Quit()
        => Application.Quit();

    public void HideDialog()
    {
        dialogueMSG = "";
        dialogueBox.transform.parent.gameObject.SetActive(false);
    }
    public void ShowDialog(string msg)
    {
        dialogueMSG = msg;
        dialogueBox.text = msg;
        dialogueBox.transform.parent.gameObject.SetActive(true);
    }
}