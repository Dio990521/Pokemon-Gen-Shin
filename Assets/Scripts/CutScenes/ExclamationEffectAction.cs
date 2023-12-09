using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExclamationEffectAction : CutsceneAction
{
    [SerializeField] private GameObject exclamation;

    public override IEnumerator Play()
    {
        exclamation.SetActive(true);
        var sequence = DOTween.Sequence();
        sequence.Append(exclamation.transform.DOMoveY(exclamation.transform.position.y + 1f, 0.15f).SetEase(Ease.Linear));
        sequence.Append(exclamation.transform.DOMoveY(exclamation.transform.position.y + 0.15f, 0.15f).SetEase(Ease.Linear));
        yield return new WaitForSeconds(0.4f);
        exclamation.SetActive(false);
    }
}
