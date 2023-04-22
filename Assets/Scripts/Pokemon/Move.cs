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
        MoveBase = MoveDB.GetMoveByName(saveData.name);
        PP = saveData.pp;
    }

    public MoveSaveData GetSaveData()
    {
        var saveData = new MoveSaveData()
        {
            name = MoveBase.MoveName,
            pp = PP
        };

        return saveData;
    }
}
