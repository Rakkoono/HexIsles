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
    public Color Color { get; protected set; }

    private void Awake() {
        Renderer = GetComponent<Renderer>();
        Color = Renderer.material.color;
    }

#if !UNITY_ANDROID && !UNITY_IOS
    // Exclude highlighting on mobile
    void IPointerEnterHandler.OnPointerEnter(PointerEventData data)
    {
        // highlight object
        if (Manager.Current.SelectedObject != this && !(Manager.Current.SelectedPlayer && Manager.Current.validTargets.Contains(this)))
            Renderer.material.color = Color + Config.Current.HighlightTint;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData data)
    {
        // de-highlight object
        if (Manager.Current.SelectedObject != this && !(Manager.Current.SelectedPlayer && Manager.Current.validTargets.Contains(this)))
            ResetColor();
    }
#endif

    void IPointerClickHandler.OnPointerClick(PointerEventData data) => ToggleSelect();
    
    public void ToggleSelect() => Manager.Current.SelectedObject = Manager.Current.SelectedObject == this ? null : this;

    public void ResetColor() => Renderer.material.color = Color;

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }
}