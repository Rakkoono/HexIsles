using UnityEngine;
using UnityEngine.InputSystem;

public class CameraHandler : MonoBehaviour
{
    [HideInInspector] public bool zoomAfterMenu = false;
    [HideInInspector] public bool startPinch = false;
    [HideInInspector] public bool pinch = false;
    [HideInInspector] public float zoomAmount = 0;
    [HideInInspector] public float panAmount = 0;

    private Vector2 PrimaryTouchPosition => Touchscreen.current.primaryTouch.position.ReadValue();
    private Vector2 SecondaryTouchPosition => Touchscreen.current.touches[1].position.ReadValue();
    float previousPinchDistance = 0f;

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
        if (pinch || startPinch)
        {
            float pinchDistance = Vector2.Distance(PrimaryTouchPosition, SecondaryTouchPosition);
            if (startPinch)
                previousPinchDistance = pinchDistance;
            
            ZoomTo(Camera.main.orthographicSize + (previousPinchDistance - pinchDistance) * .1f * Config.Instance.zoomSpeed);

            previousPinchDistance = pinchDistance;
        }
        else if (panAmount > .5f || panAmount < -.5f)
            Camera.main.transform.RotateAround(transform.position, Vector3.up, -panAmount * Time.deltaTime * Config.Instance.panSpeed);
        
        if (startPinch) 
        {
            pinch = true;
            startPinch = zoomAfterMenu = false;
        }
    }

    private void ZoomTo(float size, float speed = 1) => Camera.main.orthographicSize = Mathf.Clamp(Mathf.Lerp(Camera.main.orthographicSize, size, Time.deltaTime * speed), Config.Instance.ZoomRangeMin, Config.Instance.ZoomRangeMax);    
}