using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class TetraedroObject : SimulatedObject
{
    #region InEditorVariables
    public TextAsset nodeText;
    public TextAsset elementText;
    public TextAsset faceText;
    public DebugTetraedrosType debugTetraedros;
    public ObtainProperties readPropertiesFrom;

    public float defaultStiffnesDensity;
    public float defaultMassDensity;
    #endregion

    private Tetraedro[] tetraedros;
    private Triangle[] triangles;
    private Mesh tetraedroMesh;
    private PersonalVertex[] verticesPersonal;

    public enum DebugTetraedrosType
    {
        None = 0,
        Complete = 1,
        TriangleMesh = 2
    };
    public enum ObtainProperties
    {
        None = 0,
        FileAttributes = 1,
        PropertiesZones = 2
    };

    // Use this for initialization
    public override void loadData(PhysicsManager manager)
    {
        Manager = manager;
        if (readPropertiesFrom == ObtainProperties.FileAttributes)
        {
            //If activated recalculates the value of the attribute for each color unit 
            defaultMassDensity = defaultMassDensity / 255.0f;
            defaultStiffnesDensity = defaultStiffnesDensity / 255.0f;
        }
        //Parse and initialices nodes
        string[] lines = Regex.Split(nodeText.text, "\\s*\\n+\\s*");
        string[] elements = Regex.Split(lines[0], "\\s+");
        nodes = new Node[System.Int32.Parse(elements[0])];
        for (int i = 1; i < lines.Length; i++)
        {//Parse nodes bucle
            if (!(lines[i].StartsWith("#")))
            {//Not comment line
                elements = Regex.Split(lines[i], "\\s+");
                //debugingFileLine(i,lines[i],elements);
                if (elements.Length >= 4)
                {//Correct number of attributes
                    //Obtain the node position
                    Vector3 nodePos = transform.TransformPoint(new Vector3(float.Parse(elements[1]), float.Parse(elements[2]), float.Parse(elements[3])));
                    //initialices values at deffault value
                    bool nodeIsFixed = defaultFixed;
                    float nodeDensity = defaultMassDensity;
                    float nodeDamping = defaultDampingOfNodesRelativeToMass;
                    float nodeStiffness = defaultStiffnesDensity;
                    if (readPropertiesFrom == ObtainProperties.PropertiesZones)
                    {//obtain values from properties zones
                        foreach (propertiesDefineZone propertiesZone in Manager.propertiesZones)
                        {//checks if the node its in a propertiesDefineZone				
                            if (propertiesZone.bound.Contains(nodePos))
                            {
                                nodeIsFixed = propertiesZone.isFixed;
                                nodeDensity = propertiesZone.Mass;
                                nodeDamping = propertiesZone.dampingOfNodesRelativeToMass;
                                nodeStiffness = propertiesZone.Stiffnes;
                                break;
                            }
                        }
                    }
                    else if (readPropertiesFrom == ObtainProperties.FileAttributes)
                    {//Obtain values from extra attributes in node file
                        if (elements.Length >= 7)
                        {//Checks the new attribute requirement
                            nodeDensity *= System.Int32.Parse(elements[4]);//component red
                            nodeStiffness *= System.Int32.Parse(elements[5]);//component green
                            if (System.Int32.Parse(elements[6]) > 128)
                            {//component blue
                                nodeIsFixed = !nodeIsFixed;
                            }
                        }
                        else
                        {
                            print("No attributes detected in line: " + i + ": " + lines[i]);
                        }
                    }
                    //Creates the new node
                    nodes[System.Int32.Parse(elements[0])] = new Node(nodePos, manager, nodeDensity, nodeDamping, nodeIsFixed, debugDraw, nodeStiffness);
                }
                else
                {
                    print("Unexpected format in line: " + i + ": " + lines[i]);
                }
            }
        }

        //Creates the map used to avoid duplicate strings
        SortedDictionary<Edge, Spring> Edges = new SortedDictionary<Edge, Spring>();

        //Parse and nitialices triangles faces (if necesary)
        if (debugTetraedros == DebugTetraedrosType.TriangleMesh || manager.WindForces.Length > 0)
        {
            lines = Regex.Split(faceText.text, "\\s*\\n+\\s*");
            elements = Regex.Split(lines[0], "\\s+");
            triangles = new Triangle[System.Int32.Parse(elements[0])];
            int[] trianglesIndexes = new int[3 * triangles.Length];
            for (int i = 1; i < lines.Length; i++)
            {//Parse faces/triangles bucle
                if (!lines[i].StartsWith("#"))
                {//Not comment line
                    elements = Regex.Split(lines[i], "\\s+");
                    //debugingFileLine(i,lines[i],elements);
                    if (elements.Length >= 4)
                    {//correct number of attributes
                        int actualIndex = System.Int32.Parse(elements[0]);
                        int actualIndexx3 = actualIndex * 3;
                        Triangle actualTriangle = new Triangle();
                        for (int a = 0; a < 3; a++)
                        {//each line element
                            trianglesIndexes[actualIndexx3 + a] = System.Int32.Parse(elements[3 - a]);
                            actualTriangle.nodes[a] = nodes[trianglesIndexes[a]];
                        }

                        //assign the springs to the triangle; assignSpring avoids duplicates
                        actualTriangle.springs[0] = assignSpring(trianglesIndexes[actualIndexx3], trianglesIndexes[actualIndexx3 + 1], Edges);
                        actualTriangle.springs[1] = assignSpring(trianglesIndexes[actualIndexx3], trianglesIndexes[actualIndexx3 + 2], Edges);
                        actualTriangle.springs[2] = assignSpring(trianglesIndexes[actualIndexx3 + 1], trianglesIndexes[actualIndexx3 + 2], Edges);
                        //Assign the new triangle
                        triangles[actualIndex] = actualTriangle;
                    }
                    else
                    {
                        print("Unexpected format in line: " + i + ": " + lines[i]);
                    }
                }
            }
            if (debugTetraedros == DebugTetraedrosType.TriangleMesh)
            {//If debug using mesh triangle creates a new mesh triangle
                GameObject tetraedroObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
                tetraedroObject.name = ("DebugTetraedroMesh: " + this.transform.name);
                tetraedroMesh = new Mesh();
                tetraedroObject.GetComponent<MeshFilter>().mesh = tetraedroMesh;

                Vector3[] trianglesVertices = new Vector3[nodes.Length];
                for (int i = 0; i < nodes.Length; i++)
                {
                    trianglesVertices[i] = nodes[i].Pos;
                }
                tetraedroMesh.vertices = trianglesVertices;
                tetraedroMesh.triangles = trianglesIndexes;
                tetraedroMesh.RecalculateBounds();
                tetraedroMesh.RecalculateNormals();
                tetraedroMesh.RecalculateTangents();
            }

        }
        //Parse and initialices springs and tetraedros
        lines = Regex.Split(elementText.text, "\\s*\\n+\\s*");
        elements = Regex.Split(lines[0], "\\s+");
        tetraedros = new Tetraedro[System.Int32.Parse(elements[0])];
        for (int i = 1; i < lines.Length; i++)
        {//Parse bucle for tetraedros
            if (!lines[i].StartsWith("#"))
            {//Not comment line
                elements = Regex.Split(lines[i], "\\s+");
                //debugingFileLine(i,lines[i],elements);
                if (elements.Length >= 4)
                {//Correct number of elements
                    int[] tetraedroVertices = new int[4];
                    for (int a = 0; a < 4; a++)
                    {//each index vertex
                        tetraedroVertices[a] = System.Int32.Parse(elements[a + 1]);
                    }
                    //Creates a new tetraedr
                    Tetraedro actualTetraedro = new Tetraedro();
                    int indice = -1;
                    for (int a = 0; a < 4; a++)
                    {//Assign the nodes and springs to the tetraedro
                        actualTetraedro.nodes[a] = nodes[tetraedroVertices[a]];
                        for (int b = a + 1; b < 4; b++)
                        {
                            actualTetraedro.springs[++indice] = assignSpring(tetraedroVertices[a], tetraedroVertices[b], Edges);
                        }
                    }
                    actualTetraedro.initData(debugTetraedros);
                    tetraedros[System.Int32.Parse(elements[0])] = actualTetraedro;
                }
                else
                {
                    print("Unexpected format in line: " + i + ": " + lines[i]);
                }
            }
        }

        //initialices vertex weights
        mesh = transform.GetComponent<MeshFilter>().mesh;
        verticesPersonal = new PersonalVertex[mesh.vertices.Length];
        vertices = new Vector3[mesh.vertices.Length];
        for (int i = 0; i < verticesPersonal.Length; i++)
        {
            verticesPersonal[i] = assignPersonalVertex(transform.TransformPoint(mesh.vertices[i]));
        }

    }

    protected override void VertexRecalc()
    {
        foreach (Node node in nodes)
        {
            node.Force = Vector3.zero;
            node.ComputeForces();
        }
        foreach (Vector3 windForce in Manager.WindForces)
        {
            Vector3 newWindForce = new Vector3(
                windForce.x * Random.Range((1.0f - Manager.WindRandomnes), (1.0f + Manager.WindRandomnes)),
                windForce.y * Random.Range((1.0f - Manager.WindRandomnes), (1.0f + Manager.WindRandomnes)),
                windForce.z * Random.Range((1.0f - Manager.WindRandomnes), (1.0f + Manager.WindRandomnes)));
            foreach (Triangle triangle in triangles)
            {
                triangle.computeWindForce(newWindForce);
            }
        }

        foreach (Spring spring in springs)
        {
            spring.calculateLength();
            spring.computeForceFactor();
            spring.ComputeDamping();
        }
        foreach (Tetraedro tetraedo in tetraedros)
        {
            tetraedo.reCalculateForces();

        }
    }

    public override void updateMesh()
    {
        for (int i = 0; i < verticesPersonal.Length; i++)
        {
            vertices[i] = transform.InverseTransformPoint(verticesPersonal[i].recalPoss());
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();

        if (debugTetraedros == DebugTetraedrosType.Complete)
        {
            foreach (Tetraedro tetraedro in tetraedros)
            {
                tetraedro.debugDraw();
            }
        }
        else if (debugTetraedros == DebugTetraedrosType.TriangleMesh)
        {
            Vector3[] trianglesVertices = new Vector3[nodes.Length];
            for (int i = 0; i < nodes.Length; i++)
            {
                trianglesVertices[i] = nodes[i].Pos;
            }
            tetraedroMesh.vertices = trianglesVertices;
            tetraedroMesh.RecalculateBounds();
            tetraedroMesh.RecalculateNormals();
            tetraedroMesh.RecalculateTangents();
        }

        if (debugDraw == SimulatedObject.DebugDrawType.Instantiate)
        {
            foreach (Node node in nodes)
            {
                node.debugDrawing();
            }
            foreach (Spring spring in springs)
            {
                spring.debugDrawing();
            }
        }
    }

    //return the correct spring for those vertices, avoiding duplicated springs
    private Spring assignSpring(int verticeA, int verticeB, SortedDictionary<Edge, Spring> Edges)
    {
        Edge edge = new Edge(verticeA, verticeB);
        Spring returnSpring;

        if (Edges.ContainsKey(edge))
        {//checks if the edge already exist                               
            Edges.TryGetValue(edge, out returnSpring);//obtain the actual spring
        }
        else
        {//if the edge its not cointain creates a new edge and adds it to the dictionary 
            returnSpring = new Spring(nodes[edge.id0], nodes[edge.id1], defaultDampingOfRotationRelativeToStiffnes, defaultDampingOfProyectiveRelativeToStiffnes, debugDraw);
            springs.Add(returnSpring);
            Edges.Add(edge, returnSpring);
        }
        return returnSpring;

    }

    //Assign a tetraedro and nodes weights to a vertex
    private PersonalVertex assignPersonalVertex(Vector3 vertexPos)
    {
        foreach (Tetraedro tetraedro in tetraedros)
        {//Checks in which tetraedro is contained the vertex
            PersonalVertex personalVertex = tetraedro.PointContained(vertexPos);
            if (personalVertex != null)
            {
                //print("Vertex: " + vertexPos + " Assign: {" + tetraedro.nodes[0].Pos + ", " + tetraedro.nodes[1].Pos + ", " + tetraedro.nodes[2].Pos + ", " + tetraedro.nodes[3].Pos);
                return personalVertex;
            }
        }
        print("Error vertex: " + vertexPos + " no contenido");
        return new PersonalVertex();
    }

    //Debug method for show a parsed line
    private void debugingFileLine(int i, string line, string[] elements)
    {
        string printS = i + ": " + line + " with: " + elements.Length + "elements:{ ";
        for (int a = 0; a < elements.Length; a++)
        {
            printS += a + ": " + elements[a] + "\n";
        }
        print(printS + "\n}");
    }
}
