using UnityEngine;

public class Tetraedro
{
    //Class used in tetraedros for draw the debug mesh and calculate the wind force

    #region InEditorVariables
    #endregion

    public Node[] nodes = new Node[4];
    public Spring[] springs = new Spring[6];
    /*
     Spring order:
     0 - 1
     0 - 2
     0 - 3
     1 - 2
     1 - 3
     2 - 3
         */

    private GameObject tetraedro;
    private Mesh tetraedroMesh;
    private Vector3[] initialNormals;
    private float[] initialDot;
    private float volume;
    private Vector3[] vertices = new Vector3[4];

    public Tetraedro() { }

    //Initialices the tetraedro
    public void initData(TetraedroObject.DebugTetraedrosType debugDrawing)
    {
        if (debugDrawing == TetraedroObject.DebugTetraedrosType.Complete)
        {//Creates the object to debug draw it 
            tetraedro = GameObject.CreatePrimitive(PrimitiveType.Quad);
            tetraedro.name = ("Tetraedro: { " + nodes[0].Pos + " - " + nodes[1].Pos + " - " + nodes[2].Pos + " - " + nodes[2].Pos + " }");
            tetraedroMesh = new Mesh();

            tetraedro.GetComponent<MeshFilter>().mesh = tetraedroMesh;


            for (int i = 0; i < 4; i++)
            {
                vertices[i] = nodes[i].Pos;
            }
            tetraedroMesh.vertices = vertices;

            int[] tri = new int[12];

            tri[0] = 0;
            tri[1] = 2;
            tri[2] = 1;

            tri[3] = 2;
            tri[4] = 3;
            tri[5] = 1;

            tri[6] = 0;
            tri[7] = 3;
            tri[8] = 2;

            tri[9] = 0;
            tri[10] = 1;
            tri[11] = 3;


            tetraedroMesh.triangles = tri;

            /* Vector3[] normals = new Vector3[4];

             normals[0] = -Vector3.forward;
             normals[1] = -Vector3.forward;
             normals[2] = -Vector3.forward;
             normals[3] = -Vector3.forward;

             tetraedroMesh.normals = normals;
             MonoBehaviour.print("norm");

             Vector2[] uv = new Vector2[4];

             uv[0] = new Vector2(0, 0);
             uv[1] = new Vector2(1, 0);
             uv[2] = new Vector2(0, 1);
             uv[3] = new Vector2(1, 1);

             tetraedroMesh.uv = uv;*/

            tetraedroMesh.RecalculateBounds();
            tetraedroMesh.RecalculateNormals();
            tetraedroMesh.RecalculateTangents();
        }

        //Initialices values used to weight vertex
        reCalculateVolume();
        initialNormals = new Vector3[4];
        initialDot = new float[4];
        for (int i = 0; i < 4; i++)
        {
            initialNormals[i] = Vector3.Cross(nodes[(i + 1) % 4].Pos - nodes[i].Pos, nodes[(i + 2) % 4].Pos - nodes[i].Pos);
            initialDot[i] = Vector3.Dot(initialNormals[i], nodes[(i + 3) % 4].Pos - nodes[i].Pos);
            nodes[i].Mass += nodes[i].Density * volume * 0.25f;
        }


    }

    //Used for draw the mesh on completeTetraedros
    public void debugDraw()
    {
        for (int i = 0; i < 4; i++)
        {
            vertices[i] = nodes[i].Pos;
        }
        tetraedroMesh.vertices = vertices;
        tetraedroMesh.RecalculateBounds();
        tetraedroMesh.RecalculateNormals();
        tetraedroMesh.RecalculateTangents();

    }

    //If contained in the tetraedro return a weighted vertex, else return null
    public PersonalVertex PointContained(Vector3 point)
    {

        Vector3[] tetraedroLengths = new Vector3[4];
        for (int i = 0; i < 4; i++)
        {
            tetraedroLengths[i] = point - nodes[i].Pos;
            if (Mathf.Sign(Vector3.Dot(initialNormals[i], tetraedroLengths[i])) != Mathf.Sign(initialDot[i]))
            {
                return null;
            }
        }
        PersonalVertex personalVertex = new PersonalVertex();

        personalVertex.setWeight(0, calculateVolume(-tetraedroLengths[1], -tetraedroLengths[2], -tetraedroLengths[3]) / volume, nodes[0]);
        personalVertex.setWeight(1, calculateVolume(tetraedroLengths[0], springs[1].dir, springs[2].dir) / volume, nodes[1]);
        personalVertex.setWeight(2, calculateVolume(springs[0].dir, tetraedroLengths[0], springs[2].dir) / volume, nodes[2]);
        personalVertex.setWeight(3, calculateVolume(springs[0].dir, springs[1].dir, tetraedroLengths[0]) / volume, nodes[3]);

        return personalVertex;
    }

    //Given 3 vectors calculate the volume of the tetraedro within them
    private float calculateVolume(Vector3 distance1, Vector3 distance2, Vector3 distance3)
    {
        return Mathf.Abs(Vector3.Dot(distance1, Vector3.Cross(distance2, distance3))) / 6.0f;
    }

    //Recalculate the tetraedro volume
    private void reCalculateVolume()
    {
        volume = calculateVolume(springs[0].dir, springs[1].dir, springs[2].dir);
    }

    //Apply the corresponding deformity forces to each tetraedro
    public void reCalculateForces()
    {
        reCalculateVolume();
        foreach (Spring spring in springs)
        {
            spring.addForces(volume);
        }

    }
}
