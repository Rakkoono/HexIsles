using UnityEngine;

public class LevelSelectMenu : Menu
{
    [SerializeField] private LevelSelectDisplay[] displays = new LevelSelectDisplay[9];

    private readonly int[] displayedLevels = new int[9];

    [HideInInspector] private int levelSelectPage;
    public int LevelSelectPage
    {
        get => levelSelectPage;
        set
        {
            levelSelectPage = value;
            for (int i = 0; i < displays.Length; i++)
            {
                displayedLevels[i] = levelSelectPage * displays.Length + i + 1;

                if (displayedLevels[i] > Config.Levels.Length)
                    displays[i].gameObject.SetActive(false);
                else
                {
                    displays[i].gameObject.SetActive(true);
                    displays[i].Text = (Manager.Instance.CompletedLevels >= displayedLevels[i] - 1) ? displayedLevels[i] + " " + Config.Levels[displayedLevels[i] - 1].DisplayName : "???";
                    displays[i].Sprite = (Manager.Instance.CompletedLevels >= displayedLevels[i] - 1) ? Config.Levels[displayedLevels[i] - 1].PreviewImage : null;
                    displays[i].Interactable = Manager.Instance.CompletedLevels >= displayedLevels[i] - 1;
                }
            }
        }
    }

    public override void Open()
    {
        base.Open();
        LevelSelectPage = LevelSelectPage;
    }

    public void NextPage()
    {
        if ((LevelSelectPage + 1) * displays.Length <= Config.Levels.Length)
            LevelSelectPage++;
    }

    public void LastPage()
    {
        if (LevelSelectPage > 0)
            LevelSelectPage--;
    }

    public void SelectLevel(int display) => Manager.Instance.LoadLevel(displayedLevels[display]);
}