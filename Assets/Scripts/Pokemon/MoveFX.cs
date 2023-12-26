using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move FX")]
public class MoveFX : ScriptableObject
{
    [SerializeField] private List<Sprite> moveEffectSprites;
    [SerializeField] private ExBGM _moveBGM;
    [SerializeField] private AudioClip _moveSfx;
    [SerializeField] private bool _isBottom;
    [SerializeField] private Vector2 _posOffset;
    [SerializeField] private float _scaleFactor = 1f;


    public List<Sprite> MoveEffectSprites { get => moveEffectSprites; set => moveEffectSprites = value; }
    public ExBGM MoveBGM { get => _moveBGM; set => _moveBGM = value; }
    public Vector2 PosOffset { get => _posOffset; set => _posOffset = value; }
    public AudioClip MoveSfx { get => _moveSfx; set => _moveSfx = value; }
    public float ScaleFactor { get => _scaleFactor; set => _scaleFactor = value; }
    public bool IsBottom { get => _isBottom; set => _isBottom = value; }
}