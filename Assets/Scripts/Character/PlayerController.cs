using System;
using System.Collections;
using System.Collections.Generic;
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
        var colliders = Physics2D.OverlapCircleAll(transform.position, character.OffsetY, GameLayers.instance.TriggerableLayers);
        foreach (var collider in colliders)
        {
            var triggerable = collider.GetComponent<IPlayerTriggerable>();
            if (triggerable != null)
            {
                triggerable.OnPlayerTriggered(this);
                break;
            }
        }
    }

    public object CaptureState()
    {
        float[] position = new float[] 
        {
            transform.position.x,
            transform.position.y,
        };

        return position;
    }

    public void RestoreState(object state)
    {
        var position = (float[]) state;
        transform.position = new Vector3(position[0], position[1]);
    }
}
