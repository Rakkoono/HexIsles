using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private MainInput input;

    private void Awake() => input = new MainInput();

    private void OnEnable() => input.Enable();
    private void OnDisable() => input.Disable();

    void Start()
    {
        input.Hotkeys.Menu.performed += _ => Manager.UI.ToggleMainMenu();
        input.Hotkeys.Restart.performed += _ => Manager.Levels.LoadCurrent();
        input.Hotkeys.Undo.performed += _ => Manager.Players.Undo();
        
        // Cheat code only in debug builds
        if (Debug.isDebugBuild)
            input.Hotkeys.UnlockAllLevels.performed += _ => Manager.Levels.UnlockAll();

        input.Camera.Zoom.performed += ctx => Manager.Camera.zoomAmount = ctx.ReadValue<float>();
        input.Camera.Zoom.canceled += _ => Manager.Camera.zoomAmount = 0f;

        input.Camera.Pan.performed += ctx => Manager.Camera.panAmount = ctx.ReadValue<float>();
        input.Camera.Pan.canceled += _ => Manager.Camera.panAmount = 0f;

        input.Camera.Pinch.started += _ => Manager.Camera.startPinch = true;
        input.Camera.Pinch.canceled += _ => Manager.Camera.pinch = false;
    }
}