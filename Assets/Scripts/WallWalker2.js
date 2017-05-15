#pragma strict

function Start () {
	
}

private Vector3 curNormal = Vector3.zero;
private Vector3 usedNormal = Vector3.zero;
private Quaternion tiltToNormal;
public float turnSpeed = 2f;
public float MoveSpeed = 20f
function Update () {
 
	 moveDirection = transform.TransformDirection (moveDirection);        
         //rotates character
         var rotationAmount: float = Input.GetAxis ("Horizontal") * turnSpeed * Time.deltaTime;
         transform.RotateAround (transform.up, rotationAmount);
         
         //set up a variable to manipulate velocity directly. 
         Vector3 velocityAllAxis = transform.rigidbody.velocity;
         velocityAllAxis.x = moveDirection.x;
         velocityAllAxis.z = moveDirection.z;
             velocityAllAxis.y = moveDirection.y;
         transform.rigidbody.velocity = velocityAllAxis;    
 
 //now for detection and orientation.
 
         RaycastHit outhit;
         if (Physics.Raycast (transform.position, transform.forward, out outhit, 3f)) {
         
             Debug.DrawRay (transform.position, transform.forward, Color.blue, 2f);
         
             usedNormal = outhit.normal;
             curNormal = Vector3.Lerp (curNormal, usedNormal, 6.0f * Time.deltaTime);
             tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
             transform.rotation = tiltToNormal;
         
         } else { 
             if (Physics.Raycast (transform.position, -transform.up, out outhit, 3f)) {
             
                 Debug.DrawRay (transform.position, -transform.up, Color.green, 2f);
                 usedNormal = outhit.normal;
                 curNormal = Vector3.Lerp (curNormal, usedNormal, 6.0f * Time.deltaTime);
                 tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
                 transform.rotation = tiltToNormal;
             
             } else {
  
                 if (Physics.Raycast (transform.position + (-transform.up), -transform.forward + new Vector3 (0, .3f, 0), out outhit, 3f)) {
     
     
                     Debug.DrawRay (transform.position + (-transform.up), -transform.forward + new Vector3 (0, .3f, 0), Color.green, 2f);
                     usedNormal = outhit.normal;
                     curNormal = Vector3.Lerp (curNormal, usedNormal, 6.0f * Time.deltaTime);
                     tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
                     transform.rotation = tiltToNormal;
     
                 } else {
         
                     curNormal = Vector3.Lerp (curNormal, Vector3.up, 6.0f * Time.deltaTime);
                     tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
                     transform.rotation = tiltToNormal;
         
                 }
  
  
             }
 
         }
}
