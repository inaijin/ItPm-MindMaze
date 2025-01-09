using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackAction : AIAction
{

    [SerializeField]AgentAnimations animation = null;
    public override void TakeAction()
    {
        aiMovementData.Direction = Vector2.zero;
        aiMovementData.PointOfInterest = enemyBrain.Target.transform.position;
        enemyBrain.Move(aiMovementData.Direction, aiMovementData.PointOfInterest);
        aiActionData.Attack = true;
        if(animation != null)
        {
            animation.PlayAttackAnimation();
        }
        else
        {
            enemyBrain.Attack();
        }
        
        
    }
}
