using UnityEngine;

public class CollisionZoneBox : CollisionZone
{
    #region InEditorVariables
    #endregion

    public Vector3 v0, v7;
    public CollisionZonePlane[] planes = new CollisionZonePlane[6];
    Bounds bounds;

    //Initialices the box bound
    public override void initialateBound()
    {
        bounds = this.GetComponent<BoxCollider>().bounds;

        v0 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
        Vector3 v1 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
        //Vector3 v2 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
        Vector3 v3 = new Vector3(bounds.center.x + bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);
        Vector3 v4 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z + bounds.extents.z);
        //Vector3 v5 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y + bounds.extents.y, bounds.center.z - bounds.extents.z);
        Vector3 v6 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z + bounds.extents.z);
        v7 = new Vector3(bounds.center.x - bounds.extents.x, bounds.center.y - bounds.extents.y, bounds.center.z - bounds.extents.z);

        //y
        planes[0] = createPlane(v0, v1, v4);//v5
        planes[1] = createPlane(v3, v6, v7);//v2
        //x
        planes[2] = createPlane(v3, v1, v0);//v2
        planes[3] = createPlane(v7, v6, v4);//v5
        //z
        planes[4] = createPlane(v0, v4, v6);//v2
        planes[5] = createPlane(v1, v3, v7);//v5

    }

    //Given a vertex return its penalty force
    public override Vector3 calculatePenaltyForce(Vector3 vertexPenalty)
    {
        //Calculates only two vectors for detect all planes collisions
        Vector3 d0 = v0 - vertexPenalty;
        Vector3 d1 = v7 - vertexPenalty;
        Vector3 fMax = Vector3.zero;

        for (int i = 0; i < 6; i += 2)
        {
            Vector3 f0 = planes[i].calculatePenaltyForceFromVector(d0);
            Vector3 f1 = planes[i + 1].calculatePenaltyForceFromVector(d1);
            if (f0.magnitude == 0.0f || f1.magnitude == 0.0f)
            {
                return Vector3.zero;
            }
            else
            {
                fMax += maxForce(f0, f1);
            }
        }

        return Vector3.zero - fMax;
    }

    //creates one of the collision planes of the box
    private CollisionZonePlane createPlane(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        CollisionZonePlane plane = gameObject.AddComponent<CollisionZonePlane>();
        plane.initialazePlane(v0, v1, v2, penaltyConstant, offset);
        return plane;
    }

    //Reurns the maximum of two vectors
    private Vector3 maxForce(Vector3 fa, Vector3 fb)
    {
        if (fa.magnitude > fb.magnitude)
        {
            return fa;
        }
        else
        {
            return fb;
        }
    }
}
