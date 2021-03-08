using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseSelectable
    : MonoBehaviour
    , IPointerClickHandler
#if !UNITY_ANDROID && !UNITY_IOS
    // Exclude unused Interfaces on mobile
    , IPointerEnterHandler
    , IPointerExitHandler
#endif
{
    public Renderer Renderer { get; private set; }
    public Color InitialColor { get; private set; }

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        InitialColor = Renderer.material.color;
    }

#if !UNITY_ANDROID && !UNITY_IOS
    // Exclude highlighting on mobile
    void IPointerEnterHandler.OnPointerEnter(PointerEventData data)
    {
        // highlight object
        if (Manager.Players.SelectedObject != this && !(Manager.Players.selected && Manager.Players.possibleMoves.Contains(this)))
            Renderer.material.color = InitialColor + Config.Current.HighlightTint;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData data)
    {
        // de-highlight object
        if (Manager.Players.SelectedObject != this && !(Manager.Players.selected && Manager.Players.possibleMoves.Contains(this)))
            ResetColor();
    }

#endif
    void IPointerClickHandler.OnPointerClick(PointerEventData data) => ToggleSelect();
    
    public void ToggleSelect() => Manager.Players.SelectedObject = Manager.Players.SelectedObject == this ? null : this;

    public void ResetColor() => Renderer.material.color = InitialColor;

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }
}