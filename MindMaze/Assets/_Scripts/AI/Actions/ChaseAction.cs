using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseAction : AIAction
{
    [SerializeField] AgentAnimations animation = null;
    public override void TakeAction()
    {
        var direction = enemyBrain.Target.transform.position - transform.position;
        aiMovementData.Direction = direction.normalized;
        aiMovementData.PointOfInterest = enemyBrain.Target.transform.position;
        enemyBrain.Move(aiMovementData.Direction, aiMovementData.PointOfInterest);
        if(animation != null)
        {
            animation.StopAttackAnimation();
        }
        
    }
}
