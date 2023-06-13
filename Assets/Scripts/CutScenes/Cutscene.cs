using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Cutscene : MonoBehaviour, IPlayerTriggerable
{
    [SerializeReference]
    [SerializeField] private List<CutsceneAction> actions;

    public bool TriggerRepeatedly => false;

    public IEnumerator Play()
    {
        GameManager.Instance.StartCutsceneState();
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
        GameManager.Instance.StartFreeRoamState();
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
        player.Character.IsMoving = false;
        StartCoroutine(Play());
    }
}
