using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public class TrainerController : MonoBehaviour, InteractableObject, ISavable
{
    [SerializeField] private GameObject exclamation;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private Dialogue dialogueAfterBattle;
    [SerializeField] private int winMoney;
    [SerializeField] private BattleTrigger _battleTrigger;
    [SerializeField] private BGM meetBGM = BGM.TRAINER_EYE_MEET_YOUNG;
    [SerializeField] private BGM startBGM = BGM.BATTLE_TRAINER;
    [SerializeField] private BGM winBGM = BGM.VICTORY_TRAINER;
    [SerializeField] private GameObject fov;
    [SerializeField] private Sprite sprite;
    [SerializeField] private string tainerName;
    [SerializeField] private bool _isBoss;
    [SerializeField] private bool _isGymLeader;

    [SerializeField] private CutsceneName _activateCutsceneNameWin;

    protected Character character;
    private bool _isBattleLost = false;
    private Vector3 _exclamationPos;

    public string TrainerName
    {
        get => tainerName;
    }

    public Sprite Sprite
    {
        get => sprite;
    }

    public int WinMoney
    {
        get => winMoney;
    }

    public BGM WinBGM { get => winBGM; }
    public BGM StartBGM { get => startBGM; set => startBGM = value; }
    public BattleTrigger BattleTrigger { get => _battleTrigger; set => _battleTrigger = value; }
    public Dialogue DialogueAfterBattle { get => dialogueAfterBattle; set => dialogueAfterBattle = value; }
    public BGM MeetBGM { get => meetBGM; set => meetBGM = value; }
    public bool IsBattleLost { get => _isBattleLost; set => _isBattleLost = value; }
    public bool IsBoss { get => _isBoss; set => _isBoss = value; }
    public bool IsGymLeader { get => _isGymLeader; set => _isGymLeader = value; }

    private void Awake()
    {
        character = GetComponent<Character>();
        _exclamationPos = exclamation.transform.position;
    }

    private void Start()
    {
        if (character != null)
            SetFovRotation(character.Animator.DefaultDirection);
    }

    private void Update()
    {
        if (character != null)
            character.HandleUpdate();
    }

    public IEnumerator Interact(Transform initiator)
    {
        _exclamationPos = exclamation.transform.position;
        GameManager.Instance.StateMachine.Push(CutsceneState.I);
        character.LookTowards(initiator.position);
        if (!IsBattleLost)
        {
            if (meetBGM != BGM.NONE)
                AudioManager.Instance.PlayMusicVolume(MeetBGM);
            if (!IsGymLeader || !IsBoss)
            {
                yield return EnableExclamation();
            }
            yield return DialogueManager.Instance.ShowDialogue(dialogue);
            GameManager.Instance.StateMachine.Pop();
            GameManager.Instance.StartTrainerBattle(this);
        }

    }

    public void BattleLost()
    {
        IsBattleLost = true;
        if (fov != null)
            fov.SetActive(false);
        if (_activateCutsceneNameWin != CutsceneName.None && !GameKeyManager.Instance.GetBoolValue(_activateCutsceneNameWin.ToString()))
        {
            GameKeyManager.Instance.SetBoolValue(_activateCutsceneNameWin.ToString(), true);
        }
    }

    private IEnumerator EnableExclamation()
    {
        exclamation.SetActive(true);
        var sequence = DOTween.Sequence();
        exclamation.transform.position = _exclamationPos;
        sequence.Append(exclamation.transform.DOMoveY(_exclamationPos.y + 1f, 0.15f).SetEase(Ease.Linear));
        sequence.Append(exclamation.transform.DOMoveY(_exclamationPos.y + 0.25f, 0.15f).SetEase(Ease.Linear));
        yield return new WaitForSeconds(0.65f);
        exclamation.transform.position = _exclamationPos;
        exclamation.SetActive(false);
    }

    public IEnumerator TriggerTrainerBattle(PlayerController player)
    {

        GameManager.Instance.StateMachine.Push(CutsceneState.I);
        if (meetBGM != BGM.NONE)
            AudioManager.Instance.PlayMusicVolume(MeetBGM);

        yield return EnableExclamation();

        // Walk towards the player
        var diff = player.transform.position - transform.position;
        var moveVec = diff - diff.normalized;
        moveVec = new Vector2(Mathf.Round(moveVec.x), Mathf.Round(moveVec.y));

        yield return character.Move(moveVec);

        // Show dialog
        yield return DialogueManager.Instance.ShowDialogue(dialogue);

        GameManager.Instance.StateMachine.Pop();
        BattleState.I.BossPokemon = null;
        GameManager.Instance.StartTrainerBattle(this);
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

        if (fov != null)
            fov.transform.eulerAngles = new Vector3(0f, 0f, angle);
    }

    public object CaptureState()
    {
        return IsBattleLost;
    }

    public void RestoreState(object state)
    {
        IsBattleLost = (bool)state;
        if (IsBattleLost)
        {
            fov.SetActive(false);
        }
    }
}
