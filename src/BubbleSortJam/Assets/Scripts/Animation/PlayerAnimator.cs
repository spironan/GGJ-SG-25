using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer Circle1;
    [SerializeField] private SpriteRenderer Circle2;
    [SerializeField] private SpriteRenderer Line;

    [Header("Animations")]
    [SerializeField] private List<PlayerAnimationPreset> Presets = new List<PlayerAnimationPreset>();

    [Header("Current Animator Values")]
    private float currentDistance = 2.0f;
    private float currentRotationAngle = 0.0f;
    private bool is1CurrentPivot = true;

    private Queue<PlayerAnimation> currentAnimationQueue = new Queue<PlayerAnimation>();
    private PlayerAnimation currentAnimation = null;
    private float currentAnimationElapsed = 0.0f;

    private float currentDistanceTravelled = 0.0f;

    private Vector2 gameplayDistanceToTravel;
    private float gameplayDuration;

    private void Update()
    {
        //ProcessAnimationQueue();
        ProcessCurrentAnimation(Time.deltaTime);
    }

    #region Gameplay Logic

    public void SetGameplayDistanceToTravel(Vector2 travelDistances)
    {
        gameplayDistanceToTravel = travelDistances;
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.Start).FindProperty(PlayerAnimationPropertyType.Distance);
            property.EndValue = travelDistances.x;
        }
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.MoveNextStart).FindProperty(PlayerAnimationPropertyType.Distance);
            property.EndValue = travelDistances.y;
        }
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.MoveNextEnd).FindProperty(PlayerAnimationPropertyType.Distance);
            property.StartValue = travelDistances.y;
        }
    }

    public void SetGameplayBPM(float bpm)
    {
        float bps = bpm / 60.0f;
        gameplayDuration = 1.0f / bps;

        foreach(PlayerAnimationPreset preset in Presets)
        {
            preset.Animation.Duration = gameplayDuration;
        }
        Debug.Log("Animation BPM set to " + bpm);
    }

    public void AddDistanceTravelled()
    {
        AddDistanceTravelled(currentDistance);
    }

    public void AddDistanceTravelled(float distance)
    {
        SetCurrentDistanceTravelled(currentDistanceTravelled + distance);
    }

    public void ResetDistanceTravelled()
    {
        SetCurrentDistanceTravelled(0);
    }

    private void SetCurrentDistanceTravelled(float distance)
    {
        currentDistanceTravelled = distance;
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.FinishStart).FindProperty(PlayerAnimationPropertyType.Distance);
            property.EndValue = currentDistanceTravelled;

            GetAnimationFromPreset(PlayerAnimationPresetType.FinishStart).Duration = (currentDistanceTravelled > gameplayDistanceToTravel.x) ? gameplayDuration : 0.0f;
        }
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.FinishEnd).FindProperty(PlayerAnimationPropertyType.Distance);
            property.StartValue = currentDistanceTravelled;
        }
    }

    #endregion Gameplay Logic

    #region Animations

    public void QueueAnimation(PlayerAnimationPresetType presetType, UnityAction onFinished = null)
    {
        foreach(PlayerAnimationPreset preset in Presets)
        {
            if(preset.Type == presetType)
            {
                if(onFinished != null)
                {
                    preset.Animation.OnFinished.AddListener(onFinished);
                }
                QueueAnimation(preset.Animation);
                break;
            }
        }
    }

    private void QueueAnimation(PlayerAnimation animation)
    {
        if(currentAnimation == null)
        {
            currentAnimation = animation;
        }
        else
        {
            currentAnimationQueue.Enqueue(animation);
        }
    }

    public bool IsPlaying()
    {
        return currentAnimation != null;
    }

    public void ClearAnimationQueue()
    {
        currentAnimationQueue.Clear();
    }

    public void StopAllAnimations()
    {
        ClearAnimationQueue();
        currentAnimation = null;
    }

    private PlayerAnimation GetAnimationFromPreset(PlayerAnimationPresetType presetType)
    {
        foreach (PlayerAnimationPreset preset in Presets)
        {
            if (preset.Type == presetType)
            {
                return preset.Animation;
            }
        }
        return null;
    }

    //private void ProcessAnimationQueue()
    //{
    //    if(currentAnimation != null || currentAnimationQueue.Count <= 0)
    //    {
    //        return;
    //    }

    //    currentAnimation = currentAnimationQueue.Dequeue();
    //    currentAnimationElapsed = 0.0f;
    //}

    private void ProcessCurrentAnimation(float deltaTime)
    {
        if(currentAnimation == null)
        {
            return;
        }

        currentAnimationElapsed += deltaTime;

        if(currentAnimationElapsed >= currentAnimation.Duration)
        {
            foreach (PlayerAnimationProperty property in currentAnimation.Properties)
            {
                SetAnimationPropertyValue(property.Type, property.EndValue);
            }
            UpdateVisuals();

            currentAnimation.OnFinished?.Invoke();
            currentAnimation.OnFinished.RemoveAllListeners();

            if(currentAnimationQueue.Count > 0)
            {
                currentAnimation = currentAnimationQueue.Dequeue();
                currentAnimationElapsed -= currentAnimation.Duration;
            }
            else
            {
                currentAnimation = null;
                currentAnimationElapsed = 0.0f;
            }
        }
        else
        {
            ApplyAnimation(currentAnimation, currentAnimationElapsed);
            UpdateVisuals();
        }
    }

    private void ApplyAnimation(PlayerAnimation animation, float elapsedTime)
    {
        float t = (animation.Duration > 0) ? (elapsedTime / animation.Duration) : 1.0f;
        foreach(PlayerAnimationProperty property in animation.Properties)
        {
            ApplyAnimationProperty(property, t);
        }
    }

    private void ApplyAnimationProperty(PlayerAnimationProperty property, float t)
    {
        float currentValue = Mathf.Lerp(property.StartValue, property.EndValue, property.curve.Evaluate(t));
        SetAnimationPropertyValue(property.Type, currentValue);
    }

    private void SetAnimationPropertyValue(PlayerAnimationPropertyType type, float value)
    {
        switch (type)
        {
            case PlayerAnimationPropertyType.Distance:
                currentDistance = value;
                break;
            case PlayerAnimationPropertyType.Angle:
                currentRotationAngle = value;
                break;
        }
    }

    #endregion Animations

    #region Extra Animations

    public void PlayLeniencyWindowActivated()
    {
        PlayerAnimationPresetType presetType = GetCurrentAnimationPresetType();

        if (presetType == PlayerAnimationPresetType.MoveClockwise
            || presetType == PlayerAnimationPresetType.MoveAntiClockwise)
        {
            NotPivot.GetComponent<TweenScript>().Play("LeniencyWindowOn");
        }
    }

    public void PlayLeniencyWindowDeactivated()
    {
        if(Pivot.transform.localScale.x > 1.5f)
        {
            Pivot.GetComponent<TweenScript>().Play("LeniencyWindowOff");
        }
        if (NotPivot.transform.localScale.x > 1.5f)
        {
            NotPivot.GetComponent<TweenScript>().Play("LeniencyWindowOff");
        }
    }

    public void PlayDeathAnimation()
    {
        if(Circle1 != null)
        {
            Circle1.enabled = false;

            for(int i = 0; i < Circle1.transform.childCount; ++i)
            {
                Transform child = Circle1.transform.GetChild(i);
                child.gameObject.SetActive(child.gameObject.name.StartsWith("Death"));
            }
        }
        if (Circle2 != null)
        {
            Circle2.enabled = false;

            for (int i = 0; i < Circle2.transform.childCount; ++i)
            {
                Transform child = Circle2.transform.GetChild(i);
                child.gameObject.SetActive(child.gameObject.name.StartsWith("Death"));
            }
        }
    }

    #endregion Extra Animations

    #region Transformations

    private void UpdateVisuals()
    {
        if (Circle1 == null || Circle2 == null || Line == null)
        {
            return;
        }

        Pivot.transform.localPosition = Vector2.zero;
        NotPivot.transform.localPosition = new Vector2(Mathf.Sin(currentRotationAngle * Mathf.Deg2Rad), Mathf.Cos(currentRotationAngle * Mathf.Deg2Rad)) * currentDistance;

        //Line.SetPosition(0, Pivot.transform.localPosition);
        //Line.SetPosition(1, NotPivot.transform.localPosition);
        Line.transform.localPosition = NotPivot.transform.localPosition / 2.0f;
        Line.transform.localScale = new Vector3(Line.transform.localScale.x, currentDistance, Line.transform.localScale.z);
        Line.transform.localEulerAngles = new Vector3(0, 0, -currentRotationAngle);
    }

    public void TogglePivot()
    {
        SetPivot(!is1CurrentPivot);
    }

    private void SetPivot(bool is1Pivot)
    {
        is1CurrentPivot = is1Pivot;
        Line.color = Pivot.color;

        transform.localPosition += Pivot.transform.localPosition;
        NotPivot.transform.localPosition -= Pivot.transform.localPosition;
        Pivot.transform.localPosition = Vector2.zero;

        currentRotationAngle += (currentRotationAngle > 180) ? -180 : 180;

        UpdateVisuals();
    }

    #endregion Transformations

    #region Camera Helpers

    public Bounds GetBoundingBox()
    {
        Vector2 min;
        min.x = (Circle1.bounds.min.x < Circle2.bounds.min.x) ? Circle1.bounds.min.x : Circle2.bounds.min.x;
        min.y = (Circle1.bounds.min.y < Circle2.bounds.min.y) ? Circle1.bounds.min.y : Circle2.bounds.min.y;

        Vector2 max;
        max.x = (Circle1.bounds.max.x > Circle2.bounds.max.x) ? Circle1.bounds.max.x : Circle2.bounds.max.x;
        max.y = (Circle1.bounds.max.y > Circle2.bounds.max.y) ? Circle1.bounds.max.y : Circle2.bounds.max.y;

        return new Bounds((min + max) / 2.0f, max - min);
    }

    public PlayerAnimationPresetType GetCurrentAnimationPresetType()
    {
        PlayerAnimationPresetType presetType = PlayerAnimationPresetType.Invalid;
        foreach (PlayerAnimationPreset preset in Presets)
        {
            if (preset.Animation == currentAnimation)
            {
                presetType = preset.Type;
                break;
            }
        }
        return presetType;
    }

    public Vector3 GetTargetPositionFromPresetType()
    {
        if (currentAnimation == null)
        {
            return Pivot.transform.position;
        }

        PlayerAnimationPresetType presetType = GetCurrentAnimationPresetType();

        if(presetType == PlayerAnimationPresetType.FinishStart
            || presetType == PlayerAnimationPresetType.MoveNextStart)
        {
            return NotPivot.transform.position;
        }
        else
        {
            return Pivot.transform.position;
        }
    }

    #endregion

    #region Helpers

    private SpriteRenderer Pivot
    {
        get { return is1CurrentPivot ? Circle1 : Circle2; }
    }

    private SpriteRenderer NotPivot
    {
        get { return !is1CurrentPivot ? Circle1 : Circle2; }
    }

    #endregion Helpers
}

public enum PlayerAnimationPresetType
{
    Start,
    MoveClockwise,
    MoveAntiClockwise,
    FinishStart,
    FinishEnd,
    MoveNextStart,
    MoveNextEnd,

    Invalid,
}

[System.Serializable]
public struct PlayerAnimationPreset
{
    public PlayerAnimationPresetType Type;
    public PlayerAnimation Animation;
}

[System.Serializable]
public class PlayerAnimation
{
    public List<PlayerAnimationProperty> Properties = new List<PlayerAnimationProperty>();
    public float Duration;
    public UnityEvent OnFinished;

    public PlayerAnimationProperty FindProperty(PlayerAnimationPropertyType type)
    {
        foreach(PlayerAnimationProperty property in Properties)
        {
            if(property.Type == type)
            {
                return property;
            }
        }
        return null;
    }
}

[System.Serializable]
public class PlayerAnimationProperty
{
    public PlayerAnimationPropertyType Type;
    public float StartValue;
    public float EndValue;
    public AnimationCurve curve;
}

public enum PlayerAnimationPropertyType
{
    Distance,
    Angle
}