using UnityEngine;

public class Move
{
    public MoveBase MoveBase { get; set; }
    public int PP { get; set; }

    public Move(MoveBase moveBase)
    {
        MoveBase = moveBase;
        PP = moveBase.PP;
    }

    public Move(MoveSaveData saveData)
    {
        MoveBase = MoveDB.GetObjectByName(saveData.name);
        PP = saveData.pp;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = MoveBase.name,
            pp = PP
        };

        return saveData;
    }

    public void IncreasePP(int amount)
    {
        PP = Mathf.Clamp(PP + amount, 0, MoveBase.PP);
    }
}
