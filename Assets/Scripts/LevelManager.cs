using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public LevelData current;
    [HideInInspector] public int completed = 0;
    public int count = 15;

    public int CurrentIndex { get; set; }


    public void Load(int index)
    {
        if (index <= count) SceneManager.LoadScene(index);
        else Manager.UI.Credits();
    }
    public void LoadCurrent() => Load(CurrentIndex);
    public void LoadNext() => Load(CurrentIndex + 1);
    public void LoadLatest() => Load(completed + 1);

    public void Continue()
    {
        if (Manager.UI.inEscapeMenu) Manager.UI.ExitMenu();
        else if (Manager.Levels.completed >= Manager.Levels.count) RestartGame();
        else LoadLatest();
    }

    public void RestartGame()
    {
        completed = 0;
        LoadLatest();
    }

    public void Quit() => Application.Quit();
}