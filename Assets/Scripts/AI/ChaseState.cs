using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ChaseState : FSMState
{
    private AIMovement aiMovement;

    public ChaseState()
    {
        stateID = StateID.Chasing;
    }

    public override void Reason(GameObject player, GameObject npc)
    {
        // TODO: to make state classes modular, make an interface for
        // enemy controllers, so you can put that in the GetComponent function
        if (Vector3.Distance(player.transform.position, npc.transform.position) >= 5.0f)
            npc.GetComponent<MedievalSwordsmanController>().SetTransition(Transition.LostPlayer);
    }

    public override void Act(GameObject player, GameObject npc)
    {
        aiMovement = npc.GetComponent<AIMovement>();
        aiMovement.Move(player.transform.position);
    }
}
