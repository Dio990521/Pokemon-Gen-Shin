public class Move
{
    public MoveBase MoveBase { get; set; }
    public int PP { get; set; }

    public Move(MoveBase moveBase)
    {
        MoveBase = moveBase;
        PP = moveBase.PP;
    }
}
