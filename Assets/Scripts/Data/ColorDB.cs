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
        };
}
