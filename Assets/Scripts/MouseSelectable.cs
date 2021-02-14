using System.Linq;
using UnityEngine;

public class MouseSelectable : MonoBehaviour
{
    // Hidden variables
    [HideInInspector]
    public Color initialColor;
    [HideInInspector]
    public Renderer rend;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        initialColor = rend.material.color;
    }

    private void OnMouseEnter()
    {
        // highlight object
        if (MouseAndPlayerHandler.Instance.Selected != this && !MouseAndPlayerHandler.Instance.colored.Contains(this))
            rend.material.SetColor("_Color", initialColor + MouseAndPlayerHandler.Instance.highlightColor);
    }

    private void OnMouseExit()
    {
        // de-highlight object
        if (MouseAndPlayerHandler.Instance.Selected != this && !MouseAndPlayerHandler.Instance.colored.Contains(this))
            ResetColor();
    }

    public void OnMouseDown()
    {
        if (MouseAndPlayerHandler.Instance.Selected != this)
        {
            // deselect last selected object, select this object
            if (MouseAndPlayerHandler.Instance.Selected)
            {
                MouseAndPlayerHandler.Instance.Selected.ResetColor();
                MouseAndPlayerHandler.Instance.Selected.OnDeselect();
            }

            MouseAndPlayerHandler.Instance.Selected = this;
            rend.material.SetColor("_Color", initialColor + MouseAndPlayerHandler.Instance.selectColor);
            OnSelect();
        }
    }

    private void ResetColor()
        => rend.material.SetColor("_Color", initialColor);

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }

}