using UnityEngine;

public class FrogMoveState : FSMState
{
    NPCController npcController;
    AIMovement aiMovement;
    private Vector2 velocity;
    private float timeBetweenJumps;
    private float timeBetweenJumpsLeft;
    private bool turnOnNextJump = false;

    public FrogMoveState(GameObject npc, float timeBetweenJumps)
    {
        stateID = StateID.FrogMoving;
        npcController = npc.GetComponent<NPCController>();
        aiMovement = npc.GetComponent<AIMovement>();
        this.timeBetweenJumps = timeBetweenJumps;
        timeBetweenJumpsLeft = 0f;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        if (npcController.Health <= 0f)
            npcController.SetTransition(Transition.NoHealth);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        if (timeBetweenJumpsLeft <= 0f && aiMovement.collisions.below)
        {
            if (turnOnNextJump)
            {
                aiMovement.DirectionX = -aiMovement.DirectionX;
                turnOnNextJump = false;
            }

            timeBetweenJumpsLeft = timeBetweenJumps;
            velocity = aiMovement.CalculateJumpVelocity();
            velocity = aiMovement.CalculateVelocity();
            aiMovement.Move(velocity * Time.deltaTime);
        }

        else if (timeBetweenJumpsLeft >= 0f && aiMovement.collisions.below)
        {
            timeBetweenJumpsLeft -= Time.deltaTime;
            aiMovement.SimulateGravity();
        }

        else if (!aiMovement.collisions.below)
        {
            velocity = aiMovement.CalculateVelocity();
            aiMovement.Move(velocity * Time.deltaTime);
        }

        if (aiMovement.collisions.left || aiMovement.collisions.right)
            turnOnNextJump = true;
    }
}
