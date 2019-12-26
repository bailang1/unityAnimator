using UnityEngine;

public class CrouchSMB : StateMachineBehaviour {
	public float damping = 0.15f;
	public float idleDamping = 0.3f;
	public float timeForLastIdle = 30.0f; 
	private float timePass;
	private float idleId; 
	private bool idleUpdate;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{
		timePass = 0.0f;
		idleId = 0.0f;
		animator.SetFloat ("Idle", 0.0f);
		idleUpdate = false;
		}


	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
	{

		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");

		if (Input.GetKeyUp (KeyCode.C) || animator.GetBool("IKActivated")) {
            //If c pressed or a ik movement is activated exit of crouch mode
			animator.SetBool ("Crouch", false);
		}

		Vector2 input = new Vector2(horizontal, vertical).normalized;

		if (input == Vector2.zero) {
			if(idleUpdate){
				float animatorIdle = animator.GetFloat ("Idle");
				MonoBehaviour.print("update idle:"+animatorIdle+"-"+idleId);
				if (Mathf.Abs(animatorIdle - idleId) > 0.02f) {
					animator.SetFloat ("Idle", idleId, idleDamping, Time.deltaTime);
				} else {
					idleUpdate = false;
				}
			}
			//MonoBehaviour.print ("time Pass: "+timePass);
			if(idleId != 1.0f && timePass > timeForLastIdle){
				idleId = 1.0f;
				idleUpdate = true;
				animator.SetFloat ("Idle", 1.0f,idleDamping,Time.deltaTime);
			}else{
				timePass += Time.deltaTime;			}
		}else{
			timePass = 0.0f;
			if (idleId != 0.0) {
				idleId = 0.0f;
				idleUpdate = true;
				animator.SetFloat ("Idle", 0.0f,idleDamping,Time.deltaTime);
			}
		}

		animator.SetFloat("Horizontal", input.x, damping, Time.deltaTime);
		animator.SetFloat("Vertical", input.y, damping, Time.deltaTime);
	}
}
