using UnityEngine;

public class FrogMoveState : FSMState
{
    NPCController npcController;
    AIMovement aiMovement;
    private Vector2 velocity;
    private float timeBetweenJumps;
    private float timeBetweenJumpsLeft;

    private float gravity;
    private float jumpVelocity;

    public FrogMoveState(GameObject npc, float timeBetweenJumps, float jumpHeight, float timeToJumpHeight, float moveSpeed)
    {
        stateID = StateID.FrogMoving;
        npcController = npc.GetComponent<NPCController>();
        aiMovement = npc.GetComponent<AIMovement>();
        this.timeBetweenJumps = timeBetweenJumps;
        timeBetweenJumpsLeft = timeBetweenJumps;

        velocity.x = moveSpeed;
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpHeight, 2);
        jumpVelocity = (2 * jumpHeight) / timeToJumpHeight;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npcController.Health <= 0f)
            npcController.SetTransition(Transition.NoHealth);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        if (timeBetweenJumpsLeft >= 0f && aiMovement.collisions.below)
        {
            timeBetweenJumpsLeft -= Time.deltaTime;
            velocity = aiMovement.CalculateVelocity();
            aiMovement.Move(velocity * Time.deltaTime);
            return;
        }

        timeBetweenJumpsLeft = timeBetweenJumps;

        velocity.y = jumpVelocity;
    }
}
