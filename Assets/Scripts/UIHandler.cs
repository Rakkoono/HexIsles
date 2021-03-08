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

                if (displayedLevels[i] > Config.Current.Levels.Length)
                    levelDisplays[i].SetActive(false);
                else
                {
                    levelDisplays[i].SetActive(true);
                    levelDisplays[i].GetComponentInChildren<TMP_Text>().text = (Manager.Current.completedLevels >= displayedLevels[i] - 1) ? displayedLevels[i] + " " + Config.Current.Levels[displayedLevels[i] - 1].DisplayName : "???";
                    levelDisplays[i].transform.Find("Preview").GetComponent<Image>().sprite = (Manager.Current.completedLevels >= displayedLevels[i] - 1) ? Config.Current.Levels[displayedLevels[i] - 1].PreviewImage : null;
                    levelDisplays[i].GetComponent<Button>().interactable = Manager.Current.completedLevels >= displayedLevels[i] - 1;
                }
            }
        }
    }

    public void Initialize()
    {
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

    public void ShowGameOver(GameOver gameOverType)
    {
        if (currentMenu != Menu.None) return;

        Manager.Dialogs.Hide();
        if (Manager.Players.SelectedObject && Manager.Players.SelectedObject.GetComponent<Player>())
            Manager.Players.SelectedObject.ToggleSelect();

        if (gameOverType.UnlockNextLevel && Manager.Current.completedLevels == Manager.Current.levelIndex - 1)
            Manager.Current.completedLevels++;

        gameOver.Find("Continue").GetComponent<Button>().interactable = Manager.Current.completedLevels >= Manager.Current.levelIndex;

        currentMenu = Menu.GameOver;
        overlayAnimator.gameObject.SetActive(true);
        gameOver.gameObject.SetActive(true);
        gameOver.Find("GameOverMSG").GetComponent<TMP_Text>().text = gameOverType.Messages[Random.Range(0, gameOverType.Messages.Length)];

        // Play sound effect
        Manager.Current.SfxSource.PlayOneShot(gameOverType.Sound, .7f);
    }

    public void ToggleMainMenu()
    {
            if (currentMenu == Menu.None) ShowMainMenu(true);
            else if (inEscapeMenu) ExitMenus();
    }

    public void ShowMainMenu(bool byEsc)
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

        mainMenu.Find("Start").GetComponentInChildren<TMP_Text>().text = (Manager.Current.completedLevels == 0 || Manager.Current.completedLevels >= Config.Current.Levels.Length) && !inEscapeMenu ? "New Game" : "Continue";
        mainMenu.Find("Reset").GetComponent<Button>().interactable = inEscapeMenu;
        mainMenu.Find("ThxForPlaying").gameObject.SetActive(Manager.Current.completedLevels >= Config.Current.Levels.Length);
    }

    public void ShowMainMenu() => ShowMainMenu(false);

    public void ShowCredits()
    {
        if (currentMenu == Menu.Credits)
        {
            currentMenu = Menu.MainMenu;
            creditsAnimator.Play("CreditsOut");
        }
        else
        {
            if (!InMainOrSubMenu) ShowMainMenu();
            else if (currentMenu == Menu.LevelSelect) levelSelectAnimator.Play("LevelSelectOut");

            currentMenu = Menu.Credits;
            credits.gameObject.SetActive(true);
        }
    }

    public void ShowLevelSelect()
    {
        if (currentMenu == Menu.LevelSelect)
        {
            currentMenu = Menu.MainMenu;
            levelSelectAnimator.Play("LevelSelectOut");
        }
        else
        {
            if (!InMainOrSubMenu) ShowMainMenu();
            else if (currentMenu == Menu.Credits) creditsAnimator.Play("CreditsOut");

            currentMenu = Menu.LevelSelect;
            levelSelect.gameObject.SetActive(true);
            Manager.UI.LevelSelectPage = Manager.UI.LevelSelectPage;
        }
    }
    public void LevelSelectNext()
    {
        if ((LevelSelectPage + 1) * levelDisplays.Length <= Config.Current.Levels.Length)
            LevelSelectPage++;
    }

    public void LevelSelectBack()
    {
        if (LevelSelectPage > 0)
            LevelSelectPage--;
    }

    public void SelectLevel(int display) => Manager.Current.LoadLevel(displayedLevels[display]);

    public void ExitMenus()
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

        if (Manager.Current.levelIndex > 0)
            overlayAnimator.gameObject.SetActive(true);

        Manager.Current.cameraController.zoomAfterMenu = true;
    }
}