public class TypeChart
{
    private static float[][] chart =
    {
        //                    普    水    火    草    冰    雷    岩    风
        /*普*/ new float[] {  1f,  1f,   1f,   1f,   1f,   1f,   1f,  1f},
        /*水*/ new float[] {  1f, 0.5f,  2f, 0.5f, 0.5f,   1f,   2f,  1f},
        /*火*/ new float[] {  1f, 0.5f, 0.5f,  2f,   2f, 0.5f,   1f,  1f},
        /*草*/ new float[] {  1f,   2f, 0.5f, 0.5f,0.5f,   2f,   1f,  1f},
        /*冰*/ new float[] {  1f,   2f, 0.5f,  2f, 0.5f,   1f,   1f, 0.5f},
        /*雷*/ new float[] {  1f,   1f,   2f, 0.5f,  1f, 0.5f, 0.5f,  2f},
        /*岩*/ new float[] {  1f, 0.5f,   1f,  1f,   1f,   2f, 0.5f,  2f},
        /*风*/ new float[] {  1f,   1f,   1f,  1f,   2f, 0.5f,   2f, 0.5f}
    };

    // Return the effectiveness of the used move
    public static float GetEffectiveness(PokemonType attackType, PokemonType defenseType)
    {
        if (attackType == PokemonType.None || defenseType == PokemonType.None) return 1f;

        int row = (int) attackType;
        int col = (int) defenseType;

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