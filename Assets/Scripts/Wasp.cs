// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
using System;
using UnityEngine;
using System.Collections;

public class Wasp : WallWalker {
  public float ascendSpeed = 5f;
  public float descendSpeed = 2f;
  public float groundMoveSpeed = 5f;
  public float airMoveSpeed = 20f;
  public float liftAccelTime = 1f;

  public float throttleThrust = 2f;

  private float initialGravity;
  private float currentVerticalLift;
  private float startVerticalLift;

  private float jumpInput;
  private float throttleInput;

  protected override void Start() {
    base.Start();

    startVerticalLift = ascendSpeed/2;

    initialGravity = gravity;

    Debug.Log(throttleStick);
  }

  protected override void FixedUpdate() {
    base.FixedUpdate();

    wallWalk();

    jumpInput = Input.GetAxis("Jump");
    throttleInput = throttleStick != "" ? Input.GetAxis(throttleStick) : 0;

    if (isGrounded) {
      gravity = initialGravity;
      moveSpeed = groundMoveSpeed;

      if (anim.GetCurrentAnimatorStateInfo(0).IsName("fly")) {
        land();
      }
      else {
        if (speed != 0) {
          walk();
        }
        else {
          idle();
        }

        if (jumpInput != 0 || (throttleInput > 0 && controlStyle != "Simple")) {
          takeOff();
        }
      }
    }
    else {
      gravity = 0;
      moveSpeed = airMoveSpeed;

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

    if (controlStyle == "Simple") {
      if (jumpInput != 0) {
        // Go up a bit
        transform.Translate(0, currentVerticalLift * Time.deltaTime, 0);
      }
      else if (!isGrounded) {
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
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");
    anim.SetTrigger("idling");
  }

  void walk() {
    anim.ResetTrigger("flying");
    anim.ResetTrigger("idling");
    anim.SetTrigger("walking");
  }

  void fly() {
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.SetTrigger("flying");
  }

  void hurt() {
    // Todo: Correpsonding animation transitions
    // Todo: Trigger from impacts
    anim.SetTrigger("hurting");
  }

  void attack() {
    // Todo: Trigger hits
    anim.SetTrigger("attacking");
  }

  void die() {
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");

    // Todo: Correpsonding animation transitions
    // Todo: Trigger from low health
    anim.SetTrigger("dying");
  }

  void takeOff() {
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");

    anim.ResetTrigger("landing");
    anim.SetTrigger("takingOff");
  }

  void land() {
    anim.ResetTrigger("idling");
    anim.ResetTrigger("walking");
    anim.ResetTrigger("flying");

    anim.ResetTrigger("takingOff");
    anim.SetTrigger("landing");
  }
}
