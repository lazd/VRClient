#pragma strict

var wasp : Animator;


function Start () {

}

function Update () {
if(Input.GetKey(KeyCode.Alpha1)){
wasp.SetBool("walk",true);
wasp.SetBool("takeoff",false);
wasp.SetBool("landing",false);
wasp.SetBool("idle",false);
}
if(Input.GetKey(KeyCode.Alpha2)){
wasp.SetBool("takeoff",true);
wasp.SetBool("walk",false);
fly();
}
if(Input.GetKey(KeyCode.Alpha3)){
wasp.SetBool("attack",true);
wasp.SetBool("fly",false);
fly2();
}
if(Input.GetKey(KeyCode.Alpha4)){
wasp.SetBool("landing",true);
wasp.SetBool("fly",false);
idle();
}
if(Input.GetKey(KeyCode.Alpha5)){
wasp.SetBool("hit",true);
wasp.SetBool("fly",false);
fly2();
}
if(Input.GetKey(KeyCode.Alpha6)){
wasp.SetBool("die",true);
wasp.SetBool("fly",false);
}
}


function fly(){
yield WaitForSeconds(1.1);
wasp.SetBool("takeoff",false);
wasp.SetBool("attack",false);
wasp.SetBool("fly",true);
}

function fly2(){
yield WaitForSeconds(0.3);
wasp.SetBool("attack",false);
wasp.SetBool("hit",false);
wasp.SetBool("fly",true);
}


function idle(){
yield WaitForSeconds(1);
wasp.SetBool("landing",false);
wasp.SetBool("idle",true);
}

