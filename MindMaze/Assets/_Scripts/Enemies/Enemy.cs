using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IHittable, IAgent, IKnockBack
{
    [field: SerializeField]
    public EnemyDataSO EnemyData { get; set; }

    [field: SerializeField]
    public int Health { get; private set; } = 2;

    [field: SerializeField]
    public EnemyAttack enemyAttack { get; set; }

    private bool dead = false;

    private AgentMovement agentMovemenet;

    [field: SerializeField]
    public UnityEvent OnGetHit { get; set; }

    [field: SerializeField]
    public UnityEvent OnDie { get; set; }

    // New Field for Damage Multiplier
    [field: SerializeField]
    public int TakeDamageMultiplier { get; set; } = 1; // Default to 1

    private bool hasIncreased = false;

    private void Awake()
    {
        if (enemyAttack == null)
        {
            enemyAttack = GetComponent<EnemyAttack>();
        }
        agentMovemenet = GetComponent<AgentMovement>();
    }

    private void Start()
    {
        Health = EnemyData.MaxHealth;
    }

    public void GetHit(int damage, GameObject damageDealer)
    {
        if (!dead)
        {
            if(!hasIncreased && EnemyData.MaxHealth / 2 >= Health && gameObject.CompareTag("Boss"))
            {
                hasIncreased = true;
                agentMovemenet.increaseSpeed(2f);
            }

            // Apply damage multiplier
            int totalDamage = damage;
            Health -= totalDamage;

            Debug.Log($"Enemy took {totalDamage} damage (Multiplier: {TakeDamageMultiplier})");

            OnGetHit?.Invoke();

            if (Health <= 0)
            {
                dead = true;
                OnDie?.Invoke();
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void PerformAttack()
    {
        if (!dead)
        {
            enemyAttack.Attack(EnemyData.Damage);
        }
    }

    public void KnockBack(Vector2 direction, float power, float duration)
    {
        if(!gameObject.CompareTag("Boss"))
            agentMovemenet.KnockBack(direction, power, duration);
    }
}
