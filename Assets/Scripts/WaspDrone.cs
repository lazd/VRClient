using System;
using UnityEngine;
using System.Collections;

public class WaspDrone : MonoBehaviour {

    public GUIText ScreenReadout;

    public float throttleThrust = 2f;

    public Vector3 thrust = new Vector3(1,1,1);     //Total thrust per axis
    public Vector3 maxForces;                       //the amount of torque available for each axis, based on thrust

    protected Vector3 targetVelocity;               //user input determines how fast user wants ship to rotate
    protected Vector3 curVelocity;                  //holds the rb.angularVelocity converted from world space to local

    public Vector3 maxV = new Vector3(8,8,8);       //max desired rate of change
    public Vector3 expo = new Vector3(1.8f, 1.8f, 1.8f);

    public Vector3 Kp = new Vector3(4, 4, 4);
    public Vector3 Ki = new Vector3(.007f,.007f,.007f);
    public Vector3 Kd = new Vector3(0,0,0); 

    protected PidController3Axis pControl = new PidController3Axis();

    protected Vector3 inputs;

    private Rigidbody rb;
    private Animator anim;

    protected virtual void Start() {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();

        var collider = GetComponent<CapsuleCollider>();

        // distance from transform.position to ground
        distGround = collider.bounds.extents.y - collider.center.y;

        ApplyValues();  
    }

    protected virtual void ApplyValues(){
        SetForces();

        pControl.Kp = Kp;
        pControl.Ki = Ki;
        pControl.Kd = Kd;
        pControl.outputMax = maxForces;
        pControl.outputMin = maxForces * -1;
        pControl.SetBounds();
    }

    protected virtual void SetForces(){
        //this is where the bounding box is used to create pseudo-realistic torque;  If you want more detail, just ask.
        var shipExtents = ((MeshFilter)GetComponentInChildren(typeof(MeshFilter))).mesh.bounds.extents;
        maxForces.x = new Vector2(shipExtents.y,shipExtents.y).magnitude*thrust.x + 2;
        maxForces.y = new Vector2(shipExtents.x,shipExtents.x).magnitude*thrust.y + 2;
        maxForces.z = new Vector2(shipExtents.z,shipExtents.z).magnitude*thrust.z + 2;
    }

    protected float getI(string axis) {
        return Input.GetAxisRaw(axis);          
    }

    protected virtual void SetVelocities(){
        // collect inputs
        var inputs = new Vector3(-getI("Pitch"),getI("Yaw"),-getI("Roll"));

        // Apply expo
        inputs = new Vector3(
            Mathf.Sign(inputs.x) * Mathf.Pow(Mathf.Abs(inputs.x), expo.x),
            Mathf.Sign(inputs.y) * Mathf.Pow(Mathf.Abs(inputs.y), expo.y),
            Mathf.Sign(inputs.z) * Mathf.Pow(Mathf.Abs(inputs.z), expo.z)
        );

        targetVelocity = Vector3.Scale (inputs, maxV);

        //take the rb.angularVelocity and convert it to local space; we need this for comparison to target rotation velocities
        curVelocity = transform.InverseTransformDirection(rb.angularVelocity);
    }

    protected virtual void SetOutputs(){
        rb.AddRelativeTorque(pControl.output * Time.fixedDeltaTime, ForceMode.Impulse);  
    }

    protected virtual void RCS() {
        // Uncomment to catch inspector changes 
        ApplyValues();

        SetVelocities();                

        // run the controller
        pControl.Cycle(curVelocity, targetVelocity, Time.fixedDeltaTime);
        SetOutputs();

        // Debug.Log("Current V : " + curVelocity + "\n" + "Target V :" + targetVelocity + "Current T : " + pControl.output);
    }

    // Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html

    // Configuration
    public float moveSpeed = 8f; // move speed
    public float turnSpeed = 90f; // turning speed (degrees/second)
    public float gravity = 20f; // gravity acceleration
    public float deltaGround = 0.01f; // character is grounded up to this distance
    public float forwardMotion = 0f;
    public float airControlFactor = 0.5f;

    // API
    public float speed = 0f; // The speed of the character
    public bool isGrounded;

    private Vector3 surfaceNormal; // current surface normal
    private float distGround; // distance from character position to ground

    public float accelTime = 1f;
    private float currentSpeed = 0f;
    private float yVelocity = 0.0f;
    private float startSpeed = 0f;
    private RaycastHit hit;

    private Vector3 curNormal = Vector3.zero;
    private Vector3 usedNormal = Vector3.zero;
    private Quaternion tiltToNormal;

    public float attractionDistance = 2f;
    public float stickyRotationLerpFactor = 6f;
    private float verticalInput;
    private float horizontalInput;

    void WallWalk() {
        verticalInput = Input.GetAxis("Throttle");
        horizontalInput = Input.GetAxis("Yaw");

        // Add accelation component
        if (verticalInput == 0) {
            // Reset speed if we stop moving
            currentSpeed = startSpeed;
        }
        else {
            // Slowly increase speed
            currentSpeed = Mathf.SmoothDamp(currentSpeed, moveSpeed, ref yVelocity, accelTime);
        }

        // Calculate forward motion based on stick input
        forwardMotion = verticalInput * currentSpeed;

        if (Physics.Raycast (transform.position, transform.forward, out hit, attractionDistance)) {
            Debug.DrawRay (transform.position, transform.forward, Color.blue, attractionDistance);
            
            usedNormal = hit.normal;
            curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
            tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
            transform.rotation = tiltToNormal;
        }
        else { 
            if (Physics.Raycast (transform.position, -transform.up, out hit, attractionDistance)) {
                Debug.DrawRay (transform.position, -transform.up, Color.green, attractionDistance);
                usedNormal = hit.normal;
                curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
                tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
                transform.rotation = tiltToNormal;
            }
            else {
                // Todo: why 0.3?
                if (Physics.Raycast (transform.position + (-transform.up), -transform.forward + new Vector3 (0f, .3f, 0f), out hit, attractionDistance)) {
                    Debug.DrawRay (transform.position + (-transform.up), -transform.forward + new Vector3 (0f, .3f, 0f), Color.green, attractionDistance);
                    usedNormal = hit.normal;
                    curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
                    tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
                    transform.rotation = tiltToNormal;
                }
                else {
                    curNormal = Vector3.Lerp (curNormal, Vector3.up, stickyRotationLerpFactor * Time.deltaTime);
                    tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
                    transform.rotation = tiltToNormal;
                }
            }
        }

        // Turn left/right with horizontal axis:
        transform.Rotate(0f, horizontalInput * turnSpeed * Time.deltaTime, 0f);

        // Move forward/back with vertical axis
        if (isGrounded) {
            transform.Translate(0f, 0f, forwardMotion * Time.deltaTime);
        }
        else {
            // Move slower in the air
            transform.Translate(0f, 0f, forwardMotion * airControlFactor * Time.deltaTime);
        }

        // Apply constant force according to character normal
        // This keeps the walker stuck to the wall and acts as gravity
        // If this is applied unconditionally, gravity must be off on the RigidBody
        rb.AddForce(-gravity * rb.mass * transform.up);

        // Set the character speed as a number, -1 to 1
        speed = forwardMotion/8;
        anim.SetFloat("speed", speed);
    }

    void FixedUpdate() {
        // Cast ray downwards to detect if we're on the ground
        var ray = new Ray(transform.position, -transform.up); 
        if (Physics.Raycast(ray, out hit)) {
            isGrounded = hit.distance <= distGround + deltaGround;
        }
        else {
            isGrounded = false;
        }

        // Enable flight control system if we"re in the air
        if (!isGrounded) {
            // Enable physics rotation
            rb.freezeRotation = false;

            // Throttle
            rb.AddForce(transform.up * Input.GetAxis("Throttle") * throttleThrust, ForceMode.Impulse);

            RCS();            
        }
        else {
            // Otherwise, be a wallwalker

            // Disable physics rotation
            rb.freezeRotation = true;
            WallWalk();
        }
    }
}

public class PidController3Axis {

    public Vector3 Kp;
    public Vector3 Ki;
    public Vector3 Kd;

    public Vector3 outputMax;
    public Vector3 outputMin;

    public Vector3 preError;

    public Vector3 integral;
    public Vector3 integralMax;
    public Vector3 integralMin;

    public Vector3 output;

    public void SetBounds(){
        integralMax = Divide(outputMax, Ki);
        integralMin = Divide(outputMin, Ki);        
    }

    public Vector3 Divide(Vector3 a, Vector3 b){
        Func<float, float> inv = (n) => 1/(n != 0? n : 1);
        var iVec = new Vector3(inv(b.x), inv(b.x), inv(b.z));
        return Vector3.Scale (a, iVec);
    }

    public Vector3 MinMax(Vector3 min, Vector3 max, Vector3 val){
        return Vector3.Min(Vector3.Max(min, val), max);
    }

    public Vector3 Cycle(Vector3 PV, Vector3 setpoint, float Dt){
        var error = setpoint - PV;
        integral = MinMax(integralMin, integralMax, integral + (error * Dt));

        var derivative = (error - preError) / Dt;
        output = Vector3.Scale(Kp,error) + Vector3.Scale(Ki,integral) + Vector3.Scale(Kd,derivative);
        output = MinMax(outputMin, outputMax, output);

        // Debug.Log("setpoint : " + setpoint);
        // Debug.Log("error : " + error);
        // Debug.Log("derivative : " + derivative);
        // Debug.Log("outputMin : " + outputMin);
        // Debug.Log("outputMax : " + outputMax);
        // Debug.Log("output : " + output);

        preError = error;
        return output;
    }
}


