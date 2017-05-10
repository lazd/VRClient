﻿var moveSpeed: float = 6; // move speed
var turnSpeed: float = 90; // turning speed (degrees/second)
var lerpSpeed: float = 10; // smoothing speed
var gravity: float = 10; // gravity acceleration
var isGrounded: boolean;
var deltaGround: float = 0.2; // character is grounded up to this distance
var jumpSpeed: float = 10; // vertical jump initial speed
var forwardJumpFactor: float = 2;

private var surfaceNormal: Vector3; // current surface normal
private var myNormal: Vector3; // character normal
private var distGround: float; // distance from character position to ground
private var vertSpeed: float = 0; // vertical jump current speed 

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

function FixedUpdate() {
	if (isGrounded) {
		// apply constant weight force according to character normal:
		rb.AddForce(-gravity * rb.mass * myNormal);
	}
}

function Update() {
	var verticalInput = Input.GetAxis('Vertical');
	var horizontalInput = Input.GetAxis('Horizontal');

	var forwardMotion = verticalInput * moveSpeed;

	// jump code - jump to wall or simple jump
	var ray: Ray;
	var hit: RaycastHit;
	if (isGrounded && Input.GetButtonDown('Jump')) { // jump pressed:
		// Forward motion takes away from max jump height
		rb.velocity += (jumpSpeed - forwardMotion * forwardJumpFactor) * myNormal;

		// Add forward momentum
		// Bug: Backwards momentum seems to be too much
		rb.velocity += forwardMotion * transform.forward;
	}

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

	// move the character forth/back with Vertical axis:
	if (isGrounded) {
		transform.Translate(0, 0, forwardMotion * Time.deltaTime);
	}

	anim.SetBool('Attack1', Input.GetButtonDown('Fire1'));
	anim.SetBool('Attack2', Input.GetButtonDown('Fire2'));
	anim.SetBool('Attack3', Input.GetButtonDown('Fire3'));

	// Set the character speed as a number, 0-1
	anim.SetFloat('Speed', forwardMotion/8);
}

// Pick up items
function OnTriggerEnter (other: Collider) {
	if (other.gameObject.CompareTag('Pickup')) {
	    other.gameObject.SetActive(false);
    }
}
