using System.Collections.Generic;
using UnityEngine;

public class ColorDB
{
    public static Dictionary<ConditionID, Color> StatusColors = new Dictionary<ConditionID, Color>()
        {
            { ConditionID.psn, new Color32(165, 91, 233, 255) },
            { ConditionID.brn, new Color32(176, 70, 24, 255) },
            { ConditionID.slp, new Color32(100, 137, 34, 255) },
            { ConditionID.par, new Color32(168, 224, 31, 255) },
            { ConditionID.frz, new Color32(45, 65, 255, 255) },
            { ConditionID.hydro, new Color32(89, 195, 251, 255) },
            { ConditionID.pyro, new Color32(211, 88, 36, 255) },
            { ConditionID.electro, new Color32(140, 79, 236, 255) },
            { ConditionID.dendro, new Color32(109, 163, 63, 255) },
            { ConditionID.cryo, new Color32(74, 201, 234, 255) },
            { ConditionID.jiejing, new Color32(215, 168, 0, 255) },
            { ConditionID.confusion, new Color32(200, 155, 5, 255) },
            { ConditionID.shihua, Color.grey },
        };

    public static Dictionary<PokemonType, Color> ElementColors = new Dictionary<PokemonType, Color>()
        {
            { PokemonType.Ë®, new Color32(78, 156, 212, 255) },
            { PokemonType.»ð, new Color32(172, 50, 50, 255) },
            { PokemonType.±ù, new Color32(116, 200, 182, 255) },
            { PokemonType.²Ý, new Color32(152, 208, 13, 255) },
            { PokemonType.·ç, new Color32(111, 194, 157, 255) },
            { PokemonType.ÑÒ, new Color32(215, 168, 0, 255) },
            { PokemonType.À×, new Color32(168, 106, 213, 255) },
            { PokemonType.ÆÕÍ¨, Color.white },
            { PokemonType.×´Ì¬, new Color32(115, 166, 153, 255) },
        };

    public static Dictionary<string, Color> textColors = new Dictionary<string, Color>()
    {
        { "NotChoice", new Color32(50,50,50,255) },
        { "OnChoice", new Color32(114,90,255,255) }
    };
}
