using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public enum Menu { None, GameOver, MainMenu, LevelSelect, Credits, Options }

    [SerializeField] private GameObject[] levelDisplays = new GameObject[9];
    public TMP_Text turnDisplay;
    [Space]
    [SerializeField] private Transform overlay;
    [SerializeField] private Transform mainMenu;
    [SerializeField] private Transform gameOver;
    [SerializeField] private Transform credits;
    [SerializeField] private Transform levelSelect;

    private Animator overlayAnimator;
    private Animator mainMenuAnimator;
    private Animator gameOverAnimator;
    private Animator creditsAnimator;
    private Animator levelSelectAnimator;

    [HideInInspector] public Menu currentMenu = Menu.MainMenu;
    [HideInInspector] public bool inEscapeMenu;
    public bool InMainOrSubMenu
        => currentMenu == Menu.MainMenu
        || currentMenu == Menu.Credits
        || currentMenu == Menu.LevelSelect
        || currentMenu == Menu.Options;

    private readonly int[] displayedLevels = new int[9];

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

                if (displayedLevels[i] > Config.Instance.Levels.Length)
                    levelDisplays[i].SetActive(false);
                else
                {
                    levelDisplays[i].SetActive(true);
                    levelDisplays[i].GetComponentInChildren<TMP_Text>().text = (Manager.Levels.completed >= displayedLevels[i] - 1) ? displayedLevels[i] + " " + Config.Instance.Levels[displayedLevels[i] - 1].DisplayName : "???";
                    levelDisplays[i].transform.Find("Preview").GetComponent<Image>().sprite = (Manager.Levels.completed >= displayedLevels[i] - 1) ? Config.Instance.Levels[displayedLevels[i] - 1].PreviewImage : null;
                    levelDisplays[i].GetComponent<Button>().interactable = Manager.Levels.completed >= displayedLevels[i] - 1;
                }
            }
        }
    }

    public void Initialize() {
        overlayAnimator = overlay.GetComponent<Animator>();
        mainMenuAnimator = mainMenu.GetComponent<Animator>();
        gameOverAnimator = gameOver.GetComponent<Animator>();
        creditsAnimator = credits.GetComponent<Animator>();
        levelSelectAnimator = levelSelect.GetComponent<Animator>();

        overlay.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(false);
        gameOver.gameObject.SetActive(false);
        credits.gameObject.SetActive(false);
        levelSelect.gameObject.SetActive(false);

        Manager.Dialogs.dialogBoxAnimator.gameObject.SetActive(false);
    }

    public void GameOver(GameOver gameOver)
    {
        if (currentMenu != Menu.None) return;

        if (Manager.Players.SelectedObject && Manager.Players.SelectedObject.GetComponent<Player>())
            Manager.Players.SelectedObject.ToggleSelect();

        if (gameOver.UnlockNextLevel && Manager.Levels.completed == Manager.Levels.CurrentIndex - 1)
            Manager.Levels.completed++;

        this.gameOver.Find("Continue").GetComponent<Button>().interactable = Manager.Levels.completed >= Manager.Levels.CurrentIndex;

        currentMenu = Menu.GameOver;
        overlayAnimator.gameObject.SetActive(true);
        this.gameOver.gameObject.SetActive(true);
        this.gameOver.Find("GameOverMSG").GetComponent<TMP_Text>().text = gameOver.Messages[Random.Range(0, gameOver.Messages.Length)];

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
                levelSelectAnimator.Play("LevelSelectOut");
                break;

            case Menu.Credits:
                creditsAnimator.Play("CreditsOut");
                break;

            case Menu.GameOver:
                overlayAnimator.Play("OverlayOut");
                gameOverAnimator.Play("GameOverOut");
                break;
        }

        currentMenu = Menu.MainMenu;
        mainMenu.gameObject.SetActive(true);

        if (Manager.Players.SelectedObject)
            Manager.Players.SelectedObject.ToggleSelect();

        mainMenu.Find("Start").GetComponentInChildren<TMP_Text>().text = (Manager.Levels.completed == 0 || Manager.Levels.completed >= Config.Instance.Levels.Length) && !inEscapeMenu ? "New Game" : "Continue";
        mainMenu.Find("Reset").GetComponent<Button>().interactable = inEscapeMenu;
        mainMenu.Find("ThxForPlaying").gameObject.SetActive(Manager.Levels.completed >= Config.Instance.Levels.Length);
    }
    public void MainMenu() => MainMenu(false);

    public void Credits()
    {
        if (currentMenu == Menu.Credits)
        {
            currentMenu = Menu.MainMenu;
            creditsAnimator.Play("CreditsOut");
        }
        else
        {
            if (!InMainOrSubMenu) MainMenu();
            else if (currentMenu == Menu.LevelSelect) levelSelectAnimator.Play("LevelSelectOut");

            currentMenu = Menu.Credits;
            credits.gameObject.SetActive(true);
        }
    }

    public void LevelSelect()
    {
        if (currentMenu == Menu.LevelSelect)
        {
            currentMenu = Menu.MainMenu;
            levelSelectAnimator.Play("LevelSelectOut");
        }
        else
        {
            if (!InMainOrSubMenu) MainMenu();
            else if (currentMenu == Menu.Credits) creditsAnimator.Play("CreditsOut");

            currentMenu = Menu.LevelSelect;
            levelSelect.gameObject.SetActive(true);
            if (Manager.UI.LevelSelectPage == 0)
                Manager.UI.LevelSelectPage = 0;
        }
    }
    public void NextLevelSelectPage()
    {
        if ((LevelSelectPage + 1) * levelDisplays.Length <= Config.Instance.Levels.Length) LevelSelectPage++;
    }
    public void BackLevelSelectPage()
    {
        if (LevelSelectPage > 0) LevelSelectPage--;
    }
    public void SelectLevel(int display) => Manager.Levels.Load(displayedLevels[display]);

    public void ExitMenu()
    {
        if (InMainOrSubMenu)
        {
            mainMenuAnimator.Play("MainMenuOut");
            if (currentMenu == Menu.LevelSelect)
                levelSelectAnimator.Play("LevelSelectOut");
            else if (currentMenu == Menu.Credits)
                creditsAnimator.Play("CreditsOut");
        }
        else if (currentMenu == Menu.GameOver)
            gameOverAnimator.Play("GameOverOut");
        
        currentMenu = Menu.None;

        GameObject thx = mainMenu.Find("ThxForPlaying").gameObject;
        if (thx.activeSelf) thx.GetComponent<Animator>().Play("ThxForPlayingOut");

        if (Manager.Levels.CurrentIndex > 0)
            overlayAnimator.gameObject.SetActive(true);

        Manager.Camera.zoomAfterMenu = true;
    }
}