using UnityEngine;
using System.Collections;

public class PersonalVertex
{

    //Class with a weigtened nodes to update a mesh vertex

    #region InEditorVariables
    #endregion

    private float[] weights;
    private Node[] nodes;

    public PersonalVertex() {
       weights = new float[4];
       nodes = new Node[4];
    }

    //Returns the new vertex position
    public Vector3 recalPoss() {
        Vector3 r = Vector3.zero;
        for (int i = 0; i< 4; i++) {
            r += weights[i] * nodes[i].Pos;
        }
        return r;
    }

    //set a new weigtned node
    public void setWeight(int i, float w, Node node) { 
        weights[i]=w;
        nodes[i] = node;
     }

}
