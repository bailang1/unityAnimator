using System.Collections.Generic;
using UnityEngine;

public class Fabric : SimulatedObject
{
    #region InEditorVariables
    public float defaultStiffnes;
    public float defaultAntiStiffnes;
    public float defaultMass;
    #endregion

    //This extra class makes easy had more fabrics in the same scene sharing components

    // Use this for initialization
    public override void loadData(PhysicsManager manager)
    {
        Manager = manager;
        SortedDictionary<Edge, List<int>> Edges = new SortedDictionary<Edge, List<int>>();
        mesh = transform.GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        nodes = new Node[vertices.Length];
        for (int i = 0; i < vertices.Length; i++)
        {//iterates thought all mesh vertices
            float nodeMass = defaultMass;
            float nodeDamping = defaultDampingOfNodesRelativeToMass;
            bool nodeIsFixed = defaultFixed;
            Vector3 globalVertexPos = transform.TransformPoint(vertices[i]);
            foreach (propertiesDefineZone propertiesZone in Manager.propertiesZones)
            {//checks if the node its in a propertiesDefineZone				
                if (propertiesZone.bound.Contains(globalVertexPos))
                {
                    nodeIsFixed = propertiesZone.isFixed;
                    nodeMass = propertiesZone.Mass;
                    nodeDamping = propertiesZone.dampingOfNodesRelativeToMass;
                    break;
                }
            }
            //Creates and stores the new node;
            nodes[i] = new Node(globalVertexPos, Manager, nodeMass, nodeDamping, nodeIsFixed, debugDraw);
        }
        for (int i = 0; i < mesh.triangles.Length; i += 3)//Iterates throught all the triangles
        {
            int[] triangleVertex = new int[3];
            triangleVertex[0] = mesh.triangles[i];
            triangleVertex[1] = mesh.triangles[i + 1];
            triangleVertex[2] = mesh.triangles[i + 2];

            for (int a = 0; a < 3; a++)
            {//creates 3 new edges for each triangle
                Edge triangleEdge = new Edge(triangleVertex[a], triangleVertex[(a + 1) % 3]);
                if (Edges.ContainsKey(triangleEdge))
                {//checks if the edge already exist
                    List<int> storedVertices;
                    Edges.TryGetValue(triangleEdge, out storedVertices);//obtain all triangles that use that mesh
                    if (!storedVertices.Contains(triangleVertex[(a + 2) % 3]))
                    {//checks for repetive triangles						
                        foreach (int storedVertex in storedVertices)
                        {//creates a new spring between this triangle and all the other triangles
                            float springStiffnes = defaultAntiStiffnes;
                            float dampingOfRotation = defaultDampingOfRotationRelativeToStiffnes;
                            float dampingOfProyective = defaultDampingOfProyectiveRelativeToStiffnes;
                            Vector3 containedVertex = transform.TransformPoint(0.5f * (vertices[storedVertex] - vertices[triangleVertex[(a + 2) % 3]]));
                            foreach (propertiesDefineZone propertiesZone in Manager.propertiesZones)
                            {// checks if the new spring its in a propertie define zone
                                if (propertiesZone.bound.Contains(containedVertex))
                                {
                                    springStiffnes = propertiesZone.AntiStiffnes;
                                    dampingOfRotation = propertiesZone.dampingOfRotationRelativeToStiffnes;
                                    dampingOfProyective = propertiesZone.dampingOfProyectiveRelativeToStiffnes;
                                    break;
                                }
                            }
                            //Creates and stores the new spring between the opposite triangle vertex, this way avoid creating reptitive springs
                            springs.Add(new Spring(nodes[storedVertex], nodes[triangleVertex[(a + 2) % 3]], springStiffnes, dampingOfRotation, dampingOfProyective, debugDraw));
                        }
                        storedVertices.Add(triangleVertex[(a + 2) % 3]);//Adds the new vertex to the opposite triangles vertex of the spring
                    }
                    else
                    {
                        print("ERROR: triangle duplicated");//shows an error of repetitive triangles
                    }
                }
                else
                {//if the edge its not cointain creates a new edge and adds it to the dictionary 
                    float springStiffnes = defaultStiffnes;
                    float dampingOfRotation = defaultDampingOfRotationRelativeToStiffnes;
                    float dampingOfProyective = defaultDampingOfProyectiveRelativeToStiffnes;
                    Vector3 containedVertex = transform.TransformPoint(0.5f * (vertices[triangleEdge.id0] - vertices[triangleEdge.id1]));
                    foreach (propertiesDefineZone propertiesZone in Manager.propertiesZones)
                    {// checks if the new spring its in a propertie define zone
                        if (propertiesZone.bound.Contains(containedVertex))
                        {
                            springStiffnes = propertiesZone.Stiffnes;
                            dampingOfRotation = propertiesZone.dampingOfRotationRelativeToStiffnes;
                            dampingOfProyective = propertiesZone.dampingOfProyectiveRelativeToStiffnes;
                            break;
                        }
                    }
                    //Creates and adds the new spring
                    springs.Add(new Spring(nodes[triangleEdge.id0], nodes[triangleEdge.id1], springStiffnes, dampingOfRotation, dampingOfProyective, debugDraw));
                    //Adds the new edge to the dictionary
                    List<int> storedVertices = new List<int>();
                    storedVertices.Add(triangleVertex[(a + 2) % 3]);
                    Edges.Add(triangleEdge, storedVertices);
                }
                /**
				// DEBUG: method for show the dictionary
				string edgesString ="Edges count+ "+Edges.Count;
				foreach (KeyValuePair<Edge,List<int>> par in Edges) {
					edgesString += "\n"+ par.Key +": [ ";
					int[] pares = par.Value.ToArray ();
					foreach (int v in pares) {
						edgesString += v + " ";
					}
					edgesString += " ]";
				}
				print (edgesString);//*/
            }
        }

    }

    // Update is called once per frame
    protected override void VertexRecalc()
    {
        foreach (Node node in nodes)
        {
            node.Force = Vector3.zero;
            node.ComputeForces();
        }

        foreach (Spring spring in springs)
        {
            spring.ComputeForces();
        }
    }

    public override void updateMesh()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = transform.InverseTransformPoint(nodes[i].Pos);
        }
        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
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
}
