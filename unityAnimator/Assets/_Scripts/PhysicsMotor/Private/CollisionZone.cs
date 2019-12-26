using UnityEngine;

public abstract class CollisionZone : MonoBehaviour
{
    //Abstract Class For Plane, Box and Sphere collisionZone

    #region InEditorVariables
    public float penaltyConstant;
    public float offset;
    #endregion

    //Initialices box bound
    public abstract void initialateBound();

    //Given a vertex return its penalty force
    public abstract Vector3 calculatePenaltyForce(Vector3 vertexPenalty);

}
