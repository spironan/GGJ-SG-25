using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int currentId = 0;

    [SerializeField]
    private Life health;

    private Queue<uint> queue = new Queue<uint>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = GetComponent<Life>();
        GameManager.instance.OnFailedSwap += OnFailedSwapAttempt;
    }

    private void OnFailedSwapAttempt()
    {
        health.TakeDamage(1);
        Debug.Log("Took Damage");
    }

    // Update is called once per frame
    void Update()
    {
        // Temporary
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentId++;
            Debug.Log($"{currentId}");
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentId--;
            Debug.Log($"{currentId}");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (BpmTracker.instance.BeatWindow)
            {
                Debug.Log("attempting to swap");
                bool success = GameManager.instance.AttemptSwap(currentId, currentId + 1);
                Debug.Log("Succeeded swapping? " + success);
            }
            else
            {
                // we lost our beat window - deal dmg to player here/suffer the consequences
                OnFailedSwapAttempt();
            }
        }
    }

}
