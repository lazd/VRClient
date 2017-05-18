#pragma strict

var ascendSpeed: float = 5;
var descendSpeed: float = 2;
var groundMoveSpeed: float = 5;
var airMoveSpeed: float = 20;
var liftAccelTime: float = 1;

public var throttleThrust: float = 2;

private var anim: Animator;

private var idling: boolean = true;
private var walking: boolean = false;
private var takingOff: boolean = false;
private var flying: boolean = false;
private var landing: boolean = false;
private var hurting: boolean = false;
private var attacking: boolean = false;
private var dying: boolean = false;

private var wallWalker: WallWalker;
private var rb: Rigidbody;

private var initialGravity: float;
private var currentVerticalLift: float;
private var startVerticalLift: float = ascendSpeed/2;

private var yVelocity: float = 0.0;

function Start() {
	rb = GetComponent.<Rigidbody>();
	anim = GetComponent.<Animator>();
	wallWalker = GetComponent('WallWalker');

	initialGravity = wallWalker.gravity;
}

private var jumpInput: float;
private var throttleInput: float;

function Update() {
	jumpInput = Input.GetAxis("Jump");
	throttleInput = wallWalker.throttleStick ? Input.GetAxis(wallWalker.throttleStick) : 0;

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

		if (jumpInput || (throttleInput > 0 && wallWalker.controlStyle != 'Simple')) {
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

function FixedUpdate() {
	// Add accelation component
	if (jumpInput > 0) {
		// Slowly increase lift speed
		currentVerticalLift = Mathf.SmoothDamp(currentVerticalLift, ascendSpeed, yVelocity, liftAccelTime);
	}
	else {
		// Reset lift speed if we stop moving
		currentVerticalLift = Mathf.SmoothDamp(currentVerticalLift, startVerticalLift, yVelocity, liftAccelTime);
	}

	if (wallWalker.controlStyle == 'Simple') {
		if (jumpInput) {
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
		if (Input.GetButtonDown('Fire1')) {
			attack();
		}
	}

	// Todo: hit
	// Todo: dying

	// Set animation parameters
	anim.SetBool('idling', idling);
	anim.SetBool('walking', walking);
	anim.SetBool('takingOff', takingOff);
	anim.SetBool('flying', flying);
	anim.SetBool('landing', landing);
	anim.SetBool('attacking', attacking);
	anim.SetBool('dying', dying);
}

function idle() {
	walking = false;
	flying = false;

	idling = true;
}

function fly() {
	idling = false;
	walking = false;

	wallWalker.moveSpeed = airMoveSpeed;

	flying = true;
}

function walk() {
	idling = false;
	flying = false;

	walking = true;

	wallWalker.moveSpeed = groundMoveSpeed;
}

function hurt() {
	// Todo: Correpsonding animation transitions
	// Todo: Trigger from impacts
	hurting = true;
	yield WaitForFixedUpdate();
	hurting = false;
}

function attack() {
	// Todo: Trigger hits
	attacking = true;
	yield WaitForFixedUpdate();
	attacking = false;
}

function die() {
	idling = false;
	walking = false;
	flying = false;
	landing = false;
	takingOff = false;

	// Todo: Correpsonding animation transitions
	// Todo: Trigger from low health
	dying = true;
	yield WaitForFixedUpdate();
	dying = false;
}

function takeOff() {
	idling = false;
	walking = false;
	flying = false;
	landing = false;

	takingOff = true;
	yield WaitForFixedUpdate();
	takingOff = false;

	fly();
}

function land() {
	idling = false;
	walking = false;
	flying = false;
	takingOff = false;

	landing = true;
	yield WaitForFixedUpdate();
	landing = false;

	if (Input.GetAxis("Throttle")) {
		walk();
	}
	else {
		idle();
	}
}
