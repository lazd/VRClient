public var target: GameObject;
public var damping: float = 1;
public var anticipateMotion: boolean = true;
public var rotationDamping: float = 0.75;
public var bumperDampTime : float = 0.5;
public var bumperDistanceCheck : float = 0.5;	// length of bumper ray
public var bumperCameraHeight : float = 0.2; 	// adjust camera height while bumping

private var bumperRayOffset : Vector3; 			// allows offset of the bumper ray from target origin
private var offset: Vector3;
private var currentOffset: Vector3;
private var currentRotation: Vector3;
private var velocity: Vector3 = Vector3.zero;

function Start() {
	offset = target.transform.position - transform.position;
	currentOffset = target.transform.position - transform.position;
	currentRotation = target.transform.up;
}

function LateUpdate() {
	var desiredYAngle: float = target.transform.eulerAngles.y;
	var desiredXAngle: float = target.transform.eulerAngles.x;
	var rotation: Quaternion = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);

	// Calculate the desired new position
	var desiredNewPosition = Vector3.SmoothDamp(transform.position, target.transform.position - (rotation * currentOffset), velocity, bumperDampTime);

	// check to see if there is anything behind the target
	var hit : RaycastHit;

	// Cast a ray from the player to the new camera position
	var dir: Vector3 = desiredNewPosition - target.transform.position;
	if (
		// Todo: Layermask argument?
		Physics.Raycast(target.transform.position, dir, hit, bumperDistanceCheck) &&
		hit.transform != target // ignore ray-casts that hit the user. DR
    ) {
    	// Adjust the camera position offset upward and backwards if its colliding
		currentOffset.y -= bumperCameraHeight;
		currentOffset.z -= bumperCameraHeight;
	}
	else {
		// Reset the offset if there is no longer a collision
		// Todo: this causes bounce when the player is static and pointing down a hill
		currentOffset = Vector3.Lerp(currentOffset, offset, Time.deltaTime * damping);
	}

	// Calculate the actual new position
	var newPosition = Vector3.SmoothDamp(transform.position, target.transform.position - (rotation * currentOffset), velocity, bumperDampTime);
	transform.position = newPosition;

	// Lerp rotation
	currentRotation = Vector3.Lerp(currentRotation, target.transform.up, Time.deltaTime * rotationDamping);

	// By default, look at the taraget
	var lookAtATarget = target.transform.position;

	if (anticipateMotion) {
		// If the player is moving, look slightly ahead of them
		var forwardMotion = target.forwardMotion;
		if (forwardMotion != 0 && target.isGrounded) {
			var playerPos: Vector3 = target.transform.position;
			var playerDirection: Vector3 = target.transform.forward;
			var playerRotation: Quaternion = target.transform.rotation;
			var spawnDistance: float = forwardMotion;
			lookAtATarget = lookAtATarget + playerDirection * spawnDistance;
		}
	}

	transform.LookAt(lookAtATarget, currentRotation);
}
