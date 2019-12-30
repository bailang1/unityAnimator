using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ControlCharacter : MonoBehaviour
{

    public enum State { RUN, SNEAK, CROUCH, JUMP, NORMAL, CHANGE_IDLE, ACTIVATE_IK };

    [SerializeField] private Joystick joystick;
    [SerializeField] private ToggleGroup toggleGroup;
    [SerializeField] private Toggle runToggle;
    [SerializeField] private Toggle crouchToggle;
    [SerializeField] private Toggle sneakToggle;
    [SerializeField] private Button jumpButton;
    [SerializeField] private Button changeIdleButton;
    [SerializeField] private Button changeCameraButton;
    [SerializeField] private Button activateIK;

    private CameraChange cameraChange;
    private Vector2 moveDirection = Vector2.zero;
    private State actualState, oldState;

    // Start is called before the first frame update
    void Start()
    {
        this.cameraChange = this.transform.GetComponent<CameraChange>();
        this.actualState = State.NORMAL;
        this.crouchToggle.isOn = this.sneakToggle.isOn = this.runToggle.isOn = this.jumpButton.enabled = false;

        this.runToggle.onValueChanged.AddListener(delegate { this.changeToggled(State.RUN, this.runToggle); });
        this.crouchToggle.onValueChanged.AddListener(delegate { this.changeToggled(State.CROUCH, this.crouchToggle); });
        this.sneakToggle.onValueChanged.AddListener(delegate { this.changeToggled(State.SNEAK, this.sneakToggle); });

        if (this.jumpButton != null)
        {
            this.jumpButton.onClick.AddListener(delegate { this.setState(State.JUMP); });
        }
        if (this.changeIdleButton != null)
        {

            this.changeIdleButton.onClick.AddListener(delegate { this.setState(State.CHANGE_IDLE); });
        }
        if (this.activateIK != null) {
            this.activateIK.onClick.AddListener(delegate { this.setState(State.ACTIVATE_IK); });
        }
        if (this.changeCameraButton != null)
        {
            this.changeCameraButton.onClick.AddListener(delegate { this.cameraChange.nextCamera(); });
        }
    }

    private void changeToggled(State newState, Toggle toggle)
    {
        if (toggle.isOn)
        {
            this.setState(newState);
        }
        else if (!this.toggleGroup.AnyTogglesOn())
        {
            this.setState(State.NORMAL);
        }
    }

    private void setState(State state_)
    {
        if (this.actualState != state_)
        {
            switch (state_)
            {
                case State.RUN:
                    this.actualState = State.RUN;
                    this.runToggle.isOn = true;
                    this.crouchToggle.isOn = this.sneakToggle.isOn = this.jumpButton.enabled = false;
                    break;
                case State.CROUCH:
                    this.actualState = State.CROUCH;
                    this.crouchToggle.isOn = true;
                    this.sneakToggle.isOn = this.runToggle.isOn = this.jumpButton.enabled = false;
                    break;
                case State.SNEAK:
                    this.actualState = State.SNEAK;
                    this.sneakToggle.isOn = true;
                    this.crouchToggle.isOn = this.runToggle.isOn = this.jumpButton.enabled = false;
                    break;
                case State.NORMAL:
                    this.actualState = State.NORMAL;
                    this.crouchToggle.isOn = this.sneakToggle.isOn = this.runToggle.isOn = this.jumpButton.enabled = false;
                    break;
                default:
                    this.oldState = this.actualState;
                    this.actualState = state_;
                    break;
            }
        }
    }

    public State restoreState()
    {
        State origState = this.actualState;
        this.actualState = this.oldState;
        return origState;
    }

    // Update is called once per frame
    public Vector2 getDirection()
    {
        this.moveDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (this.moveDirection.Equals(Vector2.zero))
        {
            this.moveDirection = new Vector2(this.joystick.Horizontal, this.joystick.Vertical);
        }
        this.moveDirection = this.moveDirection.normalized;
        if (this.actualState == State.RUN)
        {
            this.moveDirection.Set(this.moveDirection.x * 2.0f, Mathf.Max(2.0f * this.moveDirection.y, 0.0f));
        }
        return this.moveDirection;
    }


    public State getActualState()
    {
        if (this.actualState == State.JUMP || this.actualState == State.CHANGE_IDLE || this.actualState == State.ACTIVATE_IK)
        {
            return restoreState();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            if (this.actualState == State.RUN)
            {
                setState(State.NORMAL);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            setState(State.RUN);
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            if (this.actualState == State.SNEAK)
            {
                setState(State.NORMAL);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            setState(State.SNEAK);
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            if (this.actualState == State.CROUCH)
            {
                setState(State.NORMAL);
            }
            else
            {
                setState(State.CROUCH);
            }
        }
        else if (Input.GetKeyDown(KeyCode.X))
        {
            setState(State.CHANGE_IDLE);
        }
        else if (Input.GetKeyDown(KeyCode.T))
        {
            setState(State.ACTIVATE_IK);
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            this.cameraChange.nextCamera();
        }
        else if (this.actualState == State.RUN || this.actualState == State.NORMAL)
        {
            if (this.moveDirection.y > 1.85f)
            {
                this.jumpButton.enabled = true;
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    setState(State.JUMP);
                }
            }
            else
            {
                this.jumpButton.enabled = false;
            }
        }
        return this.actualState;
    }
}
