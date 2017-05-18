var jumpSpeed: float = 9; // vertical jump initial speed
var forwardJumpFactor: float = 0.2;

private var anim: Animator;
private var rb: Rigidbody;
private var wallWalker: WallWalker;

function Start() {
	rb = GetComponent.<Rigidbody>();
  anim = GetComponent.<Animator>();
  wallWalker = GetComponent('WallWalker');
}

function FixedUpdate() {
  // Perform jump
  if (Input.GetButtonDown('Jump')) {
    if (wallWalker.isGrounded) {
      // Forward motion takes away from max jump height
      rb.velocity += (jumpSpeed) * transform.up;

      // Add forward momentum
      // Bug: Backwards momentum seems to be too much
      // Bug: Slingshots off of hills
      // Bug: Jumps too high randomly
      rb.velocity += wallWalker.forwardMotion * forwardJumpFactor * transform.forward;
    }
  }

	// Set animation parameters
	anim.SetBool('attack', Input.GetButtonDown('Fire1'));
	anim.SetBool('attack2', Input.GetButtonDown('Fire2'));
	anim.SetBool('attack3', Input.GetButtonDown('Fire3'));
}
