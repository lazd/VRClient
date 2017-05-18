using System;
using UnityEngine;
using System.Collections;

public class Ant : MonoBehaviour {
  /** Configuration */
  public float jumpSpeed = 9f; // vertical jump initial speed
  public float forwardJumpFactor = 0.2f;

  /** API */
  public bool isJumping = false;

  private Animator anim;
  private Rigidbody rb;
  private WallWalker wallWalker;

  void Start() {
  	rb = GetComponent<Rigidbody>();
    anim = GetComponent<Animator>();
    wallWalker = GetComponent<WallWalker>();
  }

  void FixedUpdate() {
    // Perform jump
    if (wallWalker.isGrounded) {
      if (!isJumping && ((wallWalker.throttleStick != "" && Input.GetAxis(wallWalker.throttleStick) > 0) || Input.GetButton("Jump"))) {
        isJumping = true;
        StartCoroutine(jump());
      }
    }

  	// Set animation parameters
  	anim.SetBool("attack", Input.GetButtonDown("Fire1"));
  	anim.SetBool("attack2", Input.GetButtonDown("Fire2"));
  	anim.SetBool("attack3", Input.GetButtonDown("Fire3"));
  }

  IEnumerator jump() {
    // Forward motion takes away from max jump height
    rb.velocity += (jumpSpeed) * transform.up;

    // Add forward momentum
    rb.velocity += wallWalker.forwardMotion * forwardJumpFactor * transform.forward;

    // Don't allow jumping for a bit
    yield return new WaitForSeconds(0.5f);

    isJumping = false;
  }
}
