using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cutscene : MonoBehaviour
{
    [SerializeReference]
    [SerializeField] private List<CutsceneAction> actions;

    public void AddAction(CutsceneAction action)
    {
        action.ActionName = action.GetType().ToString();
        actions.Add(action);
    }
}
