using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class TrainerController : MonoBehaviour
{
    [SerializeField] private GameObject exclamation;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private GameObject fov;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string tainerName;

    private Character character;

    public string TrainerName
    {
        get => tainerName;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void Start()
    {
        SetFovRotation(character.Animator.DefaultDirection);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {
        AudioManager.instance.PlayMusic(BGM.TRAINER_EYE_MEET_YOUNG);
        // Show exclamation
        exclamation.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        exclamation.SetActive(false);

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Show dialog
        StartCoroutine(DialogueManager.Instance.ShowDialogue(dialogue, () =>
        {
            GameManager.Instance.StartTrainerBattle(this);
        }));
    }

    public void SetFovRotation(FacingDirection dir)
    {
        float angle = 0f;
        if (dir == FacingDirection.Right) 
        {
            angle = 90f;
        }
        else if (dir == FacingDirection.Up)
        {
            angle = 180f;
        }
        else if (dir == FacingDirection.Left)
        {
            angle = 270;
        }

        fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
}
