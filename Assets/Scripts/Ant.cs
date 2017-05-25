using System;
using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Ant : WallWalker {
  /** Configuration */
  public float jumpSpeed = 9f; // vertical jump initial speed
  public float forwardJumpFactor = 0.2f;

  /** API */
  public bool isJumping = false;

  protected override void FixedUpdate() {
    // Run WallWalker calculations so we get isGroudned and inputs
    base.calculate();

    // Be sticky no matter what
    base.beSticky();

    base.wallWalk();

    // Perform jump
    if (isGrounded) {
      if (!isJumping && ((throttleStick != "" && CrossPlatformInputManager.GetAxis(throttleStick) > 0) || CrossPlatformInputManager.GetButton("Jump"))) {
        isJumping = true;
        StartCoroutine(jump());
      }
    }

  	// Set animation parameters
  	anim.SetBool("attack", CrossPlatformInputManager.GetButtonDown("Fire1"));
  	anim.SetBool("attack2", CrossPlatformInputManager.GetButtonDown("Fire2"));
  	// anim.SetBool("attack3", CrossPlatformInputManager.GetButtonDown("Fire3"));
  }

  protected IEnumerator jump() {
    // Forward motion takes away from max jump height
    rb.velocity += (jumpSpeed) * transform.up;

    // Add forward momentum
    rb.velocity += forwardMotion * forwardJumpFactor * transform.forward;

    gravity -= jumpSpeed/2;

    // Don't allow jumping for a bit
    yield return new WaitForSeconds(0.5f);

    gravity += jumpSpeed/2;

    isJumping = false;
  }
}
