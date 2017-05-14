var speed : float = 3.0;
var rotateSpeed : float = 3.0;
var hitDistance : float = 1;
var isAttacking : boolean = false;

private var anim: Animator;
private var rb: Rigidbody;

function Start() {
	anim = GetComponent.<Animator>();
	rb = GetComponent.<Rigidbody>();

	// disable physics rotation
	rb.freezeRotation = true;
}

function Update () {
    var controller : CharacterController = GetComponent.<CharacterController>();

	var verticalInput = Input.GetAxis('Vertical');
	var horizontalInput = Input.GetAxis('Horizontal');

    // Rotate around y - axis
    transform.Rotate(0, horizontalInput * rotateSpeed, 0);

    // Move forward / backward
    var forward : Vector3 = transform.TransformDirection(Vector3.forward);
    var curSpeed : float = speed * verticalInput;
    controller.SimpleMove(forward * curSpeed);

 	anim.SetBool('Attack1', Input.GetButtonDown('Fire1'));
	anim.SetBool('Attack2', Input.GetButtonDown('Fire2'));

	// Todo: Do this with timing because this is silly and lasts too long
	isAttacking = anim.GetCurrentAnimatorStateInfo(0).IsName('melee_combat_attack_A');
	if (isAttacking) {
		// Do stuff?
	}

	// Set the character speed as a number, 0-1
	anim.SetFloat('Speed', verticalInput);
}

@script RequireComponent(CharacterController)
