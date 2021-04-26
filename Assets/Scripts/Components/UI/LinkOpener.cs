using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_Text))]
public class LinkOpener : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text tmp = GetComponent<TMP_Text>();
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(tmp, eventData.position, null);

        if (linkIndex != -1)
            Application.OpenURL(tmp.textInfo.linkInfo[linkIndex].GetLinkID());
    }
}