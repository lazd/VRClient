#pragma strict

var ascendSpeed: float = 5;
var descendSpeed: float = 2;
var groundMoveSpeed: float = 5;
var airMoveSpeed: float = 20;
var liftAccelTime: float = 1;

private var anim: Animator;

private var idle: boolean = true;
private var walk: boolean = false;
private var takeoff: boolean = false;
private var fly: boolean = false;
private var land: boolean = false;
private var attack: boolean = false;
private var die: boolean = false;

private var wallWalker: WallWalker;
private var rb: Rigidbody;
private var col: CapsuleCollider;

private var initialGravity: float;
private var currentVerticalLift: float;
private var startVerticalLift: float = ascendSpeed/5;

private var yVelocity: float = 0.0;

function Start() {
	rb = GetComponent.<Rigidbody>();
	anim = GetComponent.<Animator>();
	col = GetComponent.<CapsuleCollider>();
	wallWalker = GetComponent('WallWalker');

	// Store intial gravity so we can shut it off
	initialGravity = wallWalker.gravity;
}

function FixedUpdate() {
	var jumpInput: float = Input.GetAxis('Jump');

	// Add accelation component
	if (!jumpInput && currentVerticalLift != startVerticalLift) {
		// Reset lift speed if we stop moving
		currentVerticalLift = Mathf.SmoothDamp(currentVerticalLift, startVerticalLift, yVelocity, liftAccelTime);
	}
	else {
		// Slowly increase lift speed
		currentVerticalLift = Mathf.SmoothDamp(currentVerticalLift, ascendSpeed, yVelocity, liftAccelTime);
	}

	// Center the collider based on the capsulePosition animation parameter set in curves
	// col.center.y = anim.GetFloat('capsulePosition') * 0.017 + 0.005;

	if (wallWalker.isGrounded) {
		wallWalker.moveSpeed = groundMoveSpeed;
		
		if (fly) {
			doLand();
		}

		if (Mathf.Abs(wallWalker.speed) > 0) {
			// If we're on the ground and moving, walk
			idle = false;
			walk = true;
		}
		else {
			idle = true;
			walk = false;
		}

		if (jumpInput) {
			if (!takeoff) {
				doTakeoff();
			}

			// Go up a bit
			transform.Translate(0, currentVerticalLift * Time.deltaTime, 0);
		}
	}
	else {
		wallWalker.moveSpeed = airMoveSpeed;

		// If we're in the air, flap wings
		idle = false;
		fly = true;
		walk = false;

		if (jumpInput) {
			// Go up a bit
			transform.Translate(0, currentVerticalLift * Time.deltaTime, 0);
		}
		else {
			// Go down a bit
			transform.Translate(0, -descendSpeed * Time.deltaTime, 0);
		}
	}
	
	// Attack while flying, walking, or idle
	if ((fly || walk || idle) && Input.GetButtonDown('Fire1')) {
		attack = true;
	}
	else {
		attack = false;
	}

	if (fly) {
		// Counteract gravity when we're in the air
		// rb.AddForce(wallWalker.gravity * rb.mass * transform.up);
	}

	// Todo: if grounded and jump pressed, takeoff
	// Todo: if flying and then grounded, landing
	// Todo: hit
	// Todo: die

	// Set animation parameters
	anim.SetBool('idle', idle);
	anim.SetBool('walk', walk);
	anim.SetBool('takeoff', takeoff);
	anim.SetBool('fly', fly);
	anim.SetBool('land', land);
	anim.SetBool('attack', attack);
	anim.SetBool('die', die);
}

function doTakeoff() {
	idle = false;
	takeoff = true;
	wallWalker.gravity = 0;
	yield WaitForSeconds(0.5);
	takeoff = false;
	fly = true;
}


function doLand(){
	idle = false;
	fly = false;
	land = true;
	wallWalker.gravity = initialGravity;
	yield WaitForSeconds(0.5);
	land = false;
}
