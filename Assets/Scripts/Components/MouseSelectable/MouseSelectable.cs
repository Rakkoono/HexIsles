using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Collider)), RequireComponent(typeof(Renderer))]
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

    private void Awake()
    {
        Renderer = GetComponent<Renderer>();
        Color = Renderer.material.color;
    }

#if !UNITY_ANDROID && !UNITY_IOS
    // Exclude highlighting on mobile
    void IPointerEnterHandler.OnPointerEnter(PointerEventData data)
    {
        // highlight object
        if (Manager.Instance.SelectedObject != this && !(Manager.Instance.SelectedPlayer && Manager.Instance.validTargets.Contains(this)))
            Renderer.material.color = Color + Config.HighlightTint;
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData data)
    {
        // de-highlight object
        if (Manager.Instance.SelectedObject != this && !(Manager.Instance.SelectedPlayer && Manager.Instance.validTargets.Contains(this)))
            ResetColor();
    }
#endif

    void IPointerClickHandler.OnPointerClick(PointerEventData data) => ToggleSelect();

    public void ToggleSelect() => Manager.Instance.SelectedObject = Manager.Instance.SelectedObject == this ? null : this;

    public void ResetColor() => Renderer.material.color = Color;

    public virtual void OnSelect() { }
    public virtual void OnDeselect() { }
}