using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManagerExt : MonoBehaviour
{
    private static PlayerInputManagerExt instance = null;
    public static PlayerInputManagerExt Instance { get { return instance; } }

    private PlayerInput playerInput;
    private static ControlSchemeType currentControlScheme = ControlSchemeType.Invalid;
    private List<IControlSchemeChangeListener> controlSchemeChangeListeners = new List<IControlSchemeChangeListener>();

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if(instance != this)
        {
            Destroy(this);
        }

        playerInput = GetComponent<PlayerInput>();
        InitializeCurrentControlSchemeType();
    }

    public string GetInputActionString(string actionName)
    {
        if(playerInput == null)
        {
            return "";
        }

        InputAction inputAction = playerInput.actions.FindAction(actionName);
        if (inputAction == null)
        {
            Debug.LogError("no action of name " + actionName + "found");
            return "";
        }

        switch(currentControlScheme)
        {
            case ControlSchemeType.Invalid:
                return "Press (UNDEFINED)";
            case ControlSchemeType.Touch:
                return "Tap Anywhere";
            case ControlSchemeType.Mouse:
                return "Click Anywhere";
            case ControlSchemeType.Gamepad:
                return "Press (" + inputAction.GetBindingDisplayString() + ")";
            case ControlSchemeType.Keyboard:
                return "Press <" + inputAction.GetBindingDisplayString() + ">";
            default:
                return "Press (UNDEFINED)";
        }
    }

    public void AddControlSchemeChangeListener(IControlSchemeChangeListener listener)
    {
        controlSchemeChangeListeners.Add(listener);
        listener.OnControlSchemeChanged();
    }

    public void RemoveControlSchemeChangeListener(IControlSchemeChangeListener listener)
    {
        controlSchemeChangeListeners.Remove(listener);
    }

    private void InitializeCurrentControlSchemeType()
    {
        if(currentControlScheme == ControlSchemeType.Invalid)
        {
            // first time, set based on this preference, Gamepad, Touchscreen, Keyboard & Mouse
            InputDevice activeDevice = GetActiveInputDevice();
            if(activeDevice != null)
            {
                playerInput.SwitchCurrentControlScheme(activeDevice);
            }
        }
        else
        {
            // already set in previous scene, attempt to switch to that
            InputDevice currentDevice = GetInputDeviceFromControlSchemeType(currentControlScheme);
            if(currentDevice != null)
            {
                playerInput.SwitchCurrentControlScheme(currentDevice);
            }
        }

        UpdateCurrentControlSchemeType();
    }

    // called on PlayerInput ControlsChanged Event
    public void UpdateCurrentControlSchemeType()
    {
        if(playerInput == null)
        {
            return;
        }
        Debug.Log(playerInput.currentControlScheme);
        currentControlScheme = ConvertControlSchemeType(playerInput.currentControlScheme);

        foreach(IControlSchemeChangeListener listener in controlSchemeChangeListeners)
        {
            listener.OnControlSchemeChanged();
        }
    }

    private ControlSchemeType ConvertControlSchemeType(string currentControlScheme)
    {
        if (currentControlScheme == "Gamepad")
        {
            return ControlSchemeType.Gamepad;
        }
        else if(currentControlScheme == "Keyboard")
        {
            return ControlSchemeType.Keyboard;
        }
        else if(currentControlScheme == "Mouse")
        {
            return ControlSchemeType.Mouse;
        }
        else if(currentControlScheme == "Touch")
        {
            return ControlSchemeType.Touch;
        }
        return ControlSchemeType.Invalid;
    }

    private InputDevice GetActiveInputDevice()
    {
        if (Gamepad.current != null)
        {
            return Gamepad.current;
        }
        else if (Touchscreen.current != null)
        {
            return Touchscreen.current;
        }
        else if (Keyboard.current != null)
        {
            return Keyboard.current;
        }
        else if (Mouse.current != null)
        {
            return Mouse.current;
        }
        return null;
    }

    private InputDevice GetInputDeviceFromControlSchemeType(ControlSchemeType type)
    {
        switch(type)
        {
            case ControlSchemeType.Gamepad:
                return Gamepad.current;
            case ControlSchemeType.Touch:
                return Touchscreen.current;
            case ControlSchemeType.Keyboard:
                return Keyboard.current;
            case ControlSchemeType.Mouse:
                return Mouse.current;
            default:
                return null;
        }
    }

    private enum ControlSchemeType
    {
        Invalid,
        Keyboard,
        Mouse,
        Gamepad,
        Touch
    }

    public interface IControlSchemeChangeListener
    {
        public abstract void OnControlSchemeChanged();
    }
}