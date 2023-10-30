using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportPlayerAction : CutsceneAction
{
    [SerializeField] private GameObject _contextCutscene;
    [SerializeField] private Vector2 position;
    [SerializeField] private FacingDirection facingDir;

    public override IEnumerator Play()
    {
        yield return Teleport.StartTeleport(position, GameManager.Instance.PlayerController, facingDir, _contextCutscene, false);
    }
}
