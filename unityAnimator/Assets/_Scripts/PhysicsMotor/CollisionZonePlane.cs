using UnityEngine;

public class CollisionZonePlane : CollisionZone
{
    #region InEditorVariables
    #endregion

    public Vector3[] vectors = new Vector3[3];
    public Vector3 normal;

    //Return the penalty force given a vertex
    public override Vector3 calculatePenaltyForce(Vector3 vertexPenalty)
    {
        return calculatePenaltyForceFromVector(vectors[0] - vertexPenalty);
    }

    //Initilices the bound object
    public override void initialateBound()
    {
        loadComponent();
        recalcNormal();
    }

    // Use this for initialices the planes
    public void initialazePlane(Vector3 vector0, Vector3 vector1, Vector3 vector2, float penaltyConst, float offsetLimit)
    {
        vectors[0] = vector0;
        vectors[1] = vector1;
        vectors[2] = vector2;
        this.penaltyConstant = penaltyConst;
        this.offset = offsetLimit;
        recalcNormal();
    }

    //Reclculates the normal of the plane
    public void recalcNormal()
    {
        normal = Vector3.Cross(vectors[0] - vectors[1], vectors[0] - vectors[2]).normalized;
    }

    //Used for better performance in box colliders
    public Vector3 calculatePenaltyForceFromVector(Vector3 vectorDistancePenalty)
    {
        float distancePerpendicular = Vector3.Dot(vectorDistancePenalty, normal) + offset;
        if (distancePerpendicular > 0.0f)
        {
            Vector3 force = penaltyConstant * distancePerpendicular * normal;
            //print(force);
            return force;
        }
        else
        {
            return Vector3.zero;
        }
    }

    // Use this for load components
    void loadComponent()
    {
        Mesh mesh = this.transform.GetComponent<MeshFilter>().mesh;
        if (mesh.vertexCount != 4)
            print("ERROR: not a Quad with 4 vertex");
        else
        {
            for (int i = 0; i < 3; i++)
            {
                vectors[i] = transform.TransformPoint(mesh.vertices[i]);
            }
        }
    }

}
