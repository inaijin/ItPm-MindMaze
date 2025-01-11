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

    [SerializeField] HealthBarUI uiHealth;

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
        uiHealth?.Initialized(EnemyData.MaxHealth);
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
            uiHealth?.UpdateHealth(Health);

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
        switch (gameObject.tag)
        {
            case "Boss":
                // Boss doesn't experience knockback
                Debug.Log("Boss is immune to knockback.");
                break;

            case "Enemy1":
                // Small enemies have lighter knockback
                agentMovemenet.KnockBack(direction, power, duration);
                break;

            case "Enemy2":
                // Medium enemies have normal knockback
                agentMovemenet.KnockBack(direction, power * 0.5f, duration * 0.5f);
                break;

            case "Enemy3":
                // Heavy enemies have reduced knockback
                agentMovemenet.KnockBack(direction, power * 0.25f, duration * 0.25f);
                break;

            default:
                // Default behavior for undefined tags
                Debug.LogWarning("Tag not recognized, applying default knockback behavior.");
                agentMovemenet.KnockBack(direction, power, duration);
                break;
        }
    }
}
