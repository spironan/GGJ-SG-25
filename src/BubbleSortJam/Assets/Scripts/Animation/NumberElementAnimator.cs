using TMPro;
using UnityEngine;

public class NumberElementAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI NumberText;
    [SerializeField] private SpriteRenderer Highlight;

    private int value = 0;

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        NumberText.text = value.ToString();
    }

    private void SetColor(Color color)
    {
        Highlight.color = color;
        NumberText.color = color;
    }
}
