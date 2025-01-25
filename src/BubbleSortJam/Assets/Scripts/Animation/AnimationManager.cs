using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;

public class AnimationManager : MonoBehaviour
{
    [Header("World Space Settings")]
    [SerializeField] private Vector2 GridSlotSize;
    [SerializeField] private float GameplayBPM;

    [Header("Player")]
    [SerializeField] private PlayerAnimator player;

    public PlayerAnimator Player { get { return player; } }

    [Header("Number Arrays")]
    [SerializeField] private GameObject numberArrayPrefab;

    private List<NumberArrayAnimator> numberArrays = new List<NumberArrayAnimator>();

    private static AnimationManager instance = null;

    public static AnimationManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if(instance == this)
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        Player.SetGameplayDistanceToTravel(new Vector2(GridSlotSize.x, GridSlotSize.y * 2));
        Player.SetGameplayBPM(GameplayBPM);
    }

    public void CreateNumberArray(List<int> values)
    {
        NumberArrayAnimator array = Instantiate(numberArrayPrefab).GetComponent<NumberArrayAnimator>();
        array.name = "ArrayAnimator_" + numberArrays.Count;

        array.transform.SetParent(transform);
        array.transform.localPosition = new Vector2(0.0f, numberArrays.Count * GridSlotSize.y * -2);

        array.Initialize(values, GridSlotSize.x);

        numberArrays.Add(array);
    }
}
