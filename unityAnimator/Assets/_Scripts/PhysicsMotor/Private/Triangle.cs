using UnityEngine;

public class Triangle
{
    //Class used in tetraedros for draw the debug mesh and calculate the wind force
    #region InEditorVariables
    #endregion

    public Node[] nodes = new Node[3];
    public Spring[] springs = new Spring[3];

    public Triangle() { }

    //Applies the corresponding wind force to all the nodes of the triangle
    public void computeWindForce(Vector3 windSpeed)
    {
        Vector3 TriangleSurface = Vector3.Cross(springs[0].dir, springs[1].dir);//*0.5
        Vector3 Force = Vector3.Cross(Vector3.Cross(windSpeed, TriangleSurface), TriangleSurface.normalized) / 6.0f;
        foreach (Node node in nodes)
        {
            //Fuerza de viento = velocidad del viento * normal del triángulo * área del triangulo * intensidad del viento * normal del triángulo * módulo de la velocidad.
            node.Force += Force * Mathf.Min(node.Vel.magnitude, 2.0f);
        }
    }


}
