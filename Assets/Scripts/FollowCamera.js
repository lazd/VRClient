public var target: GameObject;
public var damping: float = 1;
private var offset: Vector3;
private var currentOffset: Vector3;

public var bumperDampTime : float = 0.5;
public var collisionDampTime : float = 0.5;
public var bumperDistanceCheck : float = 0.5;	// length of bumper ray
public var bumperCameraHeight : float = 0.2; 	// adjust camera height while bumping
private var bumperRayOffset : Vector3; 			// allows offset of the bumper ray from target origin

private var velocity: Vector3 = Vector3.zero;

function Start() {
    offset = target.transform.position - transform.position;
    currentOffset = target.transform.position - transform.position;
}

function LateUpdate() {
	// Todo: Custom camera work for walking upside down
    var desiredAngle: float = target.transform.eulerAngles.y;
    var rotation: Quaternion = Quaternion.Euler(0, desiredAngle, 0);

	// Calculate the desired new position
	var newPosition = Vector3.SmoothDamp(transform.position, target.transform.position - (rotation * currentOffset), velocity, bumperDampTime);

    // check to see if there is anything behind the target
	var hit : RaycastHit;

	// Cast a ray from the player to the new camera position
	var dir: Vector3 = newPosition - target.transform.position;
	if (
		// Todo: Layermask argument?
		Physics.Raycast(target.transform.position, dir, hit, bumperDistanceCheck) &&
        hit.transform != target // ignore ray-casts that hit the user. DR
    ) {
    	// Adjust the camera position if its colliding
		currentOffset.y -= bumperCameraHeight;
		currentOffset.z -= bumperCameraHeight;
	}
	else {
		// Reset the offset if there is no longer a collision
		// Todo: this causes bounce when the player is static and pointing down a hill
		currentOffset = Vector3.Lerp(currentOffset, offset, Time.deltaTime * damping);
    }

    // Todo: follow player from behind when walking up a hill
	transform.position = Vector3.SmoothDamp(transform.position, target.transform.position - (rotation * currentOffset), velocity, bumperDampTime);

    transform.LookAt(target.transform);
}
