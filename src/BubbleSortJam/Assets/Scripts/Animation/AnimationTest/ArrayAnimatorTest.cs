using UnityEngine;
using System.Collections.Generic;

public class ArrayAnimatorTest : MonoBehaviour
{
    [SerializeField] private List<int> DebugValues;

    private AnimationManager manager;

    private void Awake()
    {
        manager = GetComponent<AnimationManager>();
    }

    private void Start()
    {
        manager.CreateNumberArray(DebugValues);
    }
}
