// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html

var moveSpeed: float = 8; // move speed
var turnSpeed: float = 90; // turning speed (degrees/second)
var lerpSpeed: float = 10; // smoothing speed
var gravity: float = 20; // gravity acceleration
var isGrounded: boolean;
var deltaGround: float = 0.01; // character is grounded up to this distance
var jumpSpeed: float = 9; // vertical jump initial speed
var forwardJumpFactor: float = 0.2;
var forwardMotion: float = 0;
var airControlFactor: float = 0.5;

var climbRange: float = 0.5;

private var surfaceNormal: Vector3; // current surface normal
private var myNormal: Vector3; // character normal
private var distGround: float; // distance from character position to ground

private var anim: Animator;
private var rb: Rigidbody;

function Start() {
	anim = GetComponent.<Animator>();
	rb = GetComponent.<Rigidbody>();

	// normal starts as character up direction 
	myNormal = transform.up;

	// disable physics rotation
	rb.freezeRotation = true;

	// distance from transform.position to ground
	distGround = GetComponent.<Collider>().bounds.extents.y - GetComponent.<Collider>().center.y;
}

// Draw a line from in front of the ant, facing the ant, and downward at a 45°
// Add and attractive force to the point of intersection

var accelTime: float = 1;
private var currentSpeed: float = 0;
private var yVelocity: float = 0.0;
private var startSpeed: float = moveSpeed / 2;
private var hit: RaycastHit;

private var jumping: boolean = false;

function FixedUpdate() {
	if (isGrounded) {
		// Apply constant weight force according to character normal:
		// This keeps the walker stuck to the wall
		rb.AddForce(-gravity * rb.mass * myNormal);
	}

	var verticalInput = Input.GetAxis('Vertical');
	var horizontalInput = Input.GetAxis('Horizontal');

	// Add accelation component
	if (!verticalInput) {
		// Reset speed if we stop moving
		currentSpeed = startSpeed;
	}
	else {
		// Slowly increase speed
		currentSpeed = Mathf.SmoothDamp(currentSpeed, moveSpeed, yVelocity, accelTime);
	}

	// Calculate forward motion based on stick input
	forwardMotion = verticalInput * currentSpeed;

	if (isGrounded && Input.GetButtonDown('Jump') && !jumping) { // jump pressed:
		// Forward motion takes away from max jump height
		rb.velocity += (jumpSpeed - forwardMotion * forwardJumpFactor) * myNormal;
//		rb.velocity += (jumpSpeed) * myNormal;

		// Add forward momentum
		// Bug: Backwards momentum seems to be too much
		rb.velocity += (forwardMotion * (1 - forwardJumpFactor)) * transform.forward;

		jumping = true;
	}
	else {
		jumping = false;
	}

//	if (Physics.Raycast(transform.position, transform.forward, hit, climbRange)){ // wall ahead?
//		Debug.Log('Going to hit wall');
//		// Rotate up
////		transform.rotation.eulerAngles.x -= 4;
////		transform.position.x += 0.1;
////		transform.normal = hit.normal;
//	}

	// update surface normal and isGrounded:
	ray = Ray(transform.position, -myNormal); // cast ray downwards
	if (Physics.Raycast(ray, hit)) { // use it to update myNormal and isGrounded
		isGrounded = hit.distance <= distGround + deltaGround;
		surfaceNormal = hit.normal;
	}
	else {
		isGrounded = false;

		// assume usual ground normal to avoid 'falling forever'
		surfaceNormal = Vector3.up;
	}

	myNormal = Vector3.Lerp(myNormal, surfaceNormal, lerpSpeed * Time.deltaTime);

	// movement code - turn left/right with Horizontal axis:
	transform.Rotate(0, horizontalInput * turnSpeed * Time.deltaTime, 0);

	// find forward direction with new myNormal:
	var myForward = Vector3.Cross(transform.right, myNormal);

	// align character to the new myNormal while keeping the forward direction:
	var targetRot = Quaternion.LookRotation(myForward, myNormal);
	transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpSpeed * Time.deltaTime);

	// move the character forward/back with Vertical axis:
	if (isGrounded) {
		transform.Translate(0, 0, forwardMotion * Time.deltaTime);
	}
	else {
		transform.Translate(0, 0, forwardMotion * airControlFactor * Time.deltaTime);
	}

	anim.SetBool('Attack1', Input.GetButtonDown('Fire1'));
	anim.SetBool('Attack2', Input.GetButtonDown('Fire2'));
	anim.SetBool('Attack3', Input.GetButtonDown('Fire3'));

	// Set the character speed as a number, -1 to 1
	anim.SetFloat('Speed', forwardMotion/8);
}

// Pick up items
function OnTriggerEnter (other: Collider) {
	if (other.gameObject.CompareTag('Pickup')) {
	    other.gameObject.SetActive(false);
    }
}
