// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
using System;
using UnityEngine;
using System.Collections;

public class Wasp : MonoBehaviour {

  public float ascendSpeed = 5f;
  public float descendSpeed = 2f;
  public float groundMoveSpeed = 5f;
  public float airMoveSpeed = 20f;
  public float liftAccelTime = 1f;

  public float throttleThrust = 2f;

  private Animator anim;

  private WallWalker wallWalker;
  private Rigidbody rb;

  private float initialGravity;
  private float currentVerticalLift;
  private float startVerticalLift;

  private float yVelocity = 0.0f;

  private float jumpInput;
  private float throttleInput;

  void Start() {
    rb = GetComponent<Rigidbody>();
    anim = GetComponent<Animator>();
    wallWalker = GetComponent<WallWalker>();

    startVerticalLift = ascendSpeed/2;

    initialGravity = wallWalker.gravity;
  }

  void FixedUpdate() {
    jumpInput = Input.GetAxis("Jump");
    throttleInput = wallWalker.throttleStick != "" ? Input.GetAxis(wallWalker.throttleStick) : 0;

    if (wallWalker.isGrounded) {
      wallWalker.gravity = initialGravity;
      wallWalker.moveSpeed = groundMoveSpeed;

      if (anim.GetCurrentAnimatorStateInfo(0).IsName("fly")) {
        land();
      }
      else {
        if (wallWalker.speed != 0) {
          walk();
        }
        else {
          idle();
        }

        if (jumpInput != 0 || (throttleInput > 0 && wallWalker.controlStyle != "Simple")) {
          takeOff();
        }
      }
    }
    else {
      wallWalker.gravity = 0;
      wallWalker.moveSpeed = airMoveSpeed;

      if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle") || anim.GetCurrentAnimatorStateInfo(0).IsName("walk")) {
        takeOff();
      }
      else {
        fly();
      }
    }

    // Add accelation component
    if (jumpInput > 0) {
      // Slowly increase lift speed
      currentVerticalLift = Mathf.SmoothDamp(currentVerticalLift, ascendSpeed, ref yVelocity, liftAccelTime);
    }
    else {
      // Reset lift speed if we stop moving
      currentVerticalLift = Mathf.SmoothDamp(currentVerticalLift, startVerticalLift, ref yVelocity, liftAccelTime);
    }

    if (wallWalker.controlStyle == "Simple") {
      if (jumpInput != 0) {
        // Go up a bit
        transform.Translate(0, currentVerticalLift * Time.deltaTime, 0);
      }
      else if (!wallWalker.isGrounded) {
        // Go down a bit
        transform.Translate(0, -descendSpeed * Time.deltaTime, 0);
      }
    }
    else {
      if (jumpInput != 0) {
        throttleInput = jumpInput;
      }

      // Bug: Need to clamp force somehow, can go so fast it flies through things
      rb.AddForce(transform.up * throttleInput * throttleThrust, ForceMode.Impulse);
    }

    if (Input.GetButtonDown("Fire1")) {
      attack();
    }

    // Todo: hit
    // Todo: dying
  }

  void idle() {
    Debug.Log("Idle");
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");
    anim.SetTrigger("idling");
  }

  void walk() {
    Debug.Log("Walk");
    anim.ResetTrigger("flying");
    anim.ResetTrigger("idling");
    anim.SetTrigger("walking");
  }

  void fly() {
    Debug.Log("Fly");
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.SetTrigger("flying");
  }

  void hurt() {
    Debug.Log("Hurt");
    // Todo: Correpsonding animation transitions
    // Todo: Trigger from impacts
    anim.SetTrigger("hurting");
  }

  void attack() {
    Debug.Log("Attack");
    // Todo: Trigger hits
    anim.SetTrigger("attacking");
  }

  void die() {
    Debug.Log("Die");
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");

    // Todo: Correpsonding animation transitions
    // Todo: Trigger from low health
    anim.SetTrigger("dying");
  }

  void takeOff() {
    Debug.Log("TakeOff");
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");

    anim.ResetTrigger("landing");
    anim.SetTrigger("takingOff");
  }

  void land() {
    Debug.Log("Land");
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");

    anim.ResetTrigger("takingOff");
    anim.SetTrigger("landing");
  }
}
