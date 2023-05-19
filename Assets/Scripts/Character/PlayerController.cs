using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{

    private Vector2 input;
    private Character character;

    [SerializeField] private Sprite sprite;
    [SerializeField] private string playerName;

    public string PlayerName
    {
        get => playerName;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public Character Character
    {
        get => character;
    }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    // Handle player movement
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Remove diagnal move
            if (input.x != 0) { input.y = 0; }

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver));
            }
        }

        character.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Interact());
        }
    }

    private IEnumerator Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);
        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.instance.InteractableLayer);
        if (collider != null)
        {
            yield return collider.GetComponent<InteractableObject>()?.Interact(transform);
        }
    }

    IPlayerTriggerable currentlyInTrigger;

    private void OnMoveOver()
    {
        var colliders = Physics2D.OverlapCircleAll(transform.position, character.OffsetY, GameLayers.instance.TriggerableLayers);
        IPlayerTriggerable triggerable = null;
        foreach (var collider in colliders)
        {
            triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                if (triggerable == currentlyInTrigger && !triggerable.TriggerRepeatedly)
                {
                    break;
                }
                triggerable.OnPlayerTriggered(this);
                currentlyInTrigger = triggerable;
                break;
            }
        }

        if (colliders.Count() == 0 || triggerable != currentlyInTrigger)
        {
            currentlyInTrigger = null;
        }
    }

    public object CaptureState()
    {
        var saveData = new PlayerSaveData()
        {
            position = new float[]
            {
                transform.position.x,
                transform.position.y,
            },
            pokemons = GetComponent<PokemonParty>().Pokemons.Select(p => p.GetSaveData()).ToList()
        };

        return saveData;
    }

    public void RestoreState(object state)
    {
        var saveData = (PlayerSaveData) state;

        // Restore pos
        var pos = saveData.position;
        transform.position = new Vector3(pos[0], pos[1]);

        // Restore party
        GetComponent<PokemonParty>().Pokemons =  saveData.pokemons.Select(s => new Pokemon(s)).ToList();
    }
}
