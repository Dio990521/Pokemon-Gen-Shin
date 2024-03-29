using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, InteractableObject
{

    [SerializeField] List<Dialogue> dialogues;

    [Header("Movement")]
    [SerializeField] List<Vector2> movementPattern;
    [SerializeField] float timeBetweenPattern;

    private Character character;

    private float idleTimer = 0f;
    private NPCState npcState;
    private int currentPattern = 0;
    private ItemGiver itemGiver;
    private PokemonGiver pokemonGiver;
    private Healer healer;
    private Merchant merchant;
    private ItemChecker checker;

    [SerializeField] private CutsceneName _enableCutscene = CutsceneName.None;
    [SerializeField] private CutsceneName _disableCutscene = CutsceneName.None;

    private void Awake()
    {
        character = GetComponent<Character>();
        itemGiver = GetComponent<ItemGiver>();
        pokemonGiver = GetComponent<PokemonGiver>();
        healer = GetComponent<Healer>();
        merchant = GetComponent<Merchant>();
        checker = GetComponent<ItemChecker>();
    }

    private void Start()
    {
        if (_enableCutscene != CutsceneName.None && GameKeyManager.Instance.GetBoolValue(_enableCutscene.ToString()))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = true;
            gameObject.GetComponent<SpriteRenderer>().enabled = true;
        }
        if (_disableCutscene != CutsceneName.None && GameKeyManager.Instance.GetBoolValue(_disableCutscene.ToString()))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
    public IEnumerator Interact(Transform initiator)
    {
        if (npcState == NPCState.Idle)
        {
            npcState = NPCState.Dialog;
            if (character != null)
                character.LookTowards(initiator.position);

            if (gameObject.TryGetComponent(out TrainerController trainerController) && !trainerController.IsBattleLost)
            {
                if (GameManager.Instance.StateMachine.CurrentState == CutsceneState.I)
                {
                    GameManager.Instance.StateMachine.Pop();
                }
                yield return trainerController.Interact(initiator);
            }
            else if (checker != null && !checker.Used)
            {
                yield return checker.CheckItem(initiator.GetComponent<PlayerController>());
            }
            else if (itemGiver != null && itemGiver.CanBeGiven())
            {
                yield return itemGiver.GiveItem(initiator.GetComponent<PlayerController>());
            }
            else if (pokemonGiver != null && pokemonGiver.CanBeGiven())
            {
                yield return pokemonGiver.GivePokemon(initiator.GetComponent<PlayerController>());
            }
            else if (healer != null)
            {
                yield return healer.Heal(initiator);
            }
            else if (merchant != null)
            {
                yield return merchant.Trade();
            }
            else
            {
                Dialogue curDialogue = null;
                foreach (var dialogue in dialogues)
                {
                    if (dialogue.AfterCutsceneActivate == CutsceneName.None)
                    {
                        curDialogue = dialogue;
                    }
                    else if (GameKeyManager.Instance.GetBoolValue(dialogue.AfterCutsceneActivate.ToString()))
                    {
                        curDialogue = dialogue;
                    }
                    else
                    {
                        break;
                    }
                }
                if (curDialogue != null)
                {
                    yield return DialogueManager.Instance.ShowDialogue(curDialogue);
                }
            }
            if (GameManager.Instance.StateMachine.CurrentState == FreeRoamState.I)
                character.Animator.SetFacingDirection(character.Animator.DefaultDirection);
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
        if (character != null)
        {
            character.HandleUpdate();
        }
        
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