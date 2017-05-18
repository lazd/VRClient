using System;
using UnityEngine;
using System.Collections;

public class Ant : WallWalker {
  /** Configuration */
  public float jumpSpeed = 9f; // vertical jump initial speed
  public float forwardJumpFactor = 0.2f;

  /** API */
  public bool isJumping = false;

  protected override void FixedUpdate() {
    base.wallWalk();

    // Perform jump
    if (isGrounded) {
      if (!isJumping && ((throttleStick != "" && Input.GetAxis(throttleStick) > 0) || Input.GetButton("Jump"))) {
        isJumping = true;
        StartCoroutine(jump());
      }
    }

  	// Set animation parameters
  	anim.SetBool("attack", Input.GetButtonDown("Fire1"));
  	anim.SetBool("attack2", Input.GetButtonDown("Fire2"));
  	anim.SetBool("attack3", Input.GetButtonDown("Fire3"));
  }

  protected IEnumerator jump() {
    // Forward motion takes away from max jump height
    rb.velocity += (jumpSpeed) * transform.up;

    // Add forward momentum
    rb.velocity += forwardMotion * forwardJumpFactor * transform.forward;

    // Don't allow jumping for a bit
    yield return new WaitForSeconds(0.5f);

    isJumping = false;
  }
}
