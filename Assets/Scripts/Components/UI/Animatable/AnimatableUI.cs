using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimatableUI : MonoBehaviour
{
    private Animator animator;
    protected Animator Animator => animator ??= GetComponent<Animator>();

    protected virtual void Awake() => gameObject.SetActive(false);

    public virtual void Show() => gameObject.SetActive(true);

    public virtual void Hide()
    {
        if (gameObject.activeSelf)
            Animator.Play("Out");
    }

    // Execute after "Out" Animation
    public void Deactivate() => gameObject.SetActive(false);
}
