using System.Collections;
using System.Linq;
using UnityEngine;

public class PlayerController : MonoBehaviour, ISavable
{

    private Vector2 input;
    private Character character;

    [SerializeField] private string playerName;

    [SerializeField] private float _interactRadius;
    [SerializeField] private Vector3 _interactStepOffset;
    [SerializeField] private AnimatedSprite _iceTrail;

    private Vector3 _interactPos;
    private Vector3 _faceDir;
    private bool _isRunning;
    private bool _avoidWildPokemon;

    private int _stepCount = 0;

    public static PlayerController I { get; private set; }

    public string PlayerName
    {
        get => playerName;
    }

    public Character Character
    {
        get => character;
    }
    public AnimatedSprite IceTrail { get => _iceTrail; set => _iceTrail = value; }
    public bool AvoidWildPokemon { get => _avoidWildPokemon; set => _avoidWildPokemon = value; }

    private void Awake()
    {
        I = this;
        character = GetComponent<Character>();
        _interactPos = transform.position + _faceDir;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + _interactStepOffset, _interactRadius);
        Gizmos.DrawWireSphere(_interactPos, _interactRadius);
    }

    // Handle player movement
    public void HandleUpdate()
    {
        if (!character.IsMoving)
        {
            _isRunning = Input.GetKey(KeyCode.X);
            
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // Remove diagnal move
            if (input.x != 0) { input.y = 0; }

            if (input != Vector2.zero)
            {
                StartCoroutine(character.Move(input, OnMoveOver, isRunning: _isRunning));
            }
        }

        character.HandleUpdate();

        if (!character.IsMoving && Input.GetKeyDown(KeyCode.Z))
        {
            StartCoroutine(Interact());
        }

    }

    private IEnumerator Interact()
    {
        _faceDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        _interactPos = transform.position + _faceDir;
        Collider2D colliderFront = Physics2D.OverlapCircle(_interactPos, _interactRadius, GameLayers.instance.InteractableLayer | GameLayers.instance.WaterLayer);
        Collider2D colliderStep = Physics2D.OverlapCircle(transform.position + _interactStepOffset, _interactRadius, GameLayers.instance.StepInteractableLayer | GameLayers.instance.WaterLayer);
        if (colliderFront != null)
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            yield return colliderFront.GetComponent<InteractableObject>()?.Interact(transform);
        }
        else if (colliderStep != null)
        {
            AudioManager.Instance.PlaySE(SFX.CONFIRM);
            yield return colliderStep.GetComponent<InteractableObject>()?.Interact(transform);
        }
    }

    IPlayerTriggerable currentlyInTrigger;

    public void OnMoveOver()
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

        if (character.Animator.IsSurfing)
        {
            _iceTrail.gameObject.SetActive(true);
        }
        else
        {
            _iceTrail.gameObject.SetActive(false);
        }

        if (_avoidWildPokemon)
        {
            if (_stepCount < 100)
            {
                _stepCount += 1;
            }
            else
            {
                _avoidWildPokemon = false;
                _stepCount = 0;
                StopMovingAnimation();
                StartCoroutine(DialogueManager.Instance.ShowDialogueText("恶臭消散了！野生宝可梦们蠢蠢欲动！"));
            }
        }
    }

    public void StopMovingAnimation()
    {
        character.IsMoving = false;
        character.Animator.IsRunning = false;
        character.Animator.IsMoving = false;
    }

    public void StartMovingAnimation()
    {
        character.IsMoving = true;
        character.Animator.IsRunning = false;
        character.Animator.IsMoving = true;
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
