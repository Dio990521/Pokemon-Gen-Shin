using System.Diagnostics;

public class EffectivenessChart
{
    private static float[][] chart =
    {
        //                    Slp   Frz   Psn   Brn   Par   Cfs  Kuosan  Jiejing  Zhengfa  Ronghua  Zhanfang
        /*超载*/ new float[] {  1f,  1f,   0.5f,   1f,   1f,   1f,   1f,     1f,      1.5f,      1f,       1f  },
        /*燃烧*/ new float[] {  1f,  1f,   1f,   0.5f,   1f,   1f,   1f,     1f,      1f,      1.5f,       1f  },
        /*睡眠*/ new float[] {  0.5f,  1f,   1f,   1f,   1f,   1f,   1f,     1f,      1f,      1f,       1.5f  },
        /*麻痹*/ new float[] {  1.5f,  1f,   1f,   1f,   0.5f,   1f,   1f,     1f,      1f,      1f,       1f  },
        /*冻结*/ new float[] {  1f,  0.5f,   1.5f,   1f,   1f,   1f,   1f,     1f,      1f,      1f,       1f  },
        /*混乱*/ new float[] {  1f,  1f,   1f,   1.5f,   1f,   0.5f,   1f,     1f,      1f,      1f,       1f  },
        /*结晶*/ new float[] {  1f,  1f,   1f,   1f,   1.5f,   1f,   1f,     0.5f,      1f,      1f,       1f  },
        /*绽放*/ new float[] {  1f,  1f,   1f,   1f,   1f,   1.5f,   1f,     1f,      1f,      1f,       0.5f  },
        /*蒸发*/ new float[] {  1f,  1f,   1f,   1f,   1f,   1f,   1.5f,     1f,      0.5f,      1f,       1f  },
        /*融化*/ new float[] {  1f,  1f,   1f,   1f,   1f,   1f,   1f,     1.5f,      1f,      0.5f,       1f  },
        /*扩散*/ new float[] {  1f,  1.5f,   1f,   1f,   1f,   1f,   0.5f,     1f,      1f,      1f,       1f  },
    };

    // Return the effectiveness of the used move
    public static float GetEffectiveness(ConditionID elementReaction, PassiveMoveType passiveMove)
    {
        if (elementReaction == ConditionID.none || passiveMove == PassiveMoveType.None) return 1f;

        int row = (int)elementReaction;
        int col = (int)passiveMove;
        return chart[row][col];
    }
}

public enum PokemonType
{
    普通,
    水,
    火,
    草,
    冰,
    雷,
    岩,
    风,
    None
}