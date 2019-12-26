using System;

public class Edge : IComparable<Edge>
{
    //Class that stores a triangle edge to be added to the map (faster than strings)

    #region InEditorVariables
    #endregion

    public int id0;
    public int id1;
    public Edge(int i1, int i2)
    {
        if (i1 < i2)
        {//always stores the lowest in id0
            id0 = i1;
            id1 = i2;
        }
        else
        {
            id1 = i1;
            id0 = i2;
        }
    }
    //Compare methods for sorting in the dictionary
    public int CompareTo(Edge e2)
    {
        return Compare(this, e2);
    }
    public int Compare(Edge e1, Edge e2)
    {
        if ((e1.id0 == e2.id0 && e1.id1 == e2.id1) || (e1.id0 == e2.id1 && e1.id1 == e2.id0))
        {
            return 0;
        }
        else if ((e1.id0 - e2.id0) != 0)
            return e1.id0 - e2.id0;
        else
            return e1.id1 - e2.id1;
    }

    //DEBUG method:
    public override string ToString()
    {
        return ("Edge: {id0: " + id0 + " id1: " + id1 + "}");
    }
}
