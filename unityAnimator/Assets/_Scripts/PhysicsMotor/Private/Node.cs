using UnityEngine;

public class Node
{
    #region InEditorVariables
    #endregion

    public float Mass;
    public float Density;
    public float Damping;
    public float Stiffnes;//for parse in tetraedros
    public bool Fixed;
    public PhysicsManager Manager;

    public Vector3 Pos;
    public Vector3 Vel;
    public Vector3 Force;
    private GameObject sphere;
    private float sphereRadius;



    // Use this for initialization in fabrics (mass)
    public Node(Vector3 vertexPos, PhysicsManager physicsManager, float newMass, float newDampingRelativeToMass, bool isFixed, SimulatedObject.DebugDrawType debugDraw)
    {
        Mass = newMass;
        sphereRadius = 0.2f;
        createNode(vertexPos, physicsManager, newDampingRelativeToMass, isFixed, debugDraw);
    }

    // Use this for initialization in tetraedros (density)
    public Node(Vector3 vertexPos, PhysicsManager physicsManager, float newDensity, float newDampingRelativeToMass, bool isFixed, SimulatedObject.DebugDrawType debugDraw, float auxiliarStiffnes)
    {
        Density = newDensity;
        Mass = 0.0f;
        Stiffnes = auxiliarStiffnes;
        sphereRadius = 80.0f;
        createNode(vertexPos, physicsManager, newDampingRelativeToMass, isFixed, debugDraw);
    }

    //Auxiliar method for initialices springs in both cases
    private void createNode(Vector3 vertexPos, PhysicsManager physicsManager, float newDampingRelativeToMass, bool isFixed, SimulatedObject.DebugDrawType debugDraw)
    {
        Pos = vertexPos;
        Manager = physicsManager;
        Vel = Force = Vector3.zero;
        Fixed = isFixed;
        Damping = newDampingRelativeToMass;
        if (debugDraw == SimulatedObject.DebugDrawType.Instantiate)
        {
            sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(sphereRadius, sphereRadius, sphereRadius);
            sphere.name = "nodo " + Pos;
            debugDrawing();
        }
    }

    //Updates the debug sphere
    public void debugDrawing()
    {
        sphere.transform.position = this.Pos;
    }

    //Creates the debug gizmos
    public void debugGizmos()
    {
        Gizmos.DrawSphere(this.Pos, this.sphereRadius);
    }

    //Add gravity force and checks if its in a collision
    public void ComputeForces()
    {
        Force += Mass * (Manager.Gravity - Vel * Damping);
        if (Manager.collisionZones.Count > 0)
        {
            foreach (CollisionZone colZone in Manager.collisionZones)
            {
                Force += colZone.calculatePenaltyForce(this.Pos);
            }
        }
    }
}
