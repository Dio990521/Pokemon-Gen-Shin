using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InteractableObject
{

    public IEnumerator Interact(Transform initiator);
}
