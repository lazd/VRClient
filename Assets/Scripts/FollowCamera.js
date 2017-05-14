// Based on http://wiki.unity3d.com/index.php?title=SmoothFollowWithCameraBumper

public var target: GameObject;
public var damping: float = 1;
public var anticipateMotion: boolean = true;
public var rotationDamping: float = 0.75;
public var bumperDampTime : float = 0.5;
public var bumperCameraDistance : float = 4; 	// adjust camera height while bumping

public var hitDetection: boolean = false;

private var bumperRayOffset : Vector3; 			// allows offset of the bumper ray from target origin
private var offset: Vector3;
private var currentRotation: Vector3;
private var velocity: Vector3 = Vector3.zero;

private var cameraRayDistance: float;
function Start() {
	offset = target.transform.position - transform.position;
	currentRotation = target.transform.up;
	cameraRayDistance = Vector3.Distance(transform.position, target.transform.position);
}

// Pay attention to camera jitter
// https://forum.unity3d.com/threads/camera-jitter-problem.115224/#post-1854046

function FixedUpdate() {
	var desiredYAngle: float = target.transform.eulerAngles.y;
	var desiredXAngle: float = target.transform.eulerAngles.x;
	var rotation: Quaternion = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);

	// Calculate the desired new position	
	var desiredNewPosition = Vector3.SmoothDamp(transform.position, target.transform.position - (target.transform.rotation * offset), velocity, bumperDampTime);

	if (hitDetection) {
		//	 Cast a ray from the player to the new camera position
		var hit : RaycastHit;
		var dir: Vector3 = desiredNewPosition - target.transform.position;
		if (
			// Todo: Layermask argument?
			Physics.Raycast(target.transform.position, dir, hit, cameraRayDistance) &&
			hit.transform != target // ignore ray-casts that hit the user. DR
	    ) {
	    	// Approach 1: Move up just a little bit
	    	// Bug: bounces
			// var newPosition: Vector3 = hit.point + hit.normal * bumperCameraDistance;

			// Approach 2: Position exactly at hit point
			// Bug: swoops too fast around corners without having character in view
			var newPosition: Vector3 = hit.point;

			transform.position = Vector3.SmoothDamp(transform.position, newPosition, velocity, bumperDampTime);
		}
		else {
			transform.position = desiredNewPosition;
		}
	}
	else {
		transform.position = desiredNewPosition;
	}

	// Lerp rotation
	currentRotation = Vector3.Lerp(currentRotation, target.transform.up, Time.deltaTime * rotationDamping);

	// By default, look at the taraget
	var lookAtATarget = target.transform.position;

	if (anticipateMotion) {
		// If the player is moving, look slightly ahead of them
		var forwardMotion = target.forwardMotion;
		if (forwardMotion != 0 && target.isGrounded) {
			// Math based on http://answers.unity3d.com/questions/772331/spawn-object-in-front-of-player-and-the-way-he-is.html
			var playerDirection: Vector3 = target.transform.forward;
			var playerRotation: Quaternion = target.transform.rotation;
			var spawnDistance: float = forwardMotion;
			lookAtATarget = lookAtATarget + playerDirection * spawnDistance;
		}
	}

	transform.LookAt(lookAtATarget, currentRotation);
}
