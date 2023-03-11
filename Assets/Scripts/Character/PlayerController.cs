using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    private Vector2 input;
    private Character character;

    public event Action OnEncountered;
    public event Action<Collider2D> OnEnterTrainerView;

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
            Interact();
        }
    }

    private void Interact()
    {
        var facingDir = new Vector3(character.Animator.MoveX, character.Animator.MoveY);
        var interactPos = transform.position + facingDir;

        //Debug.DrawLine(transform.position, interactPos, Color.red, 0.5f);
        Collider2D collider = Physics2D.OverlapCircle(interactPos, 0.3f, GameLayers.instance.InteractableLayer);
        if (collider != null)
        {
            collider.GetComponent<InteractableObject>()?.Interact(transform);
        }
    }

    private void OnMoveOver()
    {
        CheckEncounters();
        CheckIfInTrainersView();
    }

    // Start battle with 10% probability when player is on the grass
    private void CheckEncounters()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.15f, GameLayers.instance.GrassMask);
        if (collider != null)
        {
            collider.GetComponent<GrassEffect>().InitEffect();
            if (UnityEngine.Random.Range(1, 101) <= 10)
            {
                character.Animator.IsMoving = false;
                OnEncountered();
            }
        }
    }

    private void CheckIfInTrainersView()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, 0.15f, GameLayers.instance.FovLayer);
        if (collider != null)
        {
            character.Animator.IsMoving = false;
            OnEnterTrainerView?.Invoke(collider);
        }
    }

}
