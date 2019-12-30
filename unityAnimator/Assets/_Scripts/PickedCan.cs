using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickedCan : MonoBehaviour {
    public IKMovement avatar;

    void OnMouseDown()
    {
        avatar.startMoving();
    }
}