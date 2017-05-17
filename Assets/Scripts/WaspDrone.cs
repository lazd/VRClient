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

    public Vector3 maxV = new Vector3(4,4,4);       //max desired rate of change

    public Vector3 Kp = new Vector3(4, 4, 4);
    public Vector3 Ki = new Vector3(.007f,.007f,.007f);
    public Vector3 Kd = new Vector3(0,0,0); 

    protected PidController3Axis pControl = new PidController3Axis();

    protected Vector3 inputs;

    private Rigidbody rb;

    protected virtual void Start() {
        rb = GetComponent<Rigidbody>();
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

    void FixedUpdate() {
        // Throttle
        rb.AddForce(transform.up * Input.GetAxis("Throttle") * throttleThrust, ForceMode.Impulse);

        RCS();
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