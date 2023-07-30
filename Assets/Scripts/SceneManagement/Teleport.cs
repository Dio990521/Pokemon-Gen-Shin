using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Teleport : MonoBehaviour, InteractableObject
{
    [SerializeField] private int _index;
    private bool _isActive;
    private AnimatedSprite _animatedSprite;
    private SpriteRenderer _spriteRenderer;

    public bool IsActive { get { return _isActive; } }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = Color.grey;
        _animatedSprite = GetComponent<AnimatedSprite>();
        _animatedSprite.enabled = false;
    }

    private void Start()
    {
        _isActive = TeleportManager.Instance.Teleports[_index].IsActive;
        if (_isActive)
        {
            _animatedSprite.enabled = true;
            _spriteRenderer.color = Color.white;
        }
    }

    public IEnumerator Interact(Transform initiator)
    {
        if (!_isActive)
        {
            _isActive = true;
            TeleportManager.Instance.Teleports[_index].IsActive = true;
            _animatedSprite.enabled = true;
            _spriteRenderer.color = Color.white;
            AudioManager.Instance.PlaySE(SFX.ACTIVATE_TELEPORT, true);
            yield return DialogueManager.Instance.ShowDialogueText("已激活传送锚点。");
            yield break;
        }

        int selectedChoice = 0;
        List<string> teleports = TeleportManager.Instance.GetActiveList();
        List<int> indices = TeleportManager.Instance.GetActiveTeleportIndex();
        if (teleports.Count == 1)
        {
            yield return DialogueManager.Instance.ShowDialogueText("已激活的传送锚点只有这里！");
            yield break;
        }
        yield return DialogueManager.Instance.ShowDialogueText($"要传送到哪里呢？",
            choices: teleports,
            onChoiceSelected: (selection) => selectedChoice = selection);

        if (selectedChoice != teleports.Count - 1)
        {
            yield return StartTeleport(TeleportManager.Instance.Teleports[indices[selectedChoice]].SpawnPoint, initiator.GetComponent<PlayerController>());
        }

    }

    private IEnumerator StartTeleport(Vector2 telePos, PlayerController player)
    {
        GameManager.Instance.PauseGame(true);
        AudioManager.Instance.PlaySE(SFX.TELEPORT);
        yield return Fader.FadeIn(1f);

        player.Character.SetPositionAndSnapToTile(telePos);
        player.Character.Animator.SetFacingDirection(FacingDirection.Down);
        yield return new WaitForSeconds(1f);
        yield return Fader.FadeOut(1f);
        GameManager.Instance.StartFreeRoamState();

    }

}
