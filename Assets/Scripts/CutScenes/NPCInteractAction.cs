using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteractAction : CutsceneAction
{
    [SerializeField] private NPCController npc;

    public override IEnumerator Play()
    {
        yield return npc.Interact(PlayerController.I.transform);
    }
}
