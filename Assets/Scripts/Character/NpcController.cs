using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcController : MonoBehaviour, InteractableObject
{

    [SerializeField] Dialogue dialogue;
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;
    private Character character;

    private float idleTimer = 0f;
    private NPCState npcState;
    private int currentPattern = 0;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    public void Interact(Transform initiator)
    {
        if (npcState == NPCState.Idle)
        {
            npcState = NPCState.Dialog;
            character.LookTowards(initiator.position);
            StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
            {
                idleTimer = 0f;
                npcState = NPCState.Idle;
            }));
        }
            
    }

    private void Update()
    {

        if (npcState == NPCState.Idle)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer > timeBetweenPattern)
            {
                idleTimer = 0f;
                if (movementPattern.Count > 0)
                {
                    StartCoroutine(Walk());
                }
                
            }
        }

        character.HandleUpdate();
    }

    private IEnumerator Walk()
    {
        npcState = NPCState.Walking;

        var oldPos = transform.position;

        yield return character.Move(movementPattern[currentPattern]);

        if (transform.position != oldPos)
        {
            currentPattern = ++currentPattern % movementPattern.Count;
        }
        
        npcState = NPCState.Idle;
    }
}

public enum NPCState { Idle, Walking, Dialog }