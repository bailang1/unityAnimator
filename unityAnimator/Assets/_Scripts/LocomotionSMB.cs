using UnityEngine;

public class LocomotionSMB : StateMachineBehaviour
{
    public bool enableFaces;

    public float moveDamping = 0.15f;
    public float idleDamping = 0.3f;
    public float sneakDamping = 0.25f;

    public float timeForChangeIdle = 5.0f;
    public float originalTimeForLastIdle = 30.0f;

    private float timePass;
    private bool runEnable;
    private float timeForLastIdle;
    private float timeForNextIdle;
    private bool idleUpdate;
    private float idleId;
    private float faceId;
    private float sneakId;
    private bool sneakUpdate;
    private bool idleFaceUpdate;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timePass = idleId = faceId = 0.0f;
        idleUpdate = sneakUpdate = idleFaceUpdate = false;
        timeForLastIdle = originalTimeForLastIdle;
        timeForNextIdle = timeForChangeIdle;
        animator.SetFloat("Idle", 0.0f);
        if (enableFaces)
        {
            animator.SetFloat("IdleFace", 0.0f);
        }
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            animator.SetFloat("Sneak", 1.0f);
            sneakId = 1.0f;
            runEnable = false;
        }
        else
        {
            animator.SetFloat("Sneak", 0.0f);
            sneakId = 0.0f;
            runEnable = true;
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //Get values of movement
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector2 input = new Vector2(horizontal, vertical).normalized;

        if (animator.GetBool("IKActivated"))
        {
            //Using Ik animation, disabling other options
            SetIkActivated(animator);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                //enter in sneak mode
                setSneaking(animator, true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftAlt))
            {
                //exit of sneak mode
                setSneaking(animator, false);
            }
            else if (runEnable && Input.GetKey(KeyCode.LeftShift))
            {
                //run mode
                if (Input.GetKeyDown(KeyCode.Space) && input.Equals(Vector2.up))
                {
                    //jump must be only enable on running
                    animator.SetTrigger("Jump");
                }
                //movement increased, run backwards must be forbidden
                input.y = Mathf.Max(2.0f * input.y, 0.0f);
                input.x = 2.0f * input.x;

            }
            else updateAnimatorSneak(animator);//checks and performs id necesary a update of the sneak parameter gradually

            if (Input.GetKeyDown(KeyCode.C))
            {
                //sent to crouch state
                animator.SetBool("Crouch", true);
            }
            if (Input.GetKeyDown(KeyCode.X))
            {
                //Extra key for change the idle
                timeForLastIdle = originalTimeForLastIdle;
                timeForNextIdle = timeForChangeIdle;
                timePass = 0.0f;
                changeIdle(animator);
            }

            if (input == Vector2.zero)
            {
                //update de idle and face id if necessary
                updateAnimatorIdle(animator);

                //MonoBehaviour.print ("time Pass: "+timePass);
                if (idleId != 3.0f && timePass > timeForLastIdle)
                {
                    //Enables the waiting long idle
                    idleId = 3.0f;
                    idleUpdate = true;
                    animator.SetFloat("Idle", 3.0f, idleDamping, Time.deltaTime);
                    setFaceId(animator, 1.0f);
                }
                else
                {
                    //increases time pass
                    timePass += Time.deltaTime;
                    if (timePass > timeForNextIdle)
                    {
                        //change idle if necessary
                        timeForNextIdle += timeForChangeIdle;
                        changeIdle(animator);
                    }
                }
            }
            else
            {
                //Disable idle on movement
                timePass = 0.0f;
                if (idleId != 0.0)
                {
                    idleId = 0.0f;
                    idleUpdate = true;
                    animator.SetFloat("Idle", 0.0f, idleDamping, Time.deltaTime);
                    setFaceId(animator, 0.0f);
                }
            }
        }
        animator.SetFloat("Horizontal", input.x, moveDamping, Time.deltaTime);
        animator.SetFloat("Vertical", input.y, moveDamping, Time.deltaTime);
    }

    private void setFaceId(Animator animator, float value)
    {
        if (enableFaces)
        {
            faceId = value;
            idleFaceUpdate = true;
            animator.SetFloat("IdleFace", value, idleDamping, Time.deltaTime);
        }
    }


    private void SetIkActivated(Animator animator)
    {
        idleId = sneakId = 0.0f;
        idleUpdate = sneakUpdate = false;
        runEnable = true;
        animator.SetFloat("Idle", 0.0f, idleDamping, Time.deltaTime);
        animator.SetFloat("Sneak", 0.0f, sneakDamping, Time.deltaTime);
    }

    private void updateAnimatorSneak(Animator animator)
    {
        updateAnimatorParameter(animator, "Sneak", sneakId, sneakDamping, ref sneakUpdate);
    }

    private void updateAnimatorIdle(Animator animator)
    {
        updateAnimatorParameter(animator, "Idle", idleId, idleDamping, ref idleUpdate);
        if (enableFaces)
        {
            updateAnimatorParameter(animator, "IdleFace", faceId, idleDamping, ref idleFaceUpdate);
        }
    }

    private void updateAnimatorParameter(Animator animator, string nombre, float value, float damping, ref bool updating)
    {
        if (updating)
        {
            float animatorValue = animator.GetFloat(nombre);
            //MonoBehaviour.print("update sneak:"+animatorSneak+"-"+sneakId);
            if (Mathf.Abs(animatorValue - value) > 0.02f)
            {
                animator.SetFloat(nombre, value, damping, Time.deltaTime);
            }
            else
            {
                updating = false;
            }
        }
    }

    private void setSneaking(Animator animator, bool mode)
    {
        if (mode)
        {
            sneakId = 1.0f;
            animator.SetFloat("Sneak", 1.0f, sneakDamping, Time.deltaTime);
            sneakUpdate = true;
            runEnable = false;
        }
        else
        {
            sneakId = 0.0f;
            animator.SetFloat("Sneak", 0.0f, sneakDamping, Time.deltaTime);
            runEnable = sneakUpdate = true;
        }
    }

    public void changeIdle(Animator animator)
    {
        int newIdleId = Random.Range(0, 3);
        //MonoBehaviour.print("IdleId: " + newIdleId);
        if (idleId != (float)newIdleId)
        {
            idleId = (float)newIdleId;
            idleUpdate = true;
            animator.SetFloat("Idle", idleId, idleDamping, Time.deltaTime);
        }
        if (enableFaces)
        {
            //Face
            newIdleId = Random.Range(0, 3);
            //MonoBehaviour.print("Idle Face Id: " + newIdleId);
            if (faceId != (float)newIdleId)
            {
                faceId = (float)newIdleId;
                idleFaceUpdate = true;
                animator.SetFloat("IdleFace", faceId, idleDamping, Time.deltaTime);
            }
        }
    }

}
