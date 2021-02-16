using UnityEngine;

public class Sign : MouseSelectable
{
    [TextArea]
    public string[] dialog;
    public override void OnSelect()
    {
        HexGrid.GetFieldAt(HexGrid.WorldToGridPos(transform.position)).OnMouseDown();
        GameManager.instance.ShowDialog(this);
    }
}
