using System.Collections.Generic;
using System.Linq;
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
    [SerializeField]
    private Transform creditsScreen;
    public TMP_Text movesPerTurnDisplay;
    public TMP_Text movesLeftDisplay;
    public TMP_Text turnDisplay;
    [SerializeField]
    private GameObject dialogBox;
    [SerializeField]
    private TMP_Text[] dialogBoxLines = new TMP_Text[3];
    public Transform sun;
    public int levelCount = 10;

    public string[] outOfTurnsMSG;
    public string[] allPetrifiedMSG;
    public string[] levelCompleteMSG;

    // Hidden variables
    [HideInInspector]
    public LevelData lvl;
    [HideInInspector]
    public int completedLevels = 0;
    [HideInInspector]
    public HexField[] coloredFields;
    [HideInInspector]
    public bool inMenu = false;
    private bool onStartup = true, showUseSigns = true;
    [HideInInspector]
    public Sign currentSign;
    [HideInInspector]
    private int dialogPage = 0;
    [HideInInspector]
    public Player[] players;
    [HideInInspector]
    public List<Player> playersMoved;

    MouseSelectable selected;
    [HideInInspector]
    public Player selectedPlayer;
    public MouseSelectable Selected
    {
        get => selected;
        set
        {
            if (selected) selectedPlayer = selected.GetComponent<Player>();
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
            movesPerTurnDisplay.text = "";
            for (int i = 0; i < lvl.movesPerTurn; i++) movesPerTurnDisplay.text += "o ";
            movesLeftDisplay.text = "";
            for (int i = 0; i < lvl.MovesLeft; i++) movesLeftDisplay.text += "o ";
            foreach (TMP_Text txt in turnDisplay.GetComponentsInChildren<TMP_Text>())
                txt.text = lvl.TurnsLeft + " Turn" + (lvl.TurnsLeft == 1 ? "" : "s");
            playersMoved = new List<Player>();
            players = FindObjectsOfType<Player>();
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
            if (!mainMenuScreen.gameObject.activeSelf && !gameOverScreen.gameObject.activeSelf) MainMenu(true, creditsScreen.gameObject.activeSelf);
            else if (mainMenuScreen.Find("EscToReturn").gameObject.activeSelf) ExitMenu();
        }

        sun.RotateAround(transform.position, Vector3.up, 8 * Time.deltaTime);
    }

    public void PetrifyLonePlayers()
    {
        foreach (Player p in players)
        {
            if (HexGrid.GetPlayersAt(p.position, false).Length > 1) continue;
            bool playersNearby = false;
            foreach (Vector2Int f in HexGrid.GetAdjacentFields(p.position))
                if (HexGrid.GetPlayersAt(f, false).Length > 0)
                {
                    playersNearby = true;
                    break;
                }
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
        if (selected && selected.GetComponent<Player>()) selected.OnMouseDown();
        HideDialog();
        if (inMenu) return;
        if (complete && completedLevels == SceneManager.GetActiveScene().buildIndex - 1)
            completedLevels++;
        if (completedLevels >= SceneManager.GetActiveScene().buildIndex)
            gameOverScreen.Find("Continue").GetComponent<Button>().interactable = true;
        else gameOverScreen.Find("Continue").GetComponent<Button>().interactable = false;
        inMenu = true;
        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.Find("GameOverMSG").GetComponent<TMP_Text>().text = msg;
        turnDisplay.gameObject.SetActive(false);
        movesPerTurnDisplay.gameObject.SetActive(false);
    }

    public void MainMenu(bool byEsc, bool fromCredits)
    {

        if (selected && selected.GetComponent<Player>()) selected.OnMouseDown();
        HideDialog();
        if (!fromCredits)
        {
            Button button = mainMenuScreen.Find("Continue").GetComponent<Button>();
            if (byEsc)
            {
                mainMenuScreen.Find("EscToReturn").gameObject.SetActive(true);
                mainMenuScreen.Find("Start").GetComponentInChildren<TMP_Text>().text = "Restart";
                button.GetComponentInChildren<TMP_Text>().text = "Reset";
                button.interactable = true;
            }
            else
            {
                turnDisplay.gameObject.SetActive(false);
                movesPerTurnDisplay.gameObject.SetActive(false);
                mainMenuScreen.Find("EscToReturn").gameObject.SetActive(false);
                button.GetComponentInChildren<TMP_Text>().text = "Continue";
                if (completedLevels == 0 || completedLevels >= levelCount)
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
            if (completedLevels >= levelCount) Credits();
        }


        if (completedLevels == levelCount)
            mainMenuScreen.Find("ThxForPlaying").gameObject.SetActive(true);
        inMenu = true;
        creditsScreen.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(true);
        gameOverScreen.gameObject.SetActive(false);
    }
    public void MainMenu() => MainMenu(false, false);

    public void Credits()
    {
        inMenu = true;
        creditsScreen.gameObject.SetActive(true);
        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);

    }

    public void ExitMenu()
    {
        if (SceneManager.GetActiveScene().buildIndex > 0 && mainMenuScreen.gameObject.activeSelf && showUseSigns)
        {
            showUseSigns = false;
            HideDialog();
            ShowDialog("\nClick on Signs to read them");
        }

        inMenu = false;
        creditsScreen.gameObject.SetActive(false);
        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);
        movesPerTurnDisplay.gameObject.SetActive(true);
        turnDisplay.gameObject.SetActive(true);

    }

    public void Reload()
        => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    public void LoadFirstLevel()
        => SceneManager.LoadScene(1);
    public void LoadNextLevel()
    {
        if (completedLevels >= SceneManager.GetActiveScene().buildIndex && completedLevels < levelCount)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else LoadLatestLevel();
    }

    public void LoadLatestLevel()
    {
        if (completedLevels == levelCount) MainMenu(); 
        else SceneManager.LoadScene(completedLevels + 1);
    }

    public void Quit()
        => Application.Quit();

    public void HideDialog()
    {
        dialogPage = 0;
        dialogBox.SetActive(false);
    }

    public void ShowDialog(Sign sign)
    {
        if (sign == null && currentSign == null)
        {
            HideDialog();
            return;
        }
        if (sign != null && sign != currentSign)
            dialogPage = 0;
        dialogPage++;
        if (sign) currentSign = sign;
        if (dialogPage >= currentSign.dialog.Length) HideDialog();
        else ShowDialog(currentSign.dialog[dialogPage]);
    }
    public void ShowDialog(string dialog)
    {
        string[] lines = dialog.Split('\n');
        for (int i = 0; i < dialogBoxLines.Length; i++)
            dialogBoxLines[i].text = lines.Length > i ? lines[i] : "";
        dialogBox.SetActive(true);
    }
    public void ShowDialog() => ShowDialog(sign: null);
}