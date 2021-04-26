using UnityEngine;

public class Sign : MouseSelectable
{
    public Dialog dialog;

    private Vector2Int? position = null;
    private Vector2Int Position => position ??= GridUtility.WorldToGridPos(transform.position);

    public override void OnSelect()
    {
        if (!Menu.Current.StopGameplay)
        {
            GridUtility.GetFieldAt(Position).ToggleSelect();
            Manager.DialogBox.Open(dialog);
        }

        Manager.Instance.SelectedObject = null;
    }
}
