using TMPro;
using UnityEngine;
using static PlayerInputManagerExt;

[RequireComponent(typeof(TextMeshProUGUI))]
public class PlayerInputTextUI : MonoBehaviour, IControlSchemeChangeListener
{
    [Header("Settings")]
    [SerializeField] private string actionName;
    [SerializeField] private string prefix;
    [SerializeField] private string suffix;

    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        Instance.AddControlSchemeChangeListener(this);
    }

    private void OnDestroy()
    {
        Instance.RemoveControlSchemeChangeListener(this);
    }

    public void OnControlSchemeChanged()
    {
        text.text = ((prefix.Length > 0) ? (prefix + " ") : "")
                    + Instance.GetInputActionString(actionName)
                    + ((suffix.Length > 0) ? (" " + suffix) : "");
    }
}
