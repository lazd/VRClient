private var anim: Animator;

function Start() {
	anim = GetComponent.<Animator>();
}

function FixedUpdate() {
	// Set animation parameters
	anim.SetBool('Attack1', Input.GetButtonDown('Fire1'));
	anim.SetBool('Attack2', Input.GetButtonDown('Fire2'));
	anim.SetBool('Attack3', Input.GetButtonDown('Fire3'));
}
