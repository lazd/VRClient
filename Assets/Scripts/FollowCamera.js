// Based on http://wiki.unity3d.com/index.php?title=SmoothFollowWithCameraBumper

// The game object to follow
public var target: GameObject;

// Damping for position changes
public var positionDamping: float = 0.3;

// Damping for rotation changes
public var rotationDamping: float = 0.6;

// Damping for position changes that result from a collision
public var collisionDamping : float = 0.5;

// If true, the camera will point slightly in front of the character
public var anticipateMotion: boolean = true;

// If true, the camera will avoid clipping other objects
public var collisionDetection: boolean = false;

// Distance to move away from collision.
// Increasing this makes the camera get closer to the subject when a collision happens
public var collisionOffset : float = 4;

// The offset from the character
public var offset: Vector3;

// Current camera rotation
private var currentRotation: Vector3;

// The distance at which collisions should be deteched
private var cameraRayDistance: float;

// Velocity vector for damping
private var dampVelocity: Vector3 = Vector3.zero;

function Start() {
	// Set initial position based on offset
	transform.position = target.transform.position - offset;

	// Detect collisions at most the starting camera position away
	cameraRayDistance = Vector3.Distance(transform.position, target.transform.position);

	currentRotation = target.transform.up;
}

// Pay attention to camera jitter
// https://forum.unity3d.com/threads/camera-jitter-problem.115224/#post-1854046

function FixedUpdate() {
	var desiredYAngle: float = target.transform.eulerAngles.y;
	var desiredXAngle: float = target.transform.eulerAngles.x;
	var rotation: Quaternion = Quaternion.Euler(desiredXAngle, desiredYAngle, 0);

	// Calculate the desired new position	
	var desiredNewPosition = Vector3.SmoothDamp(transform.position, target.transform.position - (target.transform.rotation * offset), dampVelocity, positionDamping);

	if (collisionDetection) {
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
			// var newPosition: Vector3 = hit.point + hit.normal * collisionOffset;

			// Approach 2: Position exactly at hit point
			// Bug: swoops too fast around corners without having character in view
			var newPosition: Vector3 = hit.point;

			transform.position = Vector3.SmoothDamp(transform.position, newPosition, dampVelocity, collisionDamping);
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
