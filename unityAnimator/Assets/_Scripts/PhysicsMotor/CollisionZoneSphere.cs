using UnityEngine;

public class CollisionZoneSphere : CollisionZone
{
    #region InEditorVariables
    #endregion

    protected SphereCollider sphereCollider;
    private Vector3 sphereCenter;
    private float sphereRadius;

    public override void initialateBound()
    {//Initialices the bound of the sphere collision
        this.sphereCollider = this.transform.GetComponent<SphereCollider>();
        sphereCenter = transform.TransformPoint(sphereCollider.center);
        sphereRadius = transform.TransformVector(sphereCollider.radius * Vector3.up).magnitude;
    }

    //Calculate thePenalty Force given and vertex
    public override Vector3 calculatePenaltyForce(Vector3 vertexPenalty)
    {
        Vector3 distanceVector = (sphereCenter - vertexPenalty);
        float distance = distanceVector.magnitude;
        float penetration = offset - distance + sphereRadius;
        if (penetration > 0.0f)
        {
            Vector3 normal = distanceVector.normalized;
            Vector3 Force = -normal * penaltyConstant * penetration * penetration;
            //print ("PenaltyForce of:"+Force);
            return Force;

        }
        else
        {
            return Vector3.zero;
        }
    }

}
