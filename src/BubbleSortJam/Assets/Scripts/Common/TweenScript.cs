using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TweenScript : MonoBehaviour
{
    public string enableTweenName;
    public List<Tween> tweenList;

    private List<ActiveTween> activeList;
    private string lastPlayed = "";

    private SpriteRenderer sr;
    private RectTransform rectTransform;
    private Image image;
    private Text text;
    private CanvasGroup canvasGroup;

    private Button button;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        text = GetComponent<Text>();
        canvasGroup = GetComponent<CanvasGroup>();

        button = GetComponent<Button>();

        activeList = new List<ActiveTween>();
    }

    private void OnEnable()
    {
        Play(enableTweenName);
    }

    private void Update()
    {
        if (activeList.Count > 0)
        {
            ProcessActiveTweens();
            if (button != null)
                button.enabled = false;
            if (canvasGroup != null)
                canvasGroup.blocksRaycasts = false;
        }
        else
        {
            if (button != null)
                button.enabled = true;
            if (canvasGroup != null)
                canvasGroup.blocksRaycasts = true;
        }
    }

    #region Public Functions

    public void Play(string tweenName)
    {
        lastPlayed = tweenName;

        for(int i = 0; i < tweenList.Count; i++)
        {
            if (tweenList[i].name == tweenName)
                activeList.Add(new ActiveTween(this, tweenList[i]));
        }
    }

    public void Play(string tweenName, TweenType type, Vector4? from, Vector4? to)
    {
        for (int i = 0; i < tweenList.Count; i++)
        {
            if (tweenList[i].name == tweenName && tweenList[i].value.type == type)
            {
                activeList.Add(new ActiveTween(this, tweenList[i], from, to));
                break;
            }
        }
    }

    public void StopAll()
    {
        for (int i = 0; i < activeList.Count; i++)
        {
            SetValue(activeList[i].type, activeList[i].to);
            activeList[i].onFinished.Invoke();
        }

        activeList = new List<ActiveTween>();
    }

    public bool IsPlaying()
    {
        return activeList.Count > 0;
    }

    #endregion Public Functions

    #region Tween Processing

    private void ProcessActiveTweens()
    {
        for(int i = 0; i < activeList.Count; i++)
        {
            int phase = activeList[i].AddElapsed((activeList[i].ignoreTimeScale) ? Time.unscaledDeltaTime : Time.deltaTime);
            if (phase < 0)
                continue;
            SetValue(activeList[i].type, activeList[i].GetElapsedCurveValue());
            if(phase > 0)
            {
                activeList[i].onFinished.Invoke();
                activeList.RemoveAt(i);
                i--;
            }
        }
    }

    #endregion Tween Processing

    #region GET_SET

    public void SetValue(TweenType type, Vector4 value)
    {
        switch(type)
        {
            case TweenType.Position:
                if (rectTransform != null)
                    rectTransform.anchoredPosition = value;
                else
                    transform.localPosition = value;
                break;
            case TweenType.Rotation:
                transform.localEulerAngles = value;
                break;
            case TweenType.Scale:
                transform.localScale = value;
                break;
            case TweenType.Color:
                if (image != null)
                    image.color = value;
                else if (text != null)
                    text.color = value;
                else if (sr != null)
                    sr.color = value;
                break;
            case TweenType.Alpha:
                if (image != null)
                    image.color = new Color(image.color.r, image.color.g, image.color.b, value.w);
                else if (text != null)
                    text.color = new Color(text.color.r, text.color.g, text.color.b, value.w);
                else if (canvasGroup != null)
                    canvasGroup.alpha = value.w;
                else if (sr != null)
                    sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, value.w);
                break;
            case TweenType.Fill:
                if (image != null)
                    image.fillAmount = value.w;
                break;
        }
    }

    public Vector4 GetValue(TweenType type)
    {
        switch (type)
        {
            case TweenType.Position:
                if (rectTransform != null)
                    return rectTransform.anchoredPosition;
                else
                    return transform.localPosition;
            case TweenType.Rotation:
                return transform.localEulerAngles;
            case TweenType.Scale:
                return transform.localScale;
            case TweenType.Color:
                if (image != null)
                    return image.color;
                else if (text != null)
                    return text.color;
                else if (sr != null)
                    return sr.color;
                return new Vector4();
            case TweenType.Alpha:
                if (image != null)
                    return new Vector4(1f, 1f, 1f, image.color.a);
                else if (text != null)
                    return new Vector4(1f, 1f, 1f, text.color.a);
                else if (canvasGroup != null)
                    return new Vector4(1f, 1f, 1f, canvasGroup.alpha);
                else if (sr != null)
                    return new Vector4(1f, 1f, 1f, sr.color.a);
                return new Vector4();
            case TweenType.Fill:
                if (image != null)
                    return new Vector4(1f, 1f, 1f, image.fillAmount);
                return new Vector4();
        }
        return new Vector4();
    }

    #endregion GET_SET

    #region Misc

    public string GetLastPlayed()
    {
        return lastPlayed;
    }

    #endregion Misc
}

public enum TweenType
{
    Position,
    Rotation,
    Scale,
    Color,
    Alpha,
    Fill
}

[System.Serializable]
public class Tween
{
    public string name;
    public TweenValue value;
    public bool isLoop;
    public AnimationCurve curve;
    public bool ignoreTimeScale;
    public float duration;
    public float delay;
    public UnityEvent onFinished;
}

[System.Serializable]
public class TweenValue
{
    public TweenType type;
    public bool fromUseCurrent;
    public Vector4 from;
    public bool toUseCurrent;
    public Vector4 to;
}

public class ActiveTween
{
    public TweenType type;
    public Vector4 from;
    public Vector4 to;
    public bool isLoop;
    public AnimationCurve curve;
    public bool ignoreTimeScale;
    public float duration;
    public float delay;
    public UnityEvent onFinished;

    private float elapsed;

    public ActiveTween(TweenScript source, Tween tween, Vector4? aFrom = null, Vector4? aTo = null)
    {
        type = tween.value.type;

        if (aFrom != null)
            from = (Vector4)aFrom;
        else if (tween.value.fromUseCurrent)
            from = source.GetValue(type);
        else
            from = tween.value.from;

        if (aTo != null)
            to = (Vector4)aTo;
        else if (tween.value.toUseCurrent)
            to = source.GetValue(type);
        else
            to = tween.value.to;

        isLoop = tween.isLoop;
        curve = tween.curve;
        ignoreTimeScale = tween.ignoreTimeScale;
        duration = tween.duration;
        delay = tween.delay;
        onFinished = tween.onFinished;

        elapsed = 0;
    }

    public int AddElapsed(float t)
    {
        elapsed += t;

        if (elapsed < delay)
            return -1;
        else if (elapsed > duration + delay)
        {
            if (isLoop)
            {
                elapsed = 0f;
                return 0;
            }
            return 1;
        }
        return 0;
    }

    public Vector4 GetElapsedCurveValue()
    {
        float t = curve.Evaluate((elapsed - delay) / duration);

        if (t < 0)
            return Vector4.Lerp(from, -to, -t);
        return Vector4.Lerp(from, to, t);
    }
}