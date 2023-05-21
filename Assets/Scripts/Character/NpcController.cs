using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, InteractableObject
{

    [SerializeField] Dialogue dialogue;

    [Header("Quests")]
    [SerializeField] private QuestBase questToStart;
    [SerializeField] private QuestBase questToComplete;

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    private Character character;

    private float idleTimer = 0f;
    private NPCState npcState;
    private int currentPattern = 0;
    private ItemGiver itemGiver;
    private PokemonGiver pokemonGiver;
    private Quest activeQuest;
    

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (npcState == NPCState.Idle)
        {
            npcState = NPCState.Dialog;
            character.LookTowards(initiator.position);

            if (questToComplete != null)
            {
                var quest = new Quest(questToComplete);
                yield return quest.CompleteQuest();
                questToComplete = null;

            }

            if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (pokemonGiver != null && pokemonGiver.CanBeGiven())
            {
                yield return pokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
            }
            else if (questToStart != null)
            {
                activeQuest = new Quest(questToStart);
                yield return activeQuest.StartQuest();
                questToStart = null;

                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest();
                    activeQuest = null;
                }
            }
            else if (activeQuest != null)
            {
                if (activeQuest.CanBeCompleted())
                {
                    yield return activeQuest.CompleteQuest();
                    activeQuest = null;
                }
                else
                {
                    yield return DialogueManager.Instance.ShowDialogue(activeQuest.Base.InProgressDialogue);
                }
            }
            else
            {
                yield return DialogueManager.Instance.ShowDialogue(dialogue);
            }

            idleTimer = 0f;
            npcState = NPCState.Idle;

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