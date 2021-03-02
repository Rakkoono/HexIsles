using System.Linq;
using UnityEngine;

public class MouseSelectable : MonoBehaviour
{
    public Renderer Renderer {get; private set; }
    public Color InitialColor { get; private set; }

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        InitialColor = Renderer.material.color;
    }

    private void OnMouseEnter()
    {
        // highlight object
        if (Manager.Players && Manager.Players.SelectedObject != this && !Manager.Players.possibleMoves.Contains(this))
            Renderer.material.color = InitialColor + Manager.Players.highlightTint;
    }

    private void OnMouseExit()
    {
        // de-highlight object
        if (Manager.Players.SelectedObject != this && !Manager.Players.possibleMoves.Contains(this))
            ResetMaterial();
    }

    private void OnMouseDown() => ToggleSelect();

    public void ToggleSelect() => Manager.Players.SelectedObject = Manager.Players.SelectedObject == this ? null : this;

    public void ResetMaterial()
        => Renderer.material.color = InitialColor;

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }

}