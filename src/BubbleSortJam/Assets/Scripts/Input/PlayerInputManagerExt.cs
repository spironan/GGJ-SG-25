using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputManagerExt : MonoBehaviour
{
    private static PlayerInputManagerExt instance = null;
    public static PlayerInputManagerExt Instance { get { return instance; } }

    private PlayerInput playerInput;
    private ControlSchemeType currentControlScheme = ControlSchemeType.Invalid;
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
        UpdateCurrentControlSchemeType();
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
            case ControlSchemeType.Touch:
                return "Tap Anywhere";
            default:
                return "Press (" + inputAction.GetBindingDisplayString() + ")";
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

    // called on PlayerInput ControlsChanged Event
    public void UpdateCurrentControlSchemeType()
    {
        if(playerInput == null)
        {
            return;
        }
        currentControlScheme = ConvertControlSchemeType(playerInput.currentControlScheme);
        foreach(IControlSchemeChangeListener listener in controlSchemeChangeListeners)
        {
            listener.OnControlSchemeChanged();
        }
    }

    private ControlSchemeType ConvertControlSchemeType(string currentControlScheme)
    {
        if(currentControlScheme == "Keyboard&Mouse")
        {
            return ControlSchemeType.KeyboardMouse;
        }
        else if(currentControlScheme == "Gamepad")
        {
            return ControlSchemeType.Gamepad;
        }
        else if(currentControlScheme == "Touch")
        {
            return ControlSchemeType.Touch;
        }
        return ControlSchemeType.Invalid;
    }

    private enum ControlSchemeType
    {
        Invalid,
        KeyboardMouse,
        Gamepad,
        Touch
    }

    public interface IControlSchemeChangeListener
    {
        public abstract void OnControlSchemeChanged();
    }
}