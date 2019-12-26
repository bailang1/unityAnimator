using UnityEngine;
using Cinemachine;

public class cameraChange : MonoBehaviour {

    public Transform camA;
    public Transform camB;

    private ICinemachineCamera a;
    private ICinemachineCamera b;
    private bool aOrB = false;

    void Start() {
        a = camA.GetComponent<ICinemachineCamera>();
        b = camB.GetComponent<ICinemachineCamera>();
    }
    
    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (aOrB)
            {
                a.Priority = 25;
                b.Priority = 0;
                aOrB = false;
            }
            else
            {
                a.Priority = 0;
                b.Priority = 25;
                aOrB = true;
            }
        }

	}
}
