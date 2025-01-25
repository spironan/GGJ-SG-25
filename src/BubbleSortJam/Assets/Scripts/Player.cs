using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private int currentId = 0;

    [SerializeField]
    private Life health;

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
            bool success = GameManager.instance.AttemptSwap(currentId, currentId + 1);
            Debug.Log("Succeeded swapping? " + success);
        }
    }
}
