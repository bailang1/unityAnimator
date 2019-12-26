using System.Collections.Generic;
using UnityEngine;

public abstract class SimulatedObject : MonoBehaviour
{
    //This extra class makes easy had more onjects in the same scene sharing components

    #region InEditorVariables
    public float defaultDampingOfNodesRelativeToMass;
    public float defaultDampingOfRotationRelativeToStiffnes;
    public float defaultDampingOfProyectiveRelativeToStiffnes;
    public bool defaultFixed;
    public DebugDrawType debugDraw;
    #endregion

    public PhysicsManager Manager;

    protected Mesh mesh;
    protected Node[] nodes;
    protected List<Spring> springs = new List<Spring>();
    protected Vector3[] vertices;

    public enum DebugDrawType
    {
        None = 0,
        Gizmos = 1,
        Instantiate = 2
    };



    // Use this for initialization
    public abstract void loadData(PhysicsManager manager);

    //Call in update to update mesh
    public abstract void updateMesh();

    //Redraw debug gizmos
    void OnDrawGizmos()
    {
        if (debugDraw == SimulatedObject.DebugDrawType.Gizmos && Application.isPlaying)
        {
            foreach (Node node in nodes)
            {
                node.debugGizmos();
            }
            foreach (Spring spring in springs)
            {
                spring.debugGizmos();
            }
        }
    }

    //Recalc nodes forces
    protected abstract void VertexRecalc();

    //Recalc nodes positions explict
    public virtual void VertexReCalcExplicit()
    {
        VertexRecalc();
        for (int i = 0; i < nodes.Length; i++)
        {
            if (!nodes[i].Fixed)
            {
                nodes[i].Pos += nodes[i].Vel * Manager.TimeStep;
                nodes[i].Vel += nodes[i].Force * Manager.TimeStep / nodes[i].Mass;
            }
        }
    }

    //Recalc nodes positions symplectic
    public virtual void VertexReCalcSymplectic()
    {
        VertexRecalc();
        for (int i = 0; i < nodes.Length; i++)
        {
            if (!nodes[i].Fixed)
            {
                nodes[i].Vel += nodes[i].Force * Manager.TimeStep / nodes[i].Mass;
                nodes[i].Pos += nodes[i].Vel * Manager.TimeStep;
                //print(i + ": mass:"+ nodes[i].Mass + " vel: " + nodes[i].Vel + " pos: " + nodes[i].Pos+ " force: "+ nodes[i].Force);
            }
        }
    }

}