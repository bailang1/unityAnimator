using UnityEngine;
using Cinemachine;
using System.Collections.Generic;
using UnityEngine.UI;


public class CameraChange : MonoBehaviour
{
    [SerializeField] private List<GameObject> camerasObj;
    private List<ICinemachineCamera> cameras = new List<ICinemachineCamera>();
    private int actualCamera = 0;

    void Awake()
    {
        foreach (GameObject cameraObject in this.camerasObj)
        {
            ICinemachineCamera camera = cameraObject.GetComponent<ICinemachineCamera>();
            this.cameras.Add(camera);
            camera.Priority = 0;
        }
        this.cameras[actualCamera].Priority = 25;
    }

    public void nextCamera()
    {
        this.cameras[actualCamera].Priority = 0;
        this.actualCamera = (1 + this.actualCamera) % this.cameras.Count;
        this.cameras[this.actualCamera].Priority = 25;
    }

}