using UnityEngine;

public class IKmovement : MonoBehaviour
{

    public Transform target;
    public Transform hand;

    private Animator anim;
    private float weight;

    private bool pick = false;
    public float moveDamping = 0.15f;
    private bool movingToTarget = false;
    private bool movedToTarget = false;
    private float y;
    private Vector3 originalTargetPos;
    private Quaternion originalTargetRot;
    private Vector3 originalTargetLocalPos;
    private Quaternion originalTargetLocalRot;
    private Transform originalTargetParent;
    private Rigidbody targetRigidbody;

    // Use this for initialization
    void Start()
    {
        anim = this.GetComponent<Animator>();
        originalTargetPos = target.position;
        originalTargetRot = target.rotation;
        originalTargetLocalPos = target.localPosition;
        originalTargetLocalRot = target.localRotation;
        originalTargetParent = target.parent;
        targetRigidbody = target.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (movingToTarget)
        {
            moveToTarget();
        }
        else if (movedToTarget)
        {
            movedToTarget = false;
            anim.SetTrigger("pickup");
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            startMoving();
        }
    }

    public void startMoving()
    {
        movingToTarget = true;
        y = 0.0f;
        anim.SetBool("IKActivated", true);
    }

    private void moveToTarget()
    {
        //print("move to Target: ");
        Vector3 distance = (target.position - this.transform.position);
        distance.y = 0;
        distance = distance - 0.5f * distance.normalized;
        //print("aaaa"+distance + "magnitud" + distance.magnitude);
        if (distance.magnitude > 0.1f)
        {
            float angle = Vector3.Angle(distance, this.transform.forward);

            float x;
            if (angle > 180.0f)
            {
                x = (angle-360) / 36;//(360 - angle) / 180 * 5;
            }
            else {
                x = (angle) / 36;
            }


            if (angle < 2.0f || angle > 358.0f)
            {
                this.transform.LookAt(target.position);
                y = Mathf.Clamp( distance.magnitude, -0.3f, 1.5f);
            }

            anim.SetFloat("Horizontal", x, moveDamping, Time.deltaTime);
            anim.SetFloat("Vertical", y, moveDamping, Time.deltaTime);
        }
        else
        {
            y = 0.0f;
            anim.SetFloat("Vertical", y);
            movingToTarget = false;
            movedToTarget = true;
        }
    }
    private void OnAnimatorIK(int layerIndex)
    {
        //print("OnAnimatorIK");
        weight = anim.GetFloat("IKpickup");
        anim.SetIKPosition(AvatarIKGoal.RightHand, target.position);
        anim.SetIKPositionWeight(AvatarIKGoal.RightHand, weight);
        anim.SetLookAtPosition(target.position);
        anim.SetLookAtWeight(weight);
    }

    private void LateUpdate()
    {
        //print("LateUpdate");
        if (weight > 0.95)
        {
            pick = true;
            target.parent = hand;
            Transform[] list = hand.GetComponentsInChildren<Transform>();
            foreach (Transform transform in list)
            {
                transform.localRotation = Quaternion.Euler(0, 0, -30);
            }
            target.localPosition = new Vector3(0.047f, -0.06f, 0.011f);
            target.localRotation = Quaternion.Euler(-53, 129, -123);
        }
        if (pick)
        {
            if (anim.GetBool("IKDestroy"))
            {
                target.parent = null;
                targetRigidbody.isKinematic = false;
                Invoke("destroyTarget", 4);
                targetRigidbody.velocity = (transform.up+transform.forward).normalized*3.0f;
                anim.SetBool("IKDestroy", false);
                anim.SetBool("IKActivated", false);
                pick = false;
            }
            else
            {
                Transform[] list = hand.GetComponentsInChildren<Transform>();
                foreach (Transform transform in list)
                {
                    transform.localRotation = Quaternion.Euler(0, 0, -30);
                }
                target.localPosition = new Vector3(0.047f, -0.03f, 0.011f);
                target.localRotation = Quaternion.Euler(-53, 129, -123);
            }
        }

    }

    private void destroyTarget() {
        target.parent = originalTargetParent;
        targetRigidbody.isKinematic = true;
        targetRigidbody.velocity = Vector3.zero;
        target.localPosition = originalTargetLocalPos;
        target.localRotation = originalTargetLocalRot;
        target.position = originalTargetPos;
        target.rotation = originalTargetRot;
    }
}
