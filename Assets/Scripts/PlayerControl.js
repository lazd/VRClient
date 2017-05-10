#pragma strict

var rb: Rigidbody;
var speed : float = 3.0;
var jumpSpeed : float = 20.0;
var rotateSpeed : float = 0.7;
var gravity : float = 10.0;

private var moveDirection : Vector3 = Vector3.zero;
private var anim: Animator;
private var controller: CharacterController;

function Start () {
	rb = GetComponent.<Rigidbody>();
	anim = GetComponent.<Animator>();
	controller = GetComponent.<CharacterController>();
}

// Called before performing physics calculations
function FixedUpdate () {
	var moveHoriz = Input.GetAxis("Horizontal");
	var moveVert = Input.GetAxis("Vertical");
	var moveDirection = Vector3(0, 0, moveVert);

	if (controller.isGrounded) {
	    moveDirection = transform.TransformDirection(moveDirection);
	    moveDirection *= speed;

	    if (Input.GetButton ("Jump")) {
	        moveDirection.y = jumpSpeed;
	    }
	}

	// Apply gravity
	moveDirection.y -= gravity * Time.deltaTime;

	// Move the character
	controller.Move(moveDirection * Time.deltaTime);

	// Rotate the character
	transform.Rotate(0, moveHoriz * rotateSpeed * Time.deltaTime, 0);

	// Pass attack status
    anim.SetBool('Attack', Input.GetButton ("Fire1"));

    // Set the animation speed
    anim.SetFloat('Speed', moveVert);
}

function OnTriggerEnter (other: Collider) {
	if (other.gameObject.CompareTag('Pickup')) {
	    other.gameObject.SetActive(false);
    }
}
