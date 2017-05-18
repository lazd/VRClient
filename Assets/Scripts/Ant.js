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

var isJumping: boolean = false;

function FixedUpdate() {
  // Perform jump
  if (wallWalker.isGrounded) {
    if (!isJumping && (Input.GetAxis(wallWalker.throttleStick) > 0 || Input.GetButton('Jump'))) {
      isJumping = true;
      jump();
    }
  }

	// Set animation parameters
	anim.SetBool('attack', Input.GetButtonDown('Fire1'));
	anim.SetBool('attack2', Input.GetButtonDown('Fire2'));
	anim.SetBool('attack3', Input.GetButtonDown('Fire3'));
}


function jump() {
  // Forward motion takes away from max jump height
  rb.velocity += (jumpSpeed) * transform.up;

  // Add forward momentum
  rb.velocity += wallWalker.forwardMotion * forwardJumpFactor * transform.forward;

  // Don't allow jumping for a bit
  yield WaitForSeconds(0.5);

  isJumping = false;
}
