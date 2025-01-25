using UnityEngine;

[RequireComponent(typeof(PlayerAnimator))]
public class PlayerAnimatorTester : MonoBehaviour
{
    private PlayerAnimator animator;

    private void Awake()
    {
        animator = GetComponent<PlayerAnimator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.QueueAnimation(PlayerAnimationPresetType.Start);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
            animator.QueueAnimation(PlayerAnimationPresetType.FinishStart);
            animator.QueueAnimation(PlayerAnimationPresetType.FinishEnd);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveNextStart);
            animator.QueueAnimation(PlayerAnimationPresetType.MoveNextEnd);
        }
    }
}