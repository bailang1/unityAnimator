using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Basic physics manager capable of simulating a given ISimulable
/// implementation using diverse integration methods: explicit,
/// implicit, Verlet and semi-implicit.
/// </summary>
public class PhysicsManager : MonoBehaviour
{
    #region InEditorVariables
    public bool Paused;
    public float TimeStep;
    public Vector3 Gravity;
    public Integration IntegrationMethod;
    public List<SimulatedObject> simulatedObjects;
    public List<CollisionZone> collisionZones;
    public List<propertiesDefineZone> propertiesZones;
    public int numIterationsPerFrame;
    public Vector3[] WindForces;
    public float WindRandomnes = 0.0f;
    #endregion

    public enum Integration
    {
        Explicit = 0,
        Symplectic = 1,
    };

    /// <summary>
    /// Default constructor. Zero all. 
    /// </summary>
    public PhysicsManager()
    {
        Paused = true;
        TimeStep = 0.01f;
        Gravity = new Vector3(0.0f, -9.81f, 0.0f);
        IntegrationMethod = Integration.Explicit;
        numIterationsPerFrame = 1;
    }

    #region MonoBehaviour

    public void Start()
    {
        foreach (propertiesDefineZone propertiesZone in propertiesZones)
        {
            propertiesZone.initialize();
        }
        foreach (CollisionZone colZone in collisionZones)
        {
            colZone.initialateBound();
        }
        foreach (SimulatedObject simulatedObject in simulatedObjects)
        {
            simulatedObject.loadData(this);
        }
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
            Paused = !Paused;
        foreach (SimulatedObject simulatedObject in simulatedObjects)
        {
            simulatedObject.updateMesh();
        }
    }

    public void FixedUpdate()
    {
        if (Paused)
            return; // Not simulating
        // Select integration method
        switch (IntegrationMethod)
        {
            case Integration.Explicit:
                for (int i = 0; i < numIterationsPerFrame; i++)
                {
                    stepExplicit();
                }
                break;
            case Integration.Symplectic:
                for (int i = 0; i < numIterationsPerFrame; i++)
                {
                    stepSymplectic();
                }
                break;
            default:
                throw new System.Exception("[ERROR] Should never happen!");
        }
    }

    #endregion

    /// <summary>
    /// Performs a simulation step using Explicit integration.
    /// </summary>
    private void stepExplicit()
    {
        foreach (SimulatedObject simulatedObject in simulatedObjects)
        {
            simulatedObject.VertexReCalcExplicit();
        }
    }

    /// <summary>
    /// Performs a simulation step using Symplectic integration.
    /// </summary>
    private void stepSymplectic()
    {
        foreach (SimulatedObject simulatedObject in simulatedObjects)
        {
            simulatedObject.VertexReCalcSymplectic();
        }
    }

}