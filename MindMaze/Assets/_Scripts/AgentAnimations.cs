using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AgentAnimations : MonoBehaviour
{
    protected Animator agentAnimator;
    private EnemyAIBrain enemyBrain;

    private void Awake()
    {
        enemyBrain = transform.root.GetComponent<EnemyAIBrain>();
        agentAnimator = GetComponent<Animator>();
    }

    public void SetWalkAnimation(bool val)
    {
        agentAnimator.SetBool("Walk", val);
    }

    public void AnimatePlayer(float velocity)
    {
        SetWalkAnimation(velocity > 0);
    }

    public void PlayDeathAnimation()
    {
        agentAnimator.SetTrigger("Death");
    }

    public void PlayAttackAnimation()
    {
        if(agentAnimator.GetBool("isNearPlayer") == false)
        {
            agentAnimator.SetBool("isNearPlayer", true);
        }
        
    }

    public void StopAttackAnimation()
    {
        if (agentAnimator.GetBool("isNearPlayer") == true)
        {
            agentAnimator.SetBool("isNearPlayer", false);
        }
    }

    public void Attack()
    {
        enemyBrain.Attack();
    }
}
