using System.Collections.Generic;
using UnityEngine;

public class AnimationTester : MonoBehaviour
{
    [SerializeField] private List<AnimationTesterData> DebugData;

    private AnimationManager manager;

    private void Awake()
    {
        manager = GetComponent<AnimationManager>();
    }

    private void Start()
    {
        foreach(AnimationTesterData data in DebugData)
        {
            manager.CreateNumberArray(data.Array).PlaySwapAnimation(1);
            for(int unsortedCount = data.Array.Count; unsortedCount > 1; --unsortedCount)
            {
                manager.Player.QueueAnimation(PlayerAnimationPresetType.Start);

                for(int i = 0; i < unsortedCount - 1; ++i)
                {
                    manager.Player.QueueAnimation(PlayerAnimationPresetType.MoveClockwise);
                }

                manager.Player.QueueAnimation(PlayerAnimationPresetType.FinishStart);
                manager.Player.QueueAnimation(PlayerAnimationPresetType.FinishEnd);
            }
            manager.Player.QueueAnimation(PlayerAnimationPresetType.MoveNextStart);
            manager.Player.QueueAnimation(PlayerAnimationPresetType.MoveNextEnd);
        }
    }
}

[System.Serializable]
public class AnimationTesterData
{
    public List<int> Array;
}