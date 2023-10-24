using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] private List<CutsceneAction> actions;

    [SerializeField] private FacingDirection direction = FacingDirection.None;
    [SerializeField] private CutsceneName cutsceneName;
    [SerializeField] private bool _isAutoPlay;
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
        GameKeyManager.Instance.SetBoolValue(cutsceneName.ToString(), true);
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
            ReadyToPlay(player);
        }
    }

    private void ReadyToPlay(PlayerController player)
    {
        if (!GameKeyManager.Instance.GetBoolValue(cutsceneName.ToString()))
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
        if (collision.CompareTag("Player") && !_isPlaying)
        {
            if (_isAutoPlay)
            {
                ReadyToPlay(collision.GetComponent<PlayerController>());
            }
        }
        
    }
}
