using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
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

    [SerializeField]
    private Animator creditsAnim, escToReturnAnim, gameOverAnim, levelSelectAnim, mainMenuAnim, gameUIAnim;
    public Animator dialogBoxAnim;

    [HideInInspector]
    public bool zoomAfterMenu, inEscapeMenu;

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
            else if (inEscapeMenu) ExitMenu();
        }

        if (Input.GetKeyDown(KeyCode.Z)) Manager.Players.Undo();
        if (Input.GetKeyDown(KeyCode.R)) Manager.Levels.LoadCurrent();

        if (Input.GetKeyDown(KeyCode.Q) && Input.GetKey(KeyCode.LeftAlt))
        {
            Manager.Levels.completed = Manager.Levels.count;
            Manager.UI.LevelSelect();
        }
    }

    public void GameOver(string[] msg, bool complete = false)
    {
        if (inMenu) return;
        Manager.Dialogs.Hide();

        if (Manager.Players.SelectedObject && Manager.Players.SelectedObject.GetComponent<Player>())
            Manager.Players.SelectedObject.ToggleSelect();

        if (complete && Manager.Levels.completed == Manager.Levels.CurrentIndex - 1)
            Manager.Levels.completed++;

        gameOverScreen.Find("Continue").GetComponent<Button>().interactable = Manager.Levels.completed >= Manager.Levels.CurrentIndex;

        inMenu = true;
        gameUIAnim.Play("GameUIOut");
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.Find("GameOverMSG").GetComponent<TMP_Text>().text = msg[Random.Range(0, msg.Length)];
    }

    public void MainMenu(bool byEsc)
    {
        inEscapeMenu = byEsc;

        Manager.Dialogs.Hide();
        if (levelSelectScreen.gameObject.activeSelf) levelSelectAnim.Play("LevelSelectOut");
        if (creditsScreen.gameObject.activeSelf) creditsAnim.Play("CreditsOut");
        if (gameOverScreen.gameObject.activeSelf) gameOverAnim.Play("GameOverOut");

        inMenu = true;
        mainMenuScreen.gameObject.SetActive(true);

            if (gameUIAnim.gameObject.activeSelf) gameUIAnim.Play("GameUIOut");
        if (Manager.Players.SelectedObject && Manager.Players.SelectedObject.GetComponent<Player>())
            Manager.Players.SelectedObject.ToggleSelect();

        mainMenuScreen.Find("Start").GetComponentInChildren<TMP_Text>().text = (Manager.Levels.completed == 0 || Manager.Levels.completed >= Manager.Levels.count) && !inEscapeMenu ? "New Game" : "Continue";
        mainMenuScreen.Find("Reset").GetComponent<Button>().interactable = inEscapeMenu;
        mainMenuScreen.Find("EscToReturn").gameObject.SetActive(inEscapeMenu);
        mainMenuScreen.Find("ThxForPlaying").gameObject.SetActive(Manager.Levels.completed >= Manager.Levels.count);
    }
    public void MainMenu() => MainMenu(false);

    public void Credits()
    {
        mainMenuScreen.Find("ThxForPlaying").gameObject.SetActive(Manager.Levels.completed >= Manager.Levels.count);
        if (!mainMenuScreen.gameObject.activeSelf) MainMenu();
        else if (levelSelectScreen.gameObject.activeSelf) levelSelectAnim.Play("LevelSelectOut");
        if (creditsScreen.gameObject.activeSelf) creditsAnim.Play("CreditsOut");
        else
        {
            if (gameUIAnim.gameObject.activeSelf) { gameUIAnim.Play("GameUIOut"); };
            creditsScreen.gameObject.SetActive(true);
        }
    }

    public void LevelSelect()
    {
        mainMenuScreen.Find("ThxForPlaying").gameObject.SetActive(Manager.Levels.completed >= Manager.Levels.count);
        if (!mainMenuScreen.gameObject.activeSelf) MainMenu();
        else if (creditsScreen.gameObject.activeSelf) creditsAnim.Play("CreditsOut");
        if (levelSelectScreen.gameObject.activeSelf) levelSelectAnim.Play("LevelSelectOut");
        else
        {
            levelSelectScreen.gameObject.SetActive(true);
            if (Manager.UI.LvlSelectPage == 0)
                Manager.UI.LvlSelectPage = 0;
        }
    }
    public void NextLevelSelectPage()
    {
        if ((LvlSelectPage + 1) * lvlDisplays.Length <= Manager.Levels.count)
        {
            LvlSelectPage++;
        }
    }
    public void BackLevelSelectPage()
    {
        if (LvlSelectPage > 0)
        {
            LvlSelectPage--;
        }
    }
    public void SelectLevel(int display) => Manager.Levels.Load(displayedLvls[display]);

    public void ExitMenu()
    {
        Manager.Dialogs.Hide();

        inMenu = false;

        if (levelSelectScreen.gameObject.activeSelf) levelSelectAnim.Play("LevelSelectOut");
        if (creditsScreen.gameObject.activeSelf) creditsAnim.Play("CreditsOut");

        if (mainMenuScreen.gameObject.activeSelf) mainMenuAnim.Play("MainMenuOut");
        if (gameOverScreen.gameObject.activeSelf) gameOverAnim.Play("GameOverOut");
        GameObject thx = mainMenuScreen.Find("ThxForPlaying").gameObject;
        if (thx.activeSelf) thx.GetComponent<Animator>().Play("ThxForPlayingOut");
        if (mainMenuScreen.Find("EscToReturn").gameObject.activeSelf) escToReturnAnim.Play("EscToReturnOut");

        if (Manager.Levels.CurrentIndex > 0)
        {
            movesPerTurnDisplay.gameObject.SetActive(true);
            gameUIAnim.gameObject.SetActive(true);
        }
        zoomAfterMenu = true;
    }
}