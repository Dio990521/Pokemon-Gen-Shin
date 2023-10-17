using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDB
{
    public static Dictionary<ConditionID, Color> statusColors = new Dictionary<ConditionID, Color>()
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

    public static Dictionary<string, Color> textColors = new Dictionary<string, Color>()
    {
        { "NotChoice", new Color32(50,50,50,255) },
        { "OnChoice", new Color32(114,90,255,255) }
    };
}
