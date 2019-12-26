using UnityEngine;

public class propertiesDefineZone : MonoBehaviour
{
    #region InEditorVariables
    public float Mass;
    public float Stiffnes;
    public float AntiStiffnes;
    public bool isFixed;
    public float dampingOfNodesRelativeToMass;
    public float dampingOfRotationRelativeToStiffnes;
    public float dampingOfProyectiveRelativeToStiffnes;
    #endregion

    public Bounds bound;

    //Variant of a fixer object with extra properties

    // Use this for initialization
    public void initialize()
    {
        bound = transform.GetComponent<Collider>().bounds;
    }

}