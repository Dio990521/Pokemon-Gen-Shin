using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [Header("Enable After")]
    [SerializeField] private CutsceneName _enableAfterCutscene;

    [SerializeReference]
    [SerializeField] private List<CutsceneAction> actions;

    [SerializeField] private FacingDirection direction = FacingDirection.None;
    [SerializeField] private CutsceneName _activateCutsceneName;
    [SerializeField] private bool _isAutoPlay;

    [Header("If Always")]
    [SerializeField] private bool _isAlwaysExist;
    [SerializeField] private CutsceneName disableAfterCutscene;

    private bool _isPlaying;

    public bool TriggerRepeatedly => false;

    public IEnumerator Play()
    {
        GameManager.Instance.StateMachine.Push(CutsceneState.I);
        foreach (var action in actions)
        {
            if (action.WaitForCompletion)
            {
                yield return action.Play();
            }
            else
            {
                StartCoroutine(action.Play());
            }
            
        }
        if (!_isAlwaysExist)
        {
            GameKeyManager.Instance.SetBoolValue(_activateCutsceneName.ToString(), true);
        }
        GameManager.Instance.StateMachine.Pop();
    }

    public void AddAction(CutsceneAction action)
    {
#if UNITY_EDITOR
        Undo.RegisterCompleteObjectUndo(this, "Add action to cutscene");
#endif
        action.ActionName = action.GetType().ToString();
        actions.Add(action);
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (!_isAutoPlay)
        {
            if (!GameKeyManager.Instance.GetBoolValue(disableAfterCutscene.ToString()))
            {
                ReadyToPlay(player);
            }
        }
    }

    private void ReadyToPlay(PlayerController player)
    {
        if (_enableAfterCutscene != CutsceneName.None && !GameKeyManager.Instance.GetBoolValue(_enableAfterCutscene.ToString())) return;
        if (!GameKeyManager.Instance.GetBoolValue(_activateCutsceneName.ToString()))
        {
            if (direction == FacingDirection.None || player.Character.GetCharacterDirection() == direction)
            {
                player.Character.Animator.IsMoving = false;
                _isPlaying = true;
                StartCoroutine(Play());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isPlaying && GameManager.Instance.StateMachine.CurrentState != PauseState.I)
        {
            if (_isAutoPlay)
            {
                ReadyToPlay(collision.GetComponent<PlayerController>());
            }
        }
        
    }
}
