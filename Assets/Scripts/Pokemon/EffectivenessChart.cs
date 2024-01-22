using System.Diagnostics;

public class ElementReactionUtil
{

    public static PassiveMoveType ConditionIDToPassiveType(ConditionID conditionID)
    {
        switch (conditionID)
        {
            case ConditionID.psn:
                return PassiveMoveType.Psn;
            case ConditionID.brn:
                return PassiveMoveType.Brn;
            case ConditionID.kuosan:
                return PassiveMoveType.Kuosan;
            case ConditionID.jiejing:
                return PassiveMoveType.Jiejing;
            case ConditionID.zhanfang:
                return PassiveMoveType.Zhanfang;
            case ConditionID.zhengfa:
                return PassiveMoveType.Zhengfa;
            case ConditionID.ronghua:
                return PassiveMoveType.Ronghua;
            case ConditionID.frz:
                return PassiveMoveType.Frz;
            case ConditionID.par:
                return PassiveMoveType.Par;
            case ConditionID.confusion:
                return PassiveMoveType.Cfs;
            case ConditionID.slp:
                return PassiveMoveType.Slp;
        }
        return PassiveMoveType.None;
    }

    public static string GetPassiveString(PassiveMoveType passiveMoveType)
    {
        switch (passiveMoveType)
        {
            case PassiveMoveType.Psn:
                return "超载";
            case PassiveMoveType.Brn:
                return "燃烧";
            case PassiveMoveType.Kuosan:
                return "扩散";
            case PassiveMoveType.Jiejing:
                return "结晶";
            case PassiveMoveType.Zhanfang:
                return "绽放";
            case PassiveMoveType.Zhengfa:
                return "蒸发";
            case PassiveMoveType.Ronghua:
                return "融化";
            case PassiveMoveType.Frz:
                return "冻结";
            case PassiveMoveType.Par:
                return "麻痹";
            case PassiveMoveType.Cfs:
                return "混乱";
            case PassiveMoveType.Slp:
                return "睡眠";
        }
        return "";
    }

    public static PokemonType ConditionIDToPokemonType(ConditionID conditionID)
    {
        switch (conditionID)
        {
            case ConditionID.hydro:
                return PokemonType.水;
            case ConditionID.pyro:
                return PokemonType.火;
            case ConditionID.electro:
                return PokemonType.雷;
            case ConditionID.anemo:
                return PokemonType.风;
            case ConditionID.geo:
                return PokemonType.岩;
            case ConditionID.dendro:
                return PokemonType.草;
            case ConditionID.cryo:
                return PokemonType.冰;
        }
        return PokemonType.None;
    }

    public static ConditionID PokemonTypeToConditionID(PokemonType type)
    {
        switch (type)
        {
            case PokemonType.水:
                return ConditionID.hydro;
            case PokemonType.火:
                return ConditionID.pyro;
            case PokemonType.雷:
                return ConditionID.electro;
            case PokemonType.风:
                return ConditionID.anemo;
            case PokemonType.岩:
                return ConditionID.geo;
            case PokemonType.草:
                return ConditionID.dendro;
        }
        return ConditionID.cryo;
    }

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
    None,
    状态
}