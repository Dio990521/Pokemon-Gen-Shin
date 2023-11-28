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
    [SerializeField] private BoxCollider2D _activateCollider;
    [SerializeField] private bool _isAutoPlay;

    [Header("If Always")]
    [SerializeField] private bool _isAlwaysExist;
    [SerializeField] private CutsceneName disableAfterCutscene;

    [HideInInspector]
    public static bool IsReloadConnectedScene;

    private bool _isPlaying;

    public bool TriggerRepeatedly => _isAlwaysExist;

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
            if (_activateCutsceneName != CutsceneName.None)
            {
                GameKeyManager.Instance.SetBoolValue(_activateCutsceneName.ToString(), true);
                GetComponent<BoxCollider2D>().enabled = false;
                if (_activateCollider != null)
                {
                    _activateCollider.enabled = true;
                }
            }
        }

        if (IsReloadConnectedScene)
        {
            yield return Fader.FadeIn(1f);
            yield return GameManager.Instance.RefreshScene();
            yield return Fader.FadeOut(1f);
            IsReloadConnectedScene = false;
            GameManager.Instance.StateMachine.Pop();
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
                player.StopMovingAnimation();
                _isPlaying = true;
                StartCoroutine(Play());
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !_isPlaying && GameManager.Instance.StateMachine.CurrentState == FreeRoamState.I)
        {
            if (_isAutoPlay)
            {
                ReadyToPlay(collision.GetComponent<PlayerController>());
            }
        }
        
    }
}
