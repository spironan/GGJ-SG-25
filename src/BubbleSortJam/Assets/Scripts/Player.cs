using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(!GameManager.instance.HasStartedGame())
            {
                GameManager.instance.StartGame();
                return;
            }

            if (BpmTracker.instance.BeatWindow)
            {
                Debug.Log("attempting to swap");
                bool success = GameManager.instance.AttemptSwap();
                if (success)
                {
                    // launch an event here to know its PLAYER action that caused it to be correct.
                }
                Debug.Log("Succeeded swapping? " + success);
            }
        }
    }

}
