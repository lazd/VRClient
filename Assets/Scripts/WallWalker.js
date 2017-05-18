// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html

// Configuration
var moveSpeed: float = 8; // move speed
var turnSpeed: float = 90; // turning speed (degrees/second)
var strafeSpeed: float = 8; // turning speed (degrees/second)
var gravity: float = 20; // gravity acceleration
var forwardMotion: float = 0;
var airControlFactor: float = 0.5;
var accelTime: float = 1;

var controlStyle: String = 'Simple';
var forwardStick: String;
var strafeStick: String;
var yawStick: String;
var throttleStick: String;

var moveExpo: float = 1.8;

// API
var speed: float = 0; // The speed of the character
var isGrounded: boolean;

private var currentSpeed: float = 0;
private var yVelocity: float = 0.0;
private var startSpeed: float = moveSpeed / 2;

private var hit: RaycastHit;

private var curNormal: Vector3  = Vector3.zero;
private var usedNormal: Vector3  = Vector3.zero;
private var tiltToNormal: Quaternion;

public var attractionDistance: float = 2;
public var stickyRotationLerpFactor: float = 6;

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

	ApplyControlStyle();
}

function ApplyControlStyle() {
	if (controlStyle == "Simple") {
		forwardStick = "Throttle";
		strafeStick = "";
		yawStick = "Yaw";
		throttleStick = "";
	}
	else if (controlStyle == "Mode1") {
		throttleStick = "Pitch";
		forwardStick = "Throttle";
		strafeStick = "Roll";
		yawStick = "Yaw";
	}
	else if (controlStyle == "Mode2") {
		throttleStick = "Throttle";
		forwardStick = "Pitch";
		strafeStick = "Roll";
		yawStick = "Yaw";
	}
	else if (controlStyle == "Mode3") {
		throttleStick = "Pitch";
		forwardStick = "Throttle";
		strafeStick = "Yaw";
		yawStick = "Roll";
	}
	else if (controlStyle == "Mode4") {
		throttleStick = "Throttle";
		forwardStick = "Pitch";
		strafeStick = "Yaw";
		yawStick = "Roll";
	}
}

private var forwardInput: float;
private var yawInput: float;
private var strafeInput: float;

function FixedUpdate() {
	// Uncomment to catch inspector changes
	ApplyControlStyle();

	if (forwardStick != "") {
		forwardInput = Mathf.Sign(Input.GetAxis(forwardStick)) * Mathf.Pow(Mathf.Abs(Input.GetAxis(forwardStick)), moveExpo);
	}
	if (yawStick != "") {
		yawInput = Mathf.Sign(Input.GetAxis(yawStick)) * Mathf.Pow(Mathf.Abs(Input.GetAxis(yawStick)), moveExpo);
	}
	if (strafeStick != "") {
		strafeInput = Mathf.Sign(Input.GetAxis(strafeStick)) * Mathf.Pow(Mathf.Abs(Input.GetAxis(strafeStick)), moveExpo);
	} 

	// Add accelation component
	if (!forwardInput) {
		// Reset speed if we stop moving
		currentSpeed = startSpeed;
	}
	else {
		// Slowly increase speed
		currentSpeed = Mathf.SmoothDamp(currentSpeed, moveSpeed, yVelocity, accelTime);
	}

	var actualPosition = transform.position + GetComponent.<Collider>().center;

	// Calculate forward motion based on stick input
	forwardMotion = forwardInput * currentSpeed;

	if (Physics.Raycast (actualPosition, transform.forward, hit, attractionDistance)) {
		Debug.DrawRay (actualPosition, transform.forward, Color.blue, attractionDistance);
		
		usedNormal = hit.normal;
		curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
		tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
		transform.rotation = tiltToNormal;
	}
	else { 
		if (Physics.Raycast (actualPosition, -transform.up, hit, attractionDistance)) {
 			Debug.DrawRay (actualPosition, -transform.up, Color.green, attractionDistance);
 			usedNormal = hit.normal;
 			curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
 			tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
 			transform.rotation = tiltToNormal;
		}
		else {
      // Todo: why 0.3?
			if (Physics.Raycast (actualPosition + (-transform.up), -transform.forward + new Vector3 (0, .3, 0), hit, attractionDistance)) {
				Debug.DrawRay (actualPosition + (-transform.up), -transform.forward + new Vector3 (0, .3, 0), Color.green, attractionDistance);
				usedNormal = hit.normal;
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
	var groundedRayFudgeFactor = 0.5;
	// var groundedRayFudgeFactor = GetComponent.<Collider>().bounds.extents.y/2;
	var rayPosition = actualPosition + new Vector3(0, groundedRayFudgeFactor, 0);
	isGrounded = Physics.Raycast(rayPosition, -transform.up, distGround + groundedRayFudgeFactor + 0.05);

	// Turn left/right with horizontal axis:
	transform.Rotate(0, yawInput * turnSpeed * Time.deltaTime, 0);

	// Move with vertical axis
	if (forwardStick != "") {
		if (isGrounded) {
			transform.Translate(0, 0, forwardMotion * Time.deltaTime);
		}
		else if (airControlFactor != 0) {
			// Move slower in the air
			transform.Translate(0, 0, forwardMotion * airControlFactor * Time.deltaTime);
		}
	}

	if (strafeStick != "" && strafeSpeed != 0) {
		if (isGrounded) {
			// Move at moveSpeed on the ground
			transform.Translate(moveSpeed * strafeInput * Time.deltaTime, 0, 0);
		}
		else {
			// Move at strafeSpeed in the air
			transform.Translate(strafeSpeed * strafeInput * Time.deltaTime, 0, 0);
		}
	}

	// Apply constant force according to character normal
	// This keeps the walker stuck to the wall and acts as gravity
	// If this is applied unconditionally, gravity must be off on the RigidBody
	if (gravity != 0) {
		// Don't bother with function calls if gravity is off
		rb.AddForce(gravity * rb.mass * -transform.up);
	}

	// Set the character speed as a number, -1 to 1
	speed = forwardMotion/8;
	anim.SetFloat('speed', speed);
}

// Pick up items
function OnTriggerEnter (other: Collider) {
	if (other.gameObject.CompareTag('Pickup')) {
	    other.gameObject.SetActive(false);
    }
}
