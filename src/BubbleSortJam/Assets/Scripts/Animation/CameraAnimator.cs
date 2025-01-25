using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    [Header("Target Settings")]
    [SerializeField] private PlayerAnimator Target;

    [Header("Movement Settings")]
    [SerializeField] private float MoveSpeed;

    private void Update()
    {
        ProcessMovement(Time.deltaTime);
    }

    #region Movement

    private void ProcessMovement(float deltaTime)
    {
        transform.position = Vector3.Lerp(transform.position, GetTargetPosition(), deltaTime * MoveSpeed);
    }

    private Vector3 GetTargetPosition()
    {
        Vector3 targetPosition = transform.position;
        if(Target == null)
        {
            return targetPosition;
        }

        targetPosition = Target.GetTargetPositionFromPresetType();
        targetPosition.z = transform.position.z;
        return targetPosition;
    }

    #endregion Movement
}