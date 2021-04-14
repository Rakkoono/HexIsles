using System.Linq;
using UnityEngine;

public class HexField : MouseSelectable
{
    // Hidden fields
    [SerializeField] private int height = 1;
    public int Height
    {
        get => height;
        set
        {
            height = value;
            transform.localPosition = new Vector3(0, .25f * height - .25f, 0);
            transform.localScale = new Vector3(1, .5f * height, 1);
        }
    }

    [SerializeField] private Vector2Int position;
    public Vector2Int Position
    {
        get => position;
        set
        {
            position = value;
            transform.parent.position = GridUtility.GridToWorldPos(position);
        }
    }

    // Is this object a possible target for the last selected player?
    public bool IsTarget => Manager.Current.LastSelectedPlayer && Manager.Current.validTargets.Contains(this);

    public override void OnSelect()
    {
        if (Manager.Current.menu == Menu.None && IsTarget)
        {
            if (!Manager.Current.LastSelectedPlayer.IsPetrified)
                Manager.Current.LastSelectedPlayer.MoveTo(this);
        }
    }
}