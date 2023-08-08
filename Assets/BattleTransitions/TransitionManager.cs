using UnityEngine;
using System.Collections;
using DG.Tweening;

[ExecuteInEditMode]
public class TransitionManager : MonoBehaviour
{
    [SerializeField] private Material transitionMaterial;
    [SerializeField] private Texture2D topbottomTransition;
    [SerializeField] private Texture2D wildBattleTransition;
    [SerializeField] private Texture2D trainerBattleTransition;
    [SerializeField] private Texture2D bossBattleTransition;
    [SerializeField] private Texture2D superBossBattleTransition;

    private float _cutoff;
    private float _fade;
    private bool _isTransitting;

    private void Awake()
    {
        _cutoff = transitionMaterial.GetFloat("_Cutoff");
        _fade = transitionMaterial.GetFloat("_Fade");
    }

    void OnRenderImage(RenderTexture src, RenderTexture dst)
    {
        if (transitionMaterial != null)
        {
            Graphics.Blit(src, dst, transitionMaterial);
        }
    }

    public void SetDistort(float distort)
    {
        transitionMaterial.SetFloat("_Distort", distort);
    }

    public IEnumerator StartTransition(TransitionType transitionType, float duration)
    {
        switch (transitionType)
        {
            case TransitionType.TopBottom:
                _cutoff = 1;
                _fade = 1;
                SetDistort(0f);
                transitionMaterial.SetTexture("_TransitionTex", topbottomTransition);
                yield return DOTween.To(() => _cutoff, x => _cutoff = x, 0f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
                break;
            case TransitionType.WildBattle:
                yield return Glitter(0.1f);
                _cutoff = 0;
                _fade = 1;
                SetDistort(0f);
                transitionMaterial.SetTexture("_TransitionTex", wildBattleTransition);
                yield return DOTween.To(() => _cutoff, x => _cutoff = x, 1f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
                break;
            case TransitionType.TrainerBattle:
                yield return Glitter(0.1f);
                _cutoff = 0;
                _fade = 1;
                SetDistort(1f);
                transitionMaterial.SetTexture("_TransitionTex", trainerBattleTransition);
                yield return DOTween.To(() => _cutoff, x => _cutoff = x, 1f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
                break;
            case TransitionType.BossBattle:
                _cutoff = 0;
                _fade = 1;
                SetDistort(0f);
                transitionMaterial.SetTexture("_TransitionTex", bossBattleTransition);
                yield return DOTween.To(() => _cutoff, x => _cutoff = x, 1f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
                break;
            case TransitionType.SuperBossBattle:
                _cutoff = 0;
                _fade = 1;
                SetDistort(0f);
                transitionMaterial.SetTexture("_TransitionTex", superBossBattleTransition);
                yield return DOTween.To(() => _cutoff, x => _cutoff = x, 1f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
                break;
        }
    }

    private IEnumerator Glitter(float duration)
    {
        transitionMaterial.SetTexture("_TransitionTex", topbottomTransition);
        _cutoff = 1;
        _fade = 0;
        transitionMaterial.SetColor("_Color", Color.grey);
        transitionMaterial.SetFloat("_Fade", 0f);
        yield return DOTween.To(() => _fade, x => _fade = x, 0.5f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
        yield return new WaitForSeconds(duration);
        yield return DOTween.To(() => _fade, x => _fade = x, 0f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
        yield return new WaitForSeconds(duration);
        yield return DOTween.To(() => _fade, x => _fade = x, 0.5f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
        yield return new WaitForSeconds(duration);
        yield return DOTween.To(() => _fade, x => _fade = x, 0f, duration)
                    .SetEase(Ease.Linear)
                    .OnUpdate(UpdateProperties);
        yield return new WaitForSeconds(duration);
        transitionMaterial.SetColor("_Color", Color.black);

    }

    public void ClearTransition(bool update=false)
    {
        _cutoff = 0;
        _fade = 1;
        if (update)
        {
            UpdateProperties();
        }
    }

    private void UpdateProperties()
    {
        transitionMaterial.SetFloat("_Cutoff", _cutoff);
        transitionMaterial.SetFloat("_Fade", _fade);
    }

}

public enum TransitionType { TopBottom, WildBattle, TrainerBattle, BossBattle, SuperBossBattle }
