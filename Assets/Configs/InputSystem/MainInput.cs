// GENERATED AUTOMATICALLY FROM 'Assets/Scripts/Input Actions/MainInput.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @MainInput : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @MainInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""MainInput"",
    ""maps"": [
        {
            ""name"": ""Hotkeys"",
            ""id"": ""b3e84447-c3b8-4ae8-9276-05fde01324ce"",
            ""actions"": [
                {
                    ""name"": ""Menu"",
                    ""type"": ""Button"",
                    ""id"": ""4be3a2c7-3367-4922-9431-1b3dea3ad1e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Undo"",
                    ""type"": ""Button"",
                    ""id"": ""3d4d8f6c-034d-410e-a641-8f47b6be448a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Restart"",
                    ""type"": ""Button"",
                    ""id"": ""8c97d3ce-ec21-4fa3-b451-af69e1823b8f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""UnlockAllLevels"",
                    ""type"": ""Button"",
                    ""id"": ""e059f504-714b-4c96-b6b7-5b98ad29c270"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""1201fce6-97c7-489f-8eef-28ed053e36e4"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""edb09479-9009-4d25-8fec-7311a4b1136b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Menu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""05ae26da-a670-4b92-bc54-da9473ab60a4"",
                    ""path"": ""<Keyboard>/z"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Undo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""d2c7d621-a2eb-4f73-b2e2-cb9ae09100d8"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Restart"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Alt + Q [Keyboard]"",
                    ""id"": ""70ebeda5-5e93-4259-bd3b-19e69765ee33"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""UnlockAllLevels"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""b8dcd9d9-d3fc-4554-8065-02af5578016f"",
                    ""path"": ""<Keyboard>/alt"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""UnlockAllLevels"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""c07db482-0dd5-47af-a66d-d3688b6b273b"",
                    ""path"": ""<Keyboard>/q"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""UnlockAllLevels"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                }
            ]
        },
        {
            ""name"": ""Camera"",
            ""id"": ""2965d313-a1a3-4949-a74f-a43e05757860"",
            ""actions"": [
                {
                    ""name"": ""Zoom"",
                    ""type"": ""PassThrough"",
                    ""id"": ""624dab28-0341-4601-a5e1-111beca2364d"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pan"",
                    ""type"": ""PassThrough"",
                    ""id"": ""d4e4bf67-affd-4b8c-a0bc-e022f12c4439"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Pinch"",
                    ""type"": ""Button"",
                    ""id"": ""98c8b55b-87e0-4daa-b952-80ceb20e9584"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""11075ca7-004a-4612-a872-ff6f1ed47f06"",
                    ""path"": ""<Mouse>/scroll/y"",
                    ""interactions"": """",
                    ""processors"": ""Clamp(min=-5,max=5)"",
                    ""groups"": """",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Arrow Keys: Vertical [Keyboard]"",
                    ""id"": ""75fa872b-af5d-4ec5-8896-d4638b3b88f7"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Zoom"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""395f0be4-7eff-4b5f-9391-ce0e83db5b38"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""bd6c96a7-0390-4f3c-9241-a0e5eb60a6ac"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Zoom"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Arrow Keys: Horizontal [Keyboard]"",
                    ""id"": ""d0c0d57b-f51a-4f40-8257-888879291369"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pan"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""32ee6b1d-aa44-4a35-b4f6-83f4a8e53b01"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""12587867-20dd-4c9d-bfd5-817a002585f2"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Right Mouse + Drag"",
                    ""id"": ""4bff6f7a-3f74-40d7-8190-3b001203ed3d"",
                    ""path"": ""ButtonWithOneModifier"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pan"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""modifier"",
                    ""id"": ""155df45c-83b3-46ab-8947-253ae7df0aed"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""button"",
                    ""id"": ""909268bb-a3f6-427c-8c4d-a0d29facdaf5"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": ""Invert"",
                    ""groups"": ""KeyboardMouse"",
                    ""action"": ""Pan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""239a1ebf-aa09-49fb-89f7-6bf01e9e65f6"",
                    ""path"": ""<Touchscreen>/delta/x"",
                    ""interactions"": """",
                    ""processors"": ""Invert,Scale(factor=0.1)"",
                    ""groups"": ""Touch"",
                    ""action"": ""Pan"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9296c09b-3f43-4dd6-b16b-886f531fff07"",
                    ""path"": ""<Touchscreen>/touch1/press"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch"",
                    ""action"": ""Pinch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Movement"",
            ""id"": ""1d685757-b20e-41d1-b8e6-b2635bc09d94"",
            ""actions"": [
                {
                    ""name"": ""Select"",
                    ""type"": ""Button"",
                    ""id"": ""9139ff19-99c9-4435-8d8b-89147faeb0be"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""da8348cc-81b0-4254-b6e2-b5abae1c4022"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""KeyboardMouse;Touch"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92547f7e-dd26-4bcc-b533-4358c072f093"",
                    ""path"": ""<Touchscreen>/primaryTouch/tap"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Touch;KeyboardMouse"",
                    ""action"": ""Select"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""KeyboardMouse"",
            ""bindingGroup"": ""KeyboardMouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Touch"",
            ""bindingGroup"": ""Touch"",
            ""devices"": [
                {
                    ""devicePath"": ""<Touchscreen>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Hotkeys
        m_Hotkeys = asset.FindActionMap("Hotkeys", throwIfNotFound: true);
        m_Hotkeys_Menu = m_Hotkeys.FindAction("Menu", throwIfNotFound: true);
        m_Hotkeys_Undo = m_Hotkeys.FindAction("Undo", throwIfNotFound: true);
        m_Hotkeys_Restart = m_Hotkeys.FindAction("Restart", throwIfNotFound: true);
        m_Hotkeys_UnlockAllLevels = m_Hotkeys.FindAction("UnlockAllLevels", throwIfNotFound: true);
        // Camera
        m_Camera = asset.FindActionMap("Camera", throwIfNotFound: true);
        m_Camera_Zoom = m_Camera.FindAction("Zoom", throwIfNotFound: true);
        m_Camera_Pan = m_Camera.FindAction("Pan", throwIfNotFound: true);
        m_Camera_Pinch = m_Camera.FindAction("Pinch", throwIfNotFound: true);
        // Movement
        m_Movement = asset.FindActionMap("Movement", throwIfNotFound: true);
        m_Movement_Select = m_Movement.FindAction("Select", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Hotkeys
    private readonly InputActionMap m_Hotkeys;
    private IHotkeysActions m_HotkeysActionsCallbackInterface;
    private readonly InputAction m_Hotkeys_Menu;
    private readonly InputAction m_Hotkeys_Undo;
    private readonly InputAction m_Hotkeys_Restart;
    private readonly InputAction m_Hotkeys_UnlockAllLevels;
    public struct HotkeysActions
    {
        private @MainInput m_Wrapper;
        public HotkeysActions(@MainInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Menu => m_Wrapper.m_Hotkeys_Menu;
        public InputAction @Undo => m_Wrapper.m_Hotkeys_Undo;
        public InputAction @Restart => m_Wrapper.m_Hotkeys_Restart;
        public InputAction @UnlockAllLevels => m_Wrapper.m_Hotkeys_UnlockAllLevels;
        public InputActionMap Get() { return m_Wrapper.m_Hotkeys; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(HotkeysActions set) { return set.Get(); }
        public void SetCallbacks(IHotkeysActions instance)
        {
            if (m_Wrapper.m_HotkeysActionsCallbackInterface != null)
            {
                @Menu.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnMenu;
                @Menu.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnMenu;
                @Menu.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnMenu;
                @Undo.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnUndo;
                @Undo.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnUndo;
                @Undo.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnUndo;
                @Restart.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRestart;
                @Restart.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRestart;
                @Restart.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnRestart;
                @UnlockAllLevels.started -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnUnlockAllLevels;
                @UnlockAllLevels.performed -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnUnlockAllLevels;
                @UnlockAllLevels.canceled -= m_Wrapper.m_HotkeysActionsCallbackInterface.OnUnlockAllLevels;
            }
            m_Wrapper.m_HotkeysActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Menu.started += instance.OnMenu;
                @Menu.performed += instance.OnMenu;
                @Menu.canceled += instance.OnMenu;
                @Undo.started += instance.OnUndo;
                @Undo.performed += instance.OnUndo;
                @Undo.canceled += instance.OnUndo;
                @Restart.started += instance.OnRestart;
                @Restart.performed += instance.OnRestart;
                @Restart.canceled += instance.OnRestart;
                @UnlockAllLevels.started += instance.OnUnlockAllLevels;
                @UnlockAllLevels.performed += instance.OnUnlockAllLevels;
                @UnlockAllLevels.canceled += instance.OnUnlockAllLevels;
            }
        }
    }
    public HotkeysActions @Hotkeys => new HotkeysActions(this);

    // Camera
    private readonly InputActionMap m_Camera;
    private ICameraActions m_CameraActionsCallbackInterface;
    private readonly InputAction m_Camera_Zoom;
    private readonly InputAction m_Camera_Pan;
    private readonly InputAction m_Camera_Pinch;
    public struct CameraActions
    {
        private @MainInput m_Wrapper;
        public CameraActions(@MainInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Zoom => m_Wrapper.m_Camera_Zoom;
        public InputAction @Pan => m_Wrapper.m_Camera_Pan;
        public InputAction @Pinch => m_Wrapper.m_Camera_Pinch;
        public InputActionMap Get() { return m_Wrapper.m_Camera; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(CameraActions set) { return set.Get(); }
        public void SetCallbacks(ICameraActions instance)
        {
            if (m_Wrapper.m_CameraActionsCallbackInterface != null)
            {
                @Zoom.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                @Zoom.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                @Zoom.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnZoom;
                @Pan.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPan;
                @Pan.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPan;
                @Pan.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPan;
                @Pinch.started -= m_Wrapper.m_CameraActionsCallbackInterface.OnPinch;
                @Pinch.performed -= m_Wrapper.m_CameraActionsCallbackInterface.OnPinch;
                @Pinch.canceled -= m_Wrapper.m_CameraActionsCallbackInterface.OnPinch;
            }
            m_Wrapper.m_CameraActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Zoom.started += instance.OnZoom;
                @Zoom.performed += instance.OnZoom;
                @Zoom.canceled += instance.OnZoom;
                @Pan.started += instance.OnPan;
                @Pan.performed += instance.OnPan;
                @Pan.canceled += instance.OnPan;
                @Pinch.started += instance.OnPinch;
                @Pinch.performed += instance.OnPinch;
                @Pinch.canceled += instance.OnPinch;
            }
        }
    }
    public CameraActions @Camera => new CameraActions(this);

    // Movement
    private readonly InputActionMap m_Movement;
    private IMovementActions m_MovementActionsCallbackInterface;
    private readonly InputAction m_Movement_Select;
    public struct MovementActions
    {
        private @MainInput m_Wrapper;
        public MovementActions(@MainInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Select => m_Wrapper.m_Movement_Select;
        public InputActionMap Get() { return m_Wrapper.m_Movement; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(MovementActions set) { return set.Get(); }
        public void SetCallbacks(IMovementActions instance)
        {
            if (m_Wrapper.m_MovementActionsCallbackInterface != null)
            {
                @Select.started -= m_Wrapper.m_MovementActionsCallbackInterface.OnSelect;
                @Select.performed -= m_Wrapper.m_MovementActionsCallbackInterface.OnSelect;
                @Select.canceled -= m_Wrapper.m_MovementActionsCallbackInterface.OnSelect;
            }
            m_Wrapper.m_MovementActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Select.started += instance.OnSelect;
                @Select.performed += instance.OnSelect;
                @Select.canceled += instance.OnSelect;
            }
        }
    }
    public MovementActions @Movement => new MovementActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("KeyboardMouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    private int m_TouchSchemeIndex = -1;
    public InputControlScheme TouchScheme
    {
        get
        {
            if (m_TouchSchemeIndex == -1) m_TouchSchemeIndex = asset.FindControlSchemeIndex("Touch");
            return asset.controlSchemes[m_TouchSchemeIndex];
        }
    }
    public interface IHotkeysActions
    {
        void OnMenu(InputAction.CallbackContext context);
        void OnUndo(InputAction.CallbackContext context);
        void OnRestart(InputAction.CallbackContext context);
        void OnUnlockAllLevels(InputAction.CallbackContext context);
    }
    public interface ICameraActions
    {
        void OnZoom(InputAction.CallbackContext context);
        void OnPan(InputAction.CallbackContext context);
        void OnPinch(InputAction.CallbackContext context);
    }
    public interface IMovementActions
    {
        void OnSelect(InputAction.CallbackContext context);
    }
}
