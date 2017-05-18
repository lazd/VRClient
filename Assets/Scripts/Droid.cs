// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
using System;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class Droid : MonoBehaviour {
  public float speed = 3.0f;
  public float rotateSpeed = 3.0f;
  public float hitDistance = 1f;
  public bool isAttacking = false;

  private Animator anim;
  private Rigidbody rb;

  void Start() {
  	anim = GetComponent<Animator>();
  	rb = GetComponent<Rigidbody>();

  	// disable physics rotation
  	rb.freezeRotation = true;
  }

  void FixedUpdate () {
    var controller = GetComponent<CharacterController>();

    // Todo: mode style controls
  	var verticalInput = Input.GetAxis("Throttle");
  	var horizontalInput = Input.GetAxis("Yaw");

    // Rotate around y - axis
    transform.Rotate(0, horizontalInput * rotateSpeed, 0);

    // Move forward / backward
    var forward = transform.TransformDirection(Vector3.forward);
    var curSpeed = speed * verticalInput;
    controller.SimpleMove(forward * curSpeed);

    if (Input.GetButtonDown("Fire1")) {
     	anim.SetTrigger("attacking");
    }

    if (Input.GetButtonDown("Fire2")) {
      anim.SetTrigger("attacking2");
    }

    if (curSpeed == 0) {
      anim.SetTrigger("idling");
    }
    else {
      anim.SetTrigger("walking");
    }

  	// Set the character speed as a number, 0-1
  	anim.SetFloat("speed", verticalInput);
  }
}
