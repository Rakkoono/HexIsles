using UnityEngine;

public class MouseInteractionHandler : MonoBehaviour
{

    // Serialized Variables
    [SerializeField]
    public Transform rotationCenter;
    [SerializeField, Range(0, 100)]
    private float rotationSpeed = 50f;
    [Space, SerializeField, Range(0, 100)]
    private float zoomSpeed = 50f;
    [SerializeField]
    private Color highlightColor = new Color(40, 40, 40, 0), selectColor = new Color(80, 80, 80, 0);

    // Private Variables
    private Vector3? mousePosition = new Vector3();
    private Transform lastHighlighted, lastSelected;
    private Renderer lastHighlightedRenderer, lastSelectedRenderer;
    private Color highlightInitialColor, selectInitialColor;

    void Update()
    {

        if (Input.GetMouseButton(1))
        {
            if (mousePosition != null && mousePosition != Input.mousePosition)
            {
                float mouseMovement = Input.mousePosition.x - ((Vector3)mousePosition).x;
                Camera.main.transform.RotateAround(rotationCenter.position, Vector3.up, mouseMovement * Time.deltaTime * rotationSpeed);
            }
            mousePosition = Input.mousePosition;
        }
        else mousePosition = null;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, LayerMask.GetMask("MouseSelectable")))
        {
            if (Input.GetMouseButtonDown(0) && hit.transform != lastSelected)
            {
                if (lastSelected) lastSelectedRenderer.material.SetColor("_Color", selectInitialColor);
                lastSelected = hit.transform;
                lastSelectedRenderer = lastSelected.GetComponent<Renderer>();

                selectInitialColor = highlightInitialColor;
                lastSelected = lastHighlighted;
                lastSelectedRenderer = lastHighlightedRenderer;
                lastHighlighted = null;

                lastSelectedRenderer.material.SetColor("_Color", selectInitialColor + selectColor);
            }
            else if (hit.transform != lastHighlighted)
            {
                if (lastHighlighted) lastHighlightedRenderer.material.SetColor("_Color", highlightInitialColor);
                lastHighlighted = hit.transform;
                lastHighlightedRenderer = lastHighlighted.GetComponent<Renderer>();

                highlightInitialColor = lastHighlightedRenderer.material.GetColor("_Color");
                lastHighlightedRenderer.material.SetColor("_Color", highlightInitialColor + highlightColor);
            }
        }
        else
        {
            if (lastHighlighted) lastHighlightedRenderer.material.SetColor("_Color", highlightInitialColor);
            lastHighlighted = null;
            if (Input.GetMouseButtonDown(0))
                lastSelectedRenderer.material.SetColor("_Color", selectInitialColor);
        }

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float size = Camera.main.orthographicSize + -Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomSpeed * 4;
            if (size > 8) size = 8;
            else if (size < 1) size = 1;
            Camera.main.orthographicSize = size;
        }
    }
}
