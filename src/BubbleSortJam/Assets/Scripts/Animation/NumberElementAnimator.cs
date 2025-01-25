using System.Collections;
using TMPro;
using UnityEngine;

public class NumberElementAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI NumberText;
    [SerializeField] private SpriteRenderer Highlight;

    [Header("Animation Settings")]
    [SerializeField] private SimpleAnimationData SwapStartAnimData;
    [SerializeField] private SimpleAnimationData SwapEndAnimData;

    private int value = 0;

    public void SetValue(int newValue)
    {
        value = newValue;
        UpdateVisuals();
    }

    public int GetValue()
    {
        return value;
    }

    public void PlaySwapAnimation(bool isLHS, int value) 
    {
        SwapStartAnimData.EndValue = SwapEndAnimData.StartValue = Mathf.Abs(SwapStartAnimData.EndValue) * (isLHS ? 1 : -1);
        StartCoroutine(SwapAnimationCoroutine(value));
    }

    private IEnumerator SwapAnimationCoroutine(int newValue)
    {
        float elapsed = 0.0f;
        while(elapsed < SwapStartAnimData.Duration)
        {
            float posValue = Mathf.Lerp(SwapStartAnimData.StartValue, SwapStartAnimData.EndValue, SwapStartAnimData.Curve.Evaluate(elapsed / SwapStartAnimData.Duration));
            NumberText.rectTransform.anchoredPosition = new Vector2(posValue, NumberText.rectTransform.anchoredPosition.y);

            yield return null;
            elapsed += Time.deltaTime;
        }

        SetValue(newValue);

        elapsed = 0.0f;
        while (elapsed < SwapEndAnimData.Duration)
        {
            float posValue = Mathf.Lerp(SwapEndAnimData.StartValue, SwapEndAnimData.EndValue, SwapEndAnimData.Curve.Evaluate(elapsed / SwapEndAnimData.Duration));
            NumberText.rectTransform.anchoredPosition = new Vector2(posValue, NumberText.rectTransform.anchoredPosition.y);

            yield return null;
            elapsed += Time.deltaTime;
        }

        float endPosValue = Mathf.Lerp(SwapEndAnimData.StartValue, SwapEndAnimData.EndValue, SwapEndAnimData.Curve.Evaluate(1.0f));
        NumberText.rectTransform.anchoredPosition = new Vector2(endPosValue, NumberText.rectTransform.anchoredPosition.y);
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
