using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class PlayerAnimator : MonoBehaviour
{
    [Header("Gameplay Variables")]
    [SerializeField] private Vector2 GameplayDistanceToTravel;

    [Header("Components")]
    [SerializeField] private SpriteRenderer Circle1;
    [SerializeField] private SpriteRenderer Circle2;
    [SerializeField] private LineRenderer Line;

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

    private void Start()
    {
        InitializeGameplayDistanceToTravel();
    }

    private void Update()
    {
        ProcessAnimationQueue();
        ProcessCurrentAnimation(Time.deltaTime);
        UpdateVisuals();
    }

    #region Gameplay Logic

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
        }
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.FinishEnd).FindProperty(PlayerAnimationPropertyType.Distance);
            property.StartValue = currentDistanceTravelled;
        }
    }

    private void InitializeGameplayDistanceToTravel()
    {
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.Start).FindProperty(PlayerAnimationPropertyType.Distance);
            property.EndValue = GameplayDistanceToTravel.x;
        }
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.MoveNextStart).FindProperty(PlayerAnimationPropertyType.Distance);
            property.EndValue = GameplayDistanceToTravel.y;
        }
        {
            PlayerAnimationProperty property = GetAnimationFromPreset(PlayerAnimationPresetType.MoveNextEnd).FindProperty(PlayerAnimationPropertyType.Distance);
            property.StartValue = GameplayDistanceToTravel.y;
        }
    }

    #endregion Gameplay Logic

    #region Animations

    public void QueueAnimation(PlayerAnimationPresetType presetType)
    {
        foreach(PlayerAnimationPreset preset in Presets)
        {
            if(preset.Type == presetType)
            {
                QueueAnimation(preset.Animation);
                break;
            }
        }
    }

    public void QueueAnimation(PlayerAnimation animation)
    {
        currentAnimationQueue.Enqueue(animation);
    }

    public bool IsPlaying()
    {
        return currentAnimation != null;
    }

    public void StopAllAnimations()
    {
        currentAnimationQueue.Clear();
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

    private void ProcessAnimationQueue()
    {
        if(currentAnimation != null || currentAnimationQueue.Count <= 0)
        {
            return;
        }

        currentAnimation = currentAnimationQueue.Dequeue();
        currentAnimationElapsed = 0.0f;
    }

    private void ProcessCurrentAnimation(float deltaTime)
    {
        if(currentAnimation == null)
        {
            return;
        }

        currentAnimationElapsed += deltaTime;
        bool isDone = currentAnimationElapsed >= currentAnimation.Duration;

        ApplyAnimation(currentAnimation, (isDone ? currentAnimation.Duration : currentAnimationElapsed));

        if(isDone)
        {
            currentAnimation.OnFinished?.Invoke();
            currentAnimation = null;
            currentAnimationElapsed = 0.0f;
        }
    }

    private void ApplyAnimation(PlayerAnimation animation, float elapsedTime)
    {
        float t = elapsedTime / animation.Duration;
        foreach(PlayerAnimationProperty property in animation.Properties)
        {
            ApplyAnimationProperty(property, t);
        }
    }

    private void ApplyAnimationProperty(PlayerAnimationProperty property, float t)
    {
        float currentValue = Mathf.Lerp(property.StartValue, property.EndValue, property.curve.Evaluate(t));

        switch(property.Type)
        {
            case PlayerAnimationPropertyType.Distance:
                currentDistance = currentValue;
                break;
            case PlayerAnimationPropertyType.Angle:
                currentRotationAngle = currentValue;
                break;
        }
    }

    #endregion Animations

    #region Transformations

    private void UpdateVisuals()
    {
        if (Circle1 == null || Circle2 == null || Line == null)
        {
            return;
        }
        UpdatePositions();
    }

    public void TogglePivot(bool preservePositions)
    {
        SetPivot(!is1CurrentPivot, preservePositions);
    }

    private void SetPivot(bool is1Pivot, bool preservePositions)
    {
        is1CurrentPivot = is1Pivot;
        Line.startColor = Line.endColor = Pivot.color;

        if (preservePositions)
        {
            NotPivot.transform.localPosition -= Pivot.transform.localPosition;
            transform.position = Pivot.transform.position;
            Pivot.transform.localPosition = Vector2.zero;

            currentRotationAngle += (currentRotationAngle > 180) ? -180 : 180;
        }
    }

    private void UpdatePositions()
    {
        Pivot.transform.localPosition = Vector2.zero;
        NotPivot.transform.localPosition = new Vector2(Mathf.Sin(currentRotationAngle * Mathf.Deg2Rad), Mathf.Cos(currentRotationAngle * Mathf.Deg2Rad)) * currentDistance;

        Line.SetPosition(0, Pivot.transform.localPosition);
        Line.SetPosition(1, NotPivot.transform.localPosition);
    }

    #endregion Transformations

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
    MoveNextEnd
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