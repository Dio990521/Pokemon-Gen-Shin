using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassEffect : MonoBehaviour
{
    [SerializeField] private GameObject grassStepEffect;
    public void InitEffect()
    {
        Instantiate(grassStepEffect, transform);
    }
}
