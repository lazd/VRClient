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

  private bool idling = true;
  private bool walking = false;
  private bool takingOff = false;
  private bool flying = false;
  private bool landing = false;
  private bool hurting = false;
  private bool attacking = false;
  private bool dying = false;

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

  void Update() {
    jumpInput = Input.GetAxis("Jump");
    throttleInput = wallWalker.throttleStick != "" ? Input.GetAxis(wallWalker.throttleStick) : 0;

    if (wallWalker.isGrounded) {
      wallWalker.gravity = initialGravity;

      if (flying) {
        land();
      }

      if (Mathf.Abs(wallWalker.speed) > 0) {
        walk();
      }
      else {
        idle();
      }

      if (jumpInput != 0 || (throttleInput > 0 && wallWalker.controlStyle != "Simple")) {
        if (!takingOff) {
          takeOff();
        }
        else {
          Debug.Log("Jump input caused takeOff condition to be called");
        }
      }
    }
    else {
      wallWalker.gravity = 0;
      fly();
    }
  }

  void FixedUpdate() {
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
      // Bug: Need to clamp force somehow, can go so fast it flies through things
      rb.AddForce(transform.up * throttleInput * throttleThrust, ForceMode.Impulse);
    }

    if (flying) {
      // Attack while flying only
      if (Input.GetButtonDown("Fire1")) {
        attack();
      }
    }

    // Todo: hit
    // Todo: dying

    // Set animation parameters
    anim.SetBool("idling", idling);
    anim.SetBool("walking", walking);
    anim.SetBool("takingOff", takingOff);
    anim.SetBool("flying", flying);
    anim.SetBool("landing", landing);
    anim.SetBool("attacking", attacking);
    anim.SetBool("dying", dying);
  }

  void idle() {
    walking = false;
    flying = false;

    idling = true;
  }

  void fly() {
    idling = false;
    walking = false;

    wallWalker.moveSpeed = airMoveSpeed;

    flying = true;
  }

  void walk() {
    idling = false;
    flying = false;

    walking = true;

    wallWalker.moveSpeed = groundMoveSpeed;
  }

  IEnumerator hurt() {
    // Todo: Correpsonding animation transitions
    // Todo: Trigger from impacts
    hurting = true;
    yield return new WaitForFixedUpdate();
    hurting = false;
  }

  IEnumerator attack() {
    // Todo: Trigger hits
    attacking = true;
    yield return new WaitForFixedUpdate();
    attacking = false;
  }

  IEnumerator die() {
    idling = false;
    walking = false;
    flying = false;
    landing = false;
    takingOff = false;

    // Todo: Correpsonding animation transitions
    // Todo: Trigger from low health
    dying = true;
    yield return new WaitForFixedUpdate();
    dying = false;
  }

  IEnumerator takeOff() {
    idling = false;
    walking = false;
    flying = false;
    landing = false;

    takingOff = true;
    yield return new WaitForFixedUpdate();
    takingOff = false;

    fly();
  }

  IEnumerator land() {
    idling = false;
    walking = false;
    flying = false;
    takingOff = false;

    landing = true;
    yield return new WaitForFixedUpdate();
    landing = false;

    if (Input.GetAxis("Throttle") != 0) {
      walk();
    }
    else {
      idle();
    }
  }
}
