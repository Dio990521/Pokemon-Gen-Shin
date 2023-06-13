using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CutsceneAction
{
    [SerializeField] private string actionName;
    [SerializeField] private bool waitForCompletion = true;

    public bool WaitForCompletion => waitForCompletion;

    public virtual IEnumerator Play()
    {
        yield break;
    }

    public string ActionName { get; set; }
}
