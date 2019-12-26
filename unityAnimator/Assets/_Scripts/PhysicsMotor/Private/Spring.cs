using UnityEngine;

public class Spring
{
    #region InEditorVariables
    public float Stiffness;
    #endregion

    public PhysicsManager Manager;
    public Node nodeA;
    public Node nodeB;
    public float Length0;
    public float Length;
    public float RotationDamping;
    public float ProyectiveDamping;
    public Vector3 dir;
    public Vector3 dirNorm;
    public Vector3 forceFactor;

    private GameObject cylender;
    private float LengthMin;
    private float LengthMax;
    private float Grossor;

    // Use this for initialization in fabrics (Stiffnes)
    public Spring(Node node1, Node node2, float newStiffness, float newRotationDamping, float newProyectiveDamping, SimulatedObject.DebugDrawType debugDraw)
    {
        Grossor = 0.1f;
        CreateSpring(node1, node2, newRotationDamping, newProyectiveDamping, debugDraw);
        Stiffness = newStiffness;
    }

    // Use this for initialization in tetraedros (StiffnesDensity of nodes)
    public Spring(Node node1, Node node2, float newRotationDamping, float newProyectiveDamping, SimulatedObject.DebugDrawType debugDraw)
    {
        Grossor = 12.5f;
        CreateSpring(node1, node2, newRotationDamping, newProyectiveDamping, debugDraw);
        Stiffness = (node1.Stiffnes + node2.Stiffnes) * 0.5f;
    }

    //Auxiliar method for initialices springs in both cases
    private void CreateSpring(Node node1, Node node2, float newRotationDamping, float newProyectiveDamping, SimulatedObject.DebugDrawType debugDraw)
    {
        nodeA = node1;
        nodeB = node2;
        dir = nodeA.Pos - nodeB.Pos;
        dirNorm = dir.normalized;
        Length0 = Length = dir.magnitude;
        LengthMin = 0.3f * Length0;
        LengthMax = 3.0f * Length0;

        RotationDamping = newRotationDamping;
        ProyectiveDamping = newProyectiveDamping;
        if (debugDraw == SimulatedObject.DebugDrawType.Instantiate)
        {//If necessary, creates the debug cylinder
            cylender = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            cylender.name = "Spring: " + nodeA.Pos + " - " + nodeB.Pos;
            debugDrawing();
        }
    }

    //Updates the debug cylinder
    public void debugDrawing()
    {
        cylender.transform.position = 0.5f * (nodeA.Pos + nodeB.Pos);
        cylender.transform.localScale = new Vector3(Grossor, Length * 0.5f, Grossor);//The default length of a cylinder in Unity is 2.0
        cylender.transform.rotation = Quaternion.FromToRotation(Vector3.up, dirNorm);
    }

    //Creates the debug gizmos
    public void debugGizmos()
    {
        Gizmos.DrawLine(nodeA.Pos, nodeB.Pos);
    }

    //Compute deformative forces and damping in 1D spring
    public void ComputeForces()
    {
        calculateLength();
        Vector3 velDifference = nodeA.Vel - nodeB.Vel;
        Vector3 Force = Stiffness * ((Length0 - Length) * dirNorm - RotationDamping * velDifference - ProyectiveDamping * Vector3.Dot(dirNorm, velDifference) * dirNorm);
        nodeA.Force += Force;
        nodeB.Force -= Force;
    }

    //Compute deformative forces in 1D spring
    public void computeForceFactor()
    {
        forceFactor = (Length0 - Length) * dir * Stiffness / (6 * Length0 * Length0 * Length);
    }

    //Compute deformative forces in springs of volume tetraedros
    public void addForces(float volume)
    {
        Vector3 Force = volume * forceFactor;
        nodeA.Force += Force;
        nodeB.Force -= Force;
    }

    //Compute Damping forces
    public void ComputeDamping()
    {
        Vector3 velDifference = nodeA.Vel - nodeB.Vel;
        Vector3 Force = Stiffness * (RotationDamping * velDifference + ProyectiveDamping * Vector3.Dot(dirNorm, velDifference) * dirNorm);
        nodeA.Force -= Force;
        nodeB.Force += Force;
    }

    //Recalculate spring Length
    public void calculateLength()
    {
        dir = nodeA.Pos - nodeB.Pos;
        Length = dir.magnitude;
        dirNorm = dir.normalized;
        //Sets a range of values to avoid problems
        if (Length < LengthMin)
        {
            dir = dirNorm * LengthMin;
            Length = LengthMin;
        }
        else if (Length > LengthMax)
        {
            dir = dirNorm * LengthMax;
            Length = LengthMax;
        }
    }

}
