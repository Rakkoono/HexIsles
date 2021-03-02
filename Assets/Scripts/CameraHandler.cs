using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [HideInInspector] public bool zoomAfterMenu = false;
    [HideInInspector] public float zoomAmount = 0;
    [HideInInspector] public float panAmount = 0;

    private void Update()
    {
        if (Manager.UI.currentMenu != UIHandler.Menu.None)
        {
            Camera.main.transform.RotateAround(transform.position, Vector3.up, .1f * Time.deltaTime * Config.Instance.panSpeed);
            ZoomTo(6);
            return;
        }
        else if (zoomAfterMenu)
            ZoomTo(5);

        if (zoomAmount != 0)
        {
            zoomAfterMenu = false;
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize - zoomAmount * Time.deltaTime * Config.Instance.zoomSpeed, Config.Instance.ZoomRangeMin, Config.Instance.ZoomRangeMax);
        }

        if (panAmount != 0)
            Camera.main.transform.RotateAround(transform.position, Vector3.up, -panAmount * Time.deltaTime * Config.Instance.panSpeed);
    }

    private void ZoomTo(int size) => Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, size, Time.deltaTime);
}