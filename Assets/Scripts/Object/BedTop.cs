using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BedTop : MonoBehaviour
{
    [SerializeField] private BoxCollider2D _topCollider;
    [SerializeField] private Vector2 _topColliderSmallSize;
    [SerializeField] private Vector2 _topColliderSmallOffset;
    [SerializeField] private Vector2 _topColliderLargeSize;
    [SerializeField] private Vector2 _topColliderLargeOffset;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _topCollider.size = _topColliderLargeSize;
            _topCollider.offset = _topColliderLargeOffset;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _topCollider.size = _topColliderSmallSize;
            _topCollider.offset = _topColliderSmallOffset;
        }
    }
}
