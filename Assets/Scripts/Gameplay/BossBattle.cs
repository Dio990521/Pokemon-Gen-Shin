using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class BossBattle : MonoBehaviour, IPlayerTriggerable
{
    [SerializeField] private Pokemon boss;
    [SerializeField] private BattleTrigger battleTrigger;
    [SerializeField] private CutsceneName activateCutsceneName;
    public bool TriggerRepeatedly => true;

    private void Start()
    {
        if (activateCutsceneName != CutsceneName.None && GameKeyManager.Instance.GetBoolValue(activateCutsceneName.ToString()))
        {
            gameObject.GetComponent<BoxCollider2D>().enabled = false;
            gameObject.GetComponent<AnimatedSprite>().enabled = false;
            gameObject.GetComponent<SpriteRenderer>().enabled = false;
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
        yield return DialogueManager.Instance.ShowDialogueText("强大的气息笼罩着周围。");
        yield return DialogueManager.Instance.ShowDialogueText($"是否要挑战{boss.PokemonBase.PokemonName}（等级：{boss.Level}）？", autoClose: false);
        ChoiceState.I.Choices = new List<string>() { "占戈！", "没准备好！" };
        yield return GameManager.Instance.StateMachine.PushAndWait(ChoiceState.I);

        int selectedChoice = ChoiceState.I.Selection;

        if (selectedChoice == 0)
        {
            GameManager.Instance.StartBattle(battleTrigger, boss, activateCutsceneName, true);
        }
        else
        {
            var dir = new Vector3(player.Character.Animator.MoveX, player.Character.Animator.MoveY);
            yield return player.Character.Move(-dir, checkCollisions: false);
        }
    }
}
