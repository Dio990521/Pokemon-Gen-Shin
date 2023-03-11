using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLayers : MonoBehaviour
{
    [SerializeField] private LayerMask objectMask;
    [SerializeField] private LayerMask grassMask;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private LayerMask fovLayer;

    public static GameLayers instance {  get; private set; }

    private void Awake()
    {
        instance = this;
    }

    public LayerMask ObjectMask
    {
        get => objectMask;
    }

    public LayerMask GrassMask
    {
        get => grassMask;
    }

    public LayerMask InteractableLayer
    {
        get => interactableLayer;
    }

    public LayerMask PlayerLayer
    {
        get => playerLayer;
    }

    public LayerMask FovLayer
    {
        get => fovLayer;
    }
}
