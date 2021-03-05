using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public enum Menu { None, GameOver, MainMenu, LevelSelect, Credits, Options }

    [SerializeField] private Transform mainMenuScreen;
    [SerializeField] private Transform gameOverScreen;
    [SerializeField] private Transform creditsScreen;
    [SerializeField] private Transform levelSelectScreen;

    [SerializeField] private GameObject[] lvlDisplays = new GameObject[9];

    public TMP_Text turnDisplay;

    private readonly int[] displayedLvls = new int[9];

    [SerializeField] private Animator overlayAnimator;
    [SerializeField] private Animator gameOverScreenAnimator;
    [SerializeField] private Animator mainMenuScreenAnimator;
    [SerializeField] private Animator levelSelectScreenAnimator;
    [SerializeField] private Animator creditsScreenAnimator;
    public Animator dialogBoxAnimator;

    [HideInInspector] public Menu currentMenu = Menu.MainMenu;
    [HideInInspector] public bool inEscapeMenu;
    public bool InMainOrSubMenu => currentMenu == Menu.MainMenu || currentMenu == Menu.Credits || currentMenu == Menu.LevelSelect || currentMenu == Menu.Options;

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
                    lvlDisplays[i].GetComponentInChildren<TMP_Text>().text = (Manager.Levels.completed >= displayedLvls[i] - 1) ? displayedLvls[i] + " " + Config.Instance.Levels[displayedLvls[i] - 1].DisplayName : "???";
                    lvlDisplays[i].transform.Find("Preview").GetComponent<Image>().sprite = (Manager.Levels.completed >= displayedLvls[i] - 1) ? Config.Instance.Levels[displayedLvls[i] - 1].PreviewImage : null;
                    lvlDisplays[i].GetComponent<Button>().interactable = Manager.Levels.completed >= displayedLvls[i] - 1;
                }
            }
        }
    }

    public void GameOver(GameOver gameOver)
    {
        if (currentMenu != Menu.None) return;

        if (Manager.Players.SelectedObject && Manager.Players.SelectedObject.GetComponent<Player>())
            Manager.Players.SelectedObject.ToggleSelect();

        if (gameOver.UnlockNextLevel && Manager.Levels.completed == Manager.Levels.CurrentIndex - 1)
            Manager.Levels.completed++;

        gameOverScreen.Find("Continue").GetComponent<Button>().interactable = Manager.Levels.completed >= Manager.Levels.CurrentIndex;

        currentMenu = Menu.GameOver;
        overlayAnimator.gameObject.SetActive(true);
        gameOverScreen.gameObject.SetActive(true);
        gameOverScreen.Find("GameOverMSG").GetComponent<TMP_Text>().text = gameOver.Messages[Random.Range(0, gameOver.Messages.Length)];

        // Play sound effect
        Manager.Players.sfxSource.PlayOneShot(gameOver.UnlockNextLevel ? Manager.Players.levelCompleteSound : Manager.Players.gameOverSound, .7f);
    }

    public void ToggleMainMenu()
    {
            if (currentMenu == Menu.None) MainMenu(true);
            else if (inEscapeMenu) ExitMenu();
    }

    public void MainMenu(bool byEsc)
    {
        inEscapeMenu = byEsc;

        switch (currentMenu)
        {
            case Menu.None:
                if (overlayAnimator.gameObject.activeSelf) overlayAnimator.Play("OverlayOut");
                break;

            case Menu.LevelSelect:
                levelSelectScreenAnimator.Play("LevelSelectOut");
                break;

            case Menu.Credits:
                creditsScreenAnimator.Play("CreditsOut");
                break;

            case Menu.GameOver:
                overlayAnimator.Play("OverlayOut");
                gameOverScreenAnimator.Play("GameOverOut");
                break;
        }

        currentMenu = Menu.MainMenu;
        mainMenuScreen.gameObject.SetActive(true);

        if (Manager.Players.SelectedObject)
            Manager.Players.SelectedObject.ToggleSelect();

        mainMenuScreen.Find("Start").GetComponentInChildren<TMP_Text>().text = (Manager.Levels.completed == 0 || Manager.Levels.completed >= Manager.Levels.count) && !inEscapeMenu ? "New Game" : "Continue";
        mainMenuScreen.Find("Reset").GetComponent<Button>().interactable = inEscapeMenu;
        mainMenuScreen.Find("ThxForPlaying").gameObject.SetActive(Manager.Levels.completed >= Manager.Levels.count);
    }
    public void MainMenu() => MainMenu(false);

    public void Credits()
    {
        if (currentMenu == Menu.Credits)
        {
            currentMenu = Menu.MainMenu;
            creditsScreenAnimator.Play("CreditsOut");
        }
        else
        {
            if (!InMainOrSubMenu) MainMenu();
            else if (currentMenu == Menu.LevelSelect) levelSelectScreenAnimator.Play("LevelSelectOut");

            currentMenu = Menu.Credits;
            creditsScreen.gameObject.SetActive(true);
        }
    }

    public void LevelSelect()
    {
        if (currentMenu == Menu.LevelSelect)
        {
            currentMenu = Menu.MainMenu;
            levelSelectScreenAnimator.Play("LevelSelectOut");
        }
        else
        {
            if (!InMainOrSubMenu) MainMenu();
            else if (currentMenu == Menu.Credits) creditsScreenAnimator.Play("CreditsOut");

            currentMenu = Menu.LevelSelect;
            levelSelectScreen.gameObject.SetActive(true);
            if (Manager.UI.LvlSelectPage == 0)
                Manager.UI.LvlSelectPage = 0;
        }
    }
    public void NextLevelSelectPage()
    {
        if ((LvlSelectPage + 1) * lvlDisplays.Length <= Manager.Levels.count) LvlSelectPage++;
    }
    public void BackLevelSelectPage()
    {
        if (LvlSelectPage > 0) LvlSelectPage--;
    }
    public void SelectLevel(int display) => Manager.Levels.Load(displayedLvls[display]);

    public void ExitMenu()
    {
        if (InMainOrSubMenu)
        {
            mainMenuScreenAnimator.Play("MainMenuOut");
            if (currentMenu == Menu.LevelSelect)
                levelSelectScreenAnimator.Play("LevelSelectOut");
            else if (currentMenu == Menu.Credits)
                creditsScreenAnimator.Play("CreditsOut");
        }
        else if (currentMenu == Menu.GameOver)
            gameOverScreenAnimator.Play("GameOverOut");
        
        currentMenu = Menu.None;

        GameObject thx = mainMenuScreen.Find("ThxForPlaying").gameObject;
        if (thx.activeSelf) thx.GetComponent<Animator>().Play("ThxForPlayingOut");

        if (Manager.Levels.CurrentIndex > 0)
            overlayAnimator.gameObject.SetActive(true);

        Manager.Camera.zoomAfterMenu = true;
    }
}