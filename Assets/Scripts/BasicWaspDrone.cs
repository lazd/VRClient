using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicWaspDrone : MonoBehaviour {
  public Rigidbody rb;
  private Animator anim;
  public float thrust = 20f;

  public float pitchRate = 10f;
  public float yawRate = 5f;
  public float rollRate = 10f;

  void Start () {
    anim = GetComponent<Animator>();
    rb = GetComponent<Rigidbody>();
    anim.SetBool("fly", true);
    anim.SetBool("fly", true);
  }
  
  void FixedUpdate () {
    // Throttle
    rb.AddForce(transform.up * Input.GetAxis("Throttle") * thrust);

    // Yaw
    // rb.AddTorque(transform.up * yawTorque * Input.GetAxis("Yaw"));

    // Pitch
    // rb.AddTorque(transform.right * yawTorque * -Input.GetAxis("Pitch"));

    // Roll
    // rb.AddTorque(transform.forward * yawTorque * -Input.GetAxis("Roll"));

    transform.Rotate(-Input.GetAxis("Pitch") * pitchRate, Input.GetAxis("Yaw") * yawRate, -Input.GetAxis("Roll") * rollRate);
  }
}