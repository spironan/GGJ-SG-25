using UnityEngine;
using System;

public class Life : MonoBehaviour
{
    public int HealthTotal = 5;
    [SerializeField]
    private int currentTotal;

    // Event to notify when the player dies
    public event Action OnDeath;
    public event Action OnDamageTaken;
    private void Start()
    {
        ResetLife();
    }

    public void TakeDamage(int damage)
    {
        currentTotal -= damage;
        OnDamageTaken?.Invoke();
        LifeChangedGameplayEvent.BroadcastEvent(currentTotal);
        if (currentTotal <= 0)
        {
            // on death event
            OnDeath?.Invoke();
            Debug.Log("Player Death");
        }
    }

    public void ResetLife()
    {
        currentTotal = HealthTotal;
        LifeChangedGameplayEvent.BroadcastEvent(currentTotal);
    }

    public void RegainHealth(int hp)
    {
        HealthTotal += hp;
        currentTotal += hp;
        //if (currentTotal > HealthTotal)
        //{
        //    currentTotal = HealthTotal;
        //}
        LifeChangedGameplayEvent.BroadcastEvent(currentTotal);
    }
}
