using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [HideInInspector] public LevelData current;
    [HideInInspector] public int completed = 0;
    public int count = 10;

    public int CurrentIndex { get; set; }

    public void LoadCurrent() => SceneManager.LoadScene(CurrentIndex);

    public void Load(int index)
    {
        if (index <= count) SceneManager.LoadScene(index);
        else Manager.GUI.Credits();
    }

    public void LoadNext()
    {
        if (completed + 1 >= CurrentIndex)
            Load(completed + 1);
    }

    public void Quit() => Application.Quit();
}