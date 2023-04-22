using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDB
{
    static Dictionary<string, MoveBase> moves;

    public static void Init()
    {
        moves = new Dictionary<string, MoveBase>();

        var moveList = Resources.LoadAll<MoveBase>("Prefabs/Moves");
        foreach (var move in moveList)
        {
            if (moves.ContainsKey(move.MoveName))
            {
                Debug.LogError($"There are 2 moves with the same name {move.MoveName}");
                continue;
            }

            moves[move.MoveName] = move;
        }
    }

    public static MoveBase GetMoveByName(string name)
    {
        if (!moves.ContainsKey(name))
        {
            Debug.LogError($"Move not found with the name {name}");
            return null;
        }

        return moves[name];
    }
}
