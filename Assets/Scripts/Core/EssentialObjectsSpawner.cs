using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EssentialObjectsSpawner : MonoBehaviour
{
    [SerializeField] private GameObject essentialObjectsPrefeb;

    private void Awake()
    {
        var existingObjects = FindObjectsOfType<EssentialObjects>();
        if (existingObjects.Length == 0)
        {
            Instantiate(essentialObjectsPrefeb, new Vector3(0, 0, 0), Quaternion.identity);
        }
    }
}
