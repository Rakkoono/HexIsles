using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // Serialized variables
    public Transform rotationCenter;
    [SerializeField, Range(0, 100)]
    private float rotationSpeed = 50f;
    [Space, SerializeField, Range(0, 100)]
    private float zoomSpeed = 50f;

    // Hidden variables
    private Vector3? mousePosition = new Vector3();

    void Update()
    {
        if (Manager.UI.inMenu)
        {
            Camera.main.transform.RotateAround(rotationCenter.position, Vector3.up, .1f * Time.deltaTime * rotationSpeed);
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 6, Time.deltaTime);
            return;
        }
        else if (Manager.UI.zoomAfterMenu)
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, 5, Time.deltaTime);

        // Pan on right mouse button
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

        // Zoom on mouse wheel and Arrow keys
        if (Input.GetAxis("Mouse ScrollWheel") != 0 || Input.GetAxis("Vertical") != 0)
        {
            Manager.UI.zoomAfterMenu = false;
            float axis = Input.GetAxis("Mouse ScrollWheel") != 0 ? Input.GetAxis("Mouse ScrollWheel") : Input.GetAxis("Vertical") / 5;
            float size = Camera.main.orthographicSize + -axis * Time.deltaTime * zoomSpeed * 4;
            if (size > 8) size = 8;
            else if (size < 1) size = 1;
            Camera.main.orthographicSize = size;
        }
    }
}
