using UnityEngine;

public class cylynder : MonoBehaviour {
    public IKmovement avatar;

    void OnMouseDown()
    {
        avatar.startMoving();
    }
}