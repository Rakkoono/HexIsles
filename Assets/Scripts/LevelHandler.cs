using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelHandler : MonoBehaviour
{
    [HideInInspector] public Level current;
    [HideInInspector] public int completed = 0;

    public int CurrentIndex { get; set; }


    public void Load(int index)
    {
        if (index <= Config.Instance.Levels.Length) SceneManager.LoadScene(index);
        else Manager.UI.Credits();
    }

    public void LoadCurrent() => Load(CurrentIndex);
    public void LoadNext() => Load(CurrentIndex + 1);
    public void LoadLatest() => Load(completed + 1);

    public void Continue()
    {
        if (Manager.UI.inEscapeMenu) Manager.UI.ExitMenu();
        else if (Manager.Levels.completed >= Config.Instance.Levels.Length) RestartGame();
        else LoadLatest();
    }

    public void RestartGame()
    {
        completed = 0;
        LoadLatest();
    }

    public void UnlockAll()
    {
            Manager.Levels.completed = Config.Instance.Levels.Length;
            if (Manager.UI.currentMenu != UIHandler.Menu.LevelSelect) Manager.UI.LevelSelect();
    }

    public void Quit() => Application.Quit();
}