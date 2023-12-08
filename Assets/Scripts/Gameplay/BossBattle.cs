using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BossBattle : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Pokemon boss;
    [SerializeField] private BattleTrigger battleTrigger;
    [SerializeField] private CutsceneName activateCutsceneName;
    [SerializeField] private BoxCollider2D cutsceneCollider;
    [SerializeField] private BossType _bossType;
    public bool TriggerRepeatedly => true;

    private void Start()
    {
        if (activateCutsceneName != CutsceneName.None && GameKeyManager.Instance.GetBoolValue(activateCutsceneName.ToString()))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<AnimatedSprite>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            BattleSystem.OnBattleOver -= ActivateCutscene;
        }
    }

    private void ActivateCutscene(bool won)
    {
        if (cutsceneCollider != null && won)
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<AnimatedSprite>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
            cutsceneCollider.enabled = true;
            BattleSystem.OnBattleOver -= ActivateCutscene;
        }
    }

    public void OnPlayerTriggered(PlayerController player)
    {
        if (activateCutsceneName != CutsceneName.None && !GameKeyManager.Instance.GetBoolValue(activateCutsceneName.ToString()))
        {
            StartCoroutine(BossFight(player));
        }
    }

    private IEnumerator BossFight(PlayerController player)
    {
        player.StopMovingAnimation();
        yield return DialogueManager.Instance.ShowDialogueText("ǿ�����Ϣ��������Χ��");
        yield return DialogueManager.Instance.ShowDialogueText($"�Ƿ�Ҫ��ս{boss.PokemonBase.PokemonName}���ȼ���{boss.Level}����", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "ռ�꣡", "û׼���ã�" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            BattleSystem.OnBattleOver += ActivateCutscene;
            GameManager.Instance.StartBossBattle(battleTrigger, boss, _bossType, activateCutsceneName, true);
        }
        else
        {
            var dir = new Vector3(player.Character.Animator.MoveX, player.Character.Animator.MoveY);
            yield return player.Character.Move(-dir, checkCollisions: false);
        }
    }
}
