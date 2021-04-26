using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform rotationCenter;

    [HideInInspector] public bool zoomAfterMenu = false;
    [HideInInspector] public bool startPinch = false;
    [HideInInspector] public bool pinch = false;
    [HideInInspector] public float zoomAmount = 0;
    [HideInInspector] public float panAmount = 0;

    private Vector2 PrimaryTouchPosition => Touchscreen.current.primaryTouch.position.ReadValue();
    private Vector2 SecondaryTouchPosition => Touchscreen.current.touches[1].position.ReadValue();
    float previousPinchDistance = 0f;

    private Camera cam;

    private void Start() => cam = GetComponent<Camera>();

    private void Update()
    {
        if (Menu.Current.StopGameplay)
        {
            zoomAfterMenu = true;
            transform.RotateAround(rotationCenter.position, Vector3.up, .1f * Time.deltaTime * Config.PanSpeed);
            ZoomTo(6);
            return;
        }
        else if (zoomAfterMenu)
            ZoomTo(4);

        if (zoomAmount != 0)
        {
            zoomAfterMenu = false;
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomAmount * Time.deltaTime * Config.ZoomSpeed, Config.ZoomRangeMin, Config.ZoomRangeMax);
        }

        if (pinch || startPinch)
        {
            float pinchDistance = Vector2.Distance(PrimaryTouchPosition, SecondaryTouchPosition);
            if (startPinch)
                previousPinchDistance = pinchDistance;

            ZoomTo(cam.orthographicSize + (previousPinchDistance - pinchDistance) * .1f * Config.ZoomSpeed);

            previousPinchDistance = pinchDistance;
        }
        else if (panAmount > .5f || panAmount < -.5f)
            transform.RotateAround(rotationCenter.position, Vector3.up, -panAmount * Time.deltaTime * Config.PanSpeed);

        if (startPinch)
        {
            pinch = true;
            startPinch = zoomAfterMenu = false;
        }
    }

    private void ZoomTo(float size, float speed = 1) => cam.orthographicSize = Mathf.Clamp(Mathf.Lerp(cam.orthographicSize, size, Time.deltaTime * speed), Config.ZoomRangeMin, Config.ZoomRangeMax);
}