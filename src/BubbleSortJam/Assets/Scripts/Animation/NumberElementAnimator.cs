using TMPro;
using UnityEngine;

public class NumberElementAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI numberText;

    private int value = 0;

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        numberText.text = value.ToString();
    }
}
