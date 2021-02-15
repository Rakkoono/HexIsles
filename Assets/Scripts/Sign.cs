using UnityEngine;

public class Sign : MouseSelectable
{
    [SerializeField, TextArea]
    private string message;
    public override void OnSelect()
    {
        HexGrid.GetFieldAt(HexGrid.WorldToGridPos(transform.position)).OnMouseDown();
        if (GameManager.instance.dialogueMSG == message)
            GameManager.instance.HideDialog();
        else
            GameManager.instance.ShowDialog(message);
    }
}
