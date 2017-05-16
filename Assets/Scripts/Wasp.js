#pragma strict

var anim: Animator;

function Start() {
	anim = GetComponent.<Animator>();
}

function FixedUpdate() {
	if(Input.GetKey(KeyCode.Alpha1)){
		anim.SetBool("walk",true);
		anim.SetBool("takeoff",false);
		anim.SetBool("landing",false);
		anim.SetBool("idle",false);
	}
	if(Input.GetKey(KeyCode.Alpha2)){
		anim.SetBool("takeoff",true);
		anim.SetBool("walk",false);
		fly();
	}
	if(Input.GetKey(KeyCode.Alpha3)){
		anim.SetBool("attack",true);
		anim.SetBool("fly",false);
		fly2();
	}
	if(Input.GetKey(KeyCode.Alpha4)){
		anim.SetBool("landing",true);
		anim.SetBool("fly",false);
		idle();
	}
	if(Input.GetKey(KeyCode.Alpha5)){
		anim.SetBool("hit",true);
		anim.SetBool("fly",false);
		fly2();
	}
	if(Input.GetKey(KeyCode.Alpha6)){
		anim.SetBool("die",true);
		anim.SetBool("fly",false);
	}
}


function fly(){
	yield WaitForSeconds(1.1);
	anim.SetBool("takeoff",false);
	anim.SetBool("attack",false);
	anim.SetBool("fly",true);
}

function fly2(){
	yield WaitForSeconds(0.3);
	anim.SetBool("attack",false);
	anim.SetBool("hit",false);
	anim.SetBool("fly",true);
}


function idle(){
	yield WaitForSeconds(1);
	anim.SetBool("landing",false);
	anim.SetBool("idle",true);
}

