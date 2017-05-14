// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html

var moveSpeed: float = 8; // move speed
var turnSpeed: float = 90; // turning speed (degrees/second)
var gravity: float = 20; // gravity acceleration
var isGrounded: boolean;
var deltaGround: float = 0.01; // character is grounded up to this distance
var jumpSpeed: float = 9; // vertical jump initial speed
var forwardJumpFactor: float = 0.2;
var forwardMotion: float = 0;
var airControlFactor: float = 0.5;

private var surfaceNormal: Vector3; // current surface normal
private var distGround: float; // distance from character position to ground

private var anim: Animator;
private var rb: Rigidbody;

function Start() {
	anim = GetComponent.<Animator>();
	rb = GetComponent.<Rigidbody>();

	// Disable physics rotation
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

private var curNormal: Vector3  = Vector3.zero;
private var usedNormal: Vector3  = Vector3.zero;
private var tiltToNormal: Quaternion;

public var attractionDistance: float = 2;
public var stickyRotationLerpFactor: float = 6;

function FixedUpdate() {
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

	var outhit: RaycastHit;
	if (Physics.Raycast (transform.position, transform.forward, outhit, attractionDistance)) {
		Debug.DrawRay (transform.position, transform.forward, Color.blue, attractionDistance);
		
		usedNormal = outhit.normal;
		curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
		tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
		transform.rotation = tiltToNormal;
	}
	else { 
		if (Physics.Raycast (transform.position, -transform.up, outhit, attractionDistance)) {
 			Debug.DrawRay (transform.position, -transform.up, Color.green, attractionDistance);
 			usedNormal = outhit.normal;
 			curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
 			tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
 			transform.rotation = tiltToNormal;
		}
		else {
         	// Todo: why 0.3?
			if (Physics.Raycast (transform.position + (-transform.up), -transform.forward + new Vector3 (0, .3, 0), outhit, attractionDistance)) {
				Debug.DrawRay (transform.position + (-transform.up), -transform.forward + new Vector3 (0, .3, 0), Color.green, attractionDistance);
				usedNormal = outhit.normal;
				curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
				tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
				transform.rotation = tiltToNormal;
			}
			else {
				curNormal = Vector3.Lerp (curNormal, Vector3.up, stickyRotationLerpFactor * Time.deltaTime);
				tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
				transform.rotation = tiltToNormal;
			}
		}
	}

	// Cast ray downwards to detect if we're on the ground
	ray = Ray(transform.position, -transform.up); 
	if (Physics.Raycast(ray, hit)) {
		isGrounded = hit.distance <= distGround + deltaGround;
	}
	else {
		isGrounded = false;
	}

	// Turn left/right with horizontal axis:
	transform.Rotate(0, horizontalInput * turnSpeed * Time.deltaTime, 0);

	// Move forward/back with vertical axis
	if (isGrounded) {
		transform.Translate(0, 0, forwardMotion * Time.deltaTime);
	}
	else {
		// Move slower in the air
		transform.Translate(0, 0, forwardMotion * airControlFactor * Time.deltaTime);
	}

	// Perform jump
	if (Input.GetButtonDown('Jump')) {
		if (isGrounded) {
			// Forward motion takes away from max jump height
			rb.velocity += (jumpSpeed) * transform.up;

			// Add forward momentum
			// Bug: Backwards momentum seems to be too much
			// Bug: Slingshots off of hills
			// Bug: Jumps too high randomly
			rb.velocity += forwardMotion * forwardJumpFactor * transform.forward;
		}
	}

	// Apply constant force according to character normal
	// This keeps the walker stuck to the wall and acts as gravity
	// If this is applied unconditionally, gravity must be off on the RigidBody
	rb.AddForce(-gravity * rb.mass * transform.up);

	// Set animation parameters
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
