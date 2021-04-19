using UnityEngine;

public class Sign : MouseSelectable
{
    public Dialog dialog;

    private Vector2Int? position = null;
    private Vector2Int Position => position ??= GridUtility.WorldToGridPos(transform.position);

    public override void OnSelect()
    {
        if (Manager.Current.menu == Menu.None)
        {
            GridUtility.GetFieldAt(Position).ToggleSelect();
            Manager.Current.DialogBox.ShowDialog(dialog);
        }

        Manager.Current.SelectedObject = null;
    }
}
