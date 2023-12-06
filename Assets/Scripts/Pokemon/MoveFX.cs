using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Move", menuName = "Pokemon/Create new move FX")]
public class MoveFX : ScriptableObject
{
    [SerializeField] private List<Sprite> moveEffectSprites;
    public List<Sprite> MoveEffectSprites { get => moveEffectSprites; set => moveEffectSprites = value; }

}
