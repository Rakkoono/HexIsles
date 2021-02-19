using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    [HideInInspector]
    public bool inMenu = false;
    [SerializeField]
    private Transform mainMenuScreen, gameOverScreen, creditsScreen, levelSelectScreen;

    [SerializeField]
    private GameObject[] lvlDisplays = new GameObject[9];
    [SerializeField]
    private string[] lvlNames = new string[10];
    [SerializeField]
    private Sprite[] lvlPreviews = new Sprite[10];

    public TMP_Text movesPerTurnDisplay, movesLeftDisplay, turnDisplay;

    public string[] outOfTurnsMSG = new string[3], allPetrifiedMSG = new string[3], levelCompleteMSG = new string[3];

    private readonly int[] displayedLvls = new int[9];

    [HideInInspector]
    private int lvlSelectPage;
    public int LvlSelectPage
    {
        get => lvlSelectPage;
        set
        {
            lvlSelectPage = value;
            for (int i = 0; i < lvlDisplays.Length; i++)
            {
                displayedLvls[i] = lvlSelectPage * lvlDisplays.Length + i + 1;

                if (displayedLvls[i] > Manager.Levels.count)
                    lvlDisplays[i].SetActive(false);
                else
                {
                    lvlDisplays[i].SetActive(true);
                    lvlDisplays[i].GetComponentInChildren<TMP_Text>().text = (Manager.Levels.completed >= displayedLvls[i] - 1) ? displayedLvls[i] + " " + lvlNames[displayedLvls[i] - 1] : "???";
                    lvlDisplays[i].transform.Find("Preview").GetComponent<Image>().sprite = (Manager.Levels.completed >= displayedLvls[i] - 1) ? lvlPreviews[displayedLvls[i] - 1] : null;
                    lvlDisplays[i].GetComponent<Button>().interactable = Manager.Levels.completed >= displayedLvls[i] - 1;
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Space))
        {
            if (!inMenu) MainMenu(true);
            else if (mainMenuScreen.Find("EscToReturn").gameObject.activeSelf) ExitMenu();
        }

        if (Input.GetKeyDown(KeyCode.Z) && Input.GetKey(KeyCode.LeftControl)) Manager.Players.Undo();

        if (Input.GetKeyDown(KeyCode.F5))
        {
            Manager.Levels.completed = Manager.Levels.count;
            Manager.GUI.LevelSelect();
        }
    }

    public void GameOver(string msg, bool complete = false)
    {
        if (inMenu) return;
        ExitMenu();

        if (Manager.Players.SelectedObject && Manager.Players.SelectedObject.GetComponent<Player>())
            Manager.Players.SelectedObject.OnMouseDown();

        if (complete && Manager.Levels.completed == Manager.Levels.CurrentIndex - 1)
            Manager.Levels.completed++;

        gameOverScreen.Find("Continue").GetComponent<Button>().interactable = Manager.Levels.completed >= Manager.Levels.CurrentIndex;

        inMenu = true;
        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.Find("GameOverMSG").GetComponent<TMP_Text>().text = msg;
        turnDisplay.gameObject.SetActive(false);
        movesPerTurnDisplay.gameObject.SetActive(false);
    }

    public void MainMenu(bool byEsc)
    {
        ExitMenu();
        inMenu = true;
        mainMenuScreen.gameObject.SetActive(true);

        if (Manager.Players.SelectedObject && Manager.Players.SelectedObject.GetComponent<Player>())
            Manager.Players.SelectedObject.OnMouseDown();
        if (byEsc)
        {
            mainMenuScreen.Find("EscToReturn").gameObject.SetActive(true);
            mainMenuScreen.Find("Start").GetComponentInChildren<TMP_Text>().text = "Restart Game";
            mainMenuScreen.Find("Reset").GetComponent<Button>().interactable = true;
            mainMenuScreen.Find("LevelSelect").GetComponent<Button>().interactable = true;
        }
        else
        {
            turnDisplay.gameObject.SetActive(false);
            movesPerTurnDisplay.gameObject.SetActive(false);

            mainMenuScreen.Find("EscToReturn").gameObject.SetActive(false);
            if (Manager.Levels.completed == 0 || Manager.Levels.completed >= Manager.Levels.count)
                mainMenuScreen.Find("Start").GetComponentInChildren<TMP_Text>().text = Manager.Levels.completed == 0 || Manager.Levels.completed >= Manager.Levels.completed ? "Start Game" : "Restart Game";
            mainMenuScreen.Find("Reset").GetComponent<Button>().interactable = false;
        }

        if (Manager.Levels.completed >= Manager.Levels.count)
            mainMenuScreen.Find("ThxForPlaying").gameObject.SetActive(true);
    }
    public void MainMenu() => MainMenu(false);

    public void Credits()
    {
        bool active = creditsScreen.gameObject.activeSelf;
        ExitMenu();
        MainMenu();
        if (!active)
        {
            movesPerTurnDisplay.gameObject.SetActive(false);
            turnDisplay.gameObject.SetActive(false);
            creditsScreen.gameObject.SetActive(true);
        }
    }

    public void LevelSelect()
    {
        bool active = levelSelectScreen.gameObject.activeSelf;
        ExitMenu();
        MainMenu();
        if (!active)
        {
            levelSelectScreen.gameObject.SetActive(true);
            if (Manager.GUI.LvlSelectPage == 0)
                Manager.GUI.LvlSelectPage = 0;
        }
    }
    public void NextLevelSelectPage()
    {
        if ((LvlSelectPage + 1) * lvlDisplays.Length <= Manager.Levels.count)
            LvlSelectPage++;
    }
    public void BackLevelSelectPage()
    {
        if (LvlSelectPage > 0)
            LvlSelectPage--;
    }
    public void SelectLevel(int display) => Manager.Levels.Load(displayedLvls[display]);

    public void ExitMenu()
    {
        Manager.Dialogs.Hide();

        inMenu = false;

        levelSelectScreen.gameObject.SetActive(false);
        creditsScreen.gameObject.SetActive(false);

        mainMenuScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);

        movesPerTurnDisplay.gameObject.SetActive(true);
        turnDisplay.gameObject.SetActive(true);
    }
}