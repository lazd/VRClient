// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
using System;
using UnityEngine;
using System.Collections;

public class WallWalker : MonoBehaviour {

	/** Configuration */
	public float moveSpeed = 8f; // move speed
	public float turnSpeed = 90f; // turning speed (degrees/second)
	public float strafeSpeed = 8f; // turning speed (degrees/second)
	public float gravity = 20f; // gravity acceleration
	public float airControlFactor = 0.5f;
	public float accelTime = 1f;

	// Control style: Simple, Mode1, Mode2, Mode3, Mode4
	// Todo: enum
	public String controlStyle = "Simple";

	// The expo to apply to stick inputs
	// This eases twitchyness
	public float moveExpo = 1.8f;

	// The distance at which the character should start rotating towards a detected face normal to stick to it
	public float attractionDistance = 2f;

	// The lerp factor for rotations when sticking to walls
	public float stickyRotationLerpFactor = 6f;

	/** API */
	public float speed = 0; // The speed of the character
	public bool isGrounded;
	public float forwardMotion = 0; // The current forward motion of the character

	// Sticks to use for each action
	public String forwardStick;
	public String strafeStick;
	public String yawStick;
	public String throttleStick;


	private Vector3 surfaceNormal; // current surface normal
	private float distGround; // distance from character position to ground

	private Animator anim;
	private Rigidbody rb;

	private float forwardInput;
	private float yawInput;
	private float strafeInput;

	private float currentSpeed = 0;
	private float yVelocity = 0.0f;
	private float startSpeed;

	private RaycastHit hit;

	private Vector3 curNormal = Vector3.zero;
	private Vector3 usedNormal = Vector3.zero;
	private Quaternion tiltToNormal;


	void Start() {
		anim = GetComponent<Animator>();
		rb = GetComponent<Rigidbody>();
	
		// Start at half the normal speed
		startSpeed = moveSpeed / 2f;

		// Disable physics rotation
		rb.freezeRotation = true;

		// distance from transform.position to ground
		distGround = GetComponent<CapsuleCollider>().bounds.extents.y - GetComponent<CapsuleCollider>().center.y;

		ApplyControlStyle();
	}

	void ApplyControlStyle() {
		if (controlStyle == "Simple") {
			forwardStick = "Throttle";
			strafeStick = "";
			yawStick = "Yaw";
			throttleStick = "";
		}
		else if (controlStyle == "Mode1") {
			throttleStick = "Pitch";
			forwardStick = "Throttle";
			strafeStick = "Roll";
			yawStick = "Yaw";
		}
		else if (controlStyle == "Mode2") {
			throttleStick = "Throttle";
			forwardStick = "Pitch";
			strafeStick = "Roll";
			yawStick = "Yaw";
		}
		else if (controlStyle == "Mode3") {
			throttleStick = "Pitch";
			forwardStick = "Throttle";
			strafeStick = "Yaw";
			yawStick = "Roll";
		}
		else if (controlStyle == "Mode4") {
			throttleStick = "Throttle";
			forwardStick = "Pitch";
			strafeStick = "Yaw";
			yawStick = "Roll";
		}
	}

	void FixedUpdate() {
		// Uncomment to catch inspector changes
		ApplyControlStyle();

		if (forwardStick != "") {
			forwardInput = Mathf.Sign(Input.GetAxis(forwardStick)) * Mathf.Pow(Mathf.Abs(Input.GetAxis(forwardStick)), moveExpo);
		}
		if (yawStick != "") {
			yawInput = Mathf.Sign(Input.GetAxis(yawStick)) * Mathf.Pow(Mathf.Abs(Input.GetAxis(yawStick)), moveExpo);
		}
		if (strafeStick != "") {
			strafeInput = Mathf.Sign(Input.GetAxis(strafeStick)) * Mathf.Pow(Mathf.Abs(Input.GetAxis(strafeStick)), moveExpo);
		} 

		// Add accelation component
		if (forwardInput != 0 || strafeInput != 0) {
			// Slowly increase speed
			currentSpeed = Mathf.SmoothDamp(currentSpeed, moveSpeed, ref yVelocity, accelTime);
		}
		else {
			// Reset speed if we stop moving
			currentSpeed = startSpeed;
		}

		var actualPosition = transform.position + GetComponent<CapsuleCollider>().center;

		// Calculate forward motion based on stick input
		forwardMotion = forwardInput * currentSpeed;

		if (Physics.Raycast (actualPosition, transform.forward, out hit, attractionDistance)) {
			Debug.DrawRay (actualPosition, transform.forward, Color.blue, attractionDistance);
			
			usedNormal = hit.normal;
			curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
			tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
			transform.rotation = tiltToNormal;
		}
		else { 
			if (Physics.Raycast (actualPosition, -transform.up, out hit, attractionDistance)) {
	 			Debug.DrawRay (actualPosition, -transform.up, Color.green, attractionDistance);
	 			usedNormal = hit.normal;
	 			curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
	 			tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
	 			transform.rotation = tiltToNormal;
			}
			else {
	      // Todo: why 0.3?
				if (Physics.Raycast (actualPosition + (-transform.up), -transform.forward + new Vector3 (0, .3f, 0), out hit, attractionDistance)) {
					Debug.DrawRay (actualPosition + (-transform.up), -transform.forward + new Vector3 (0, .3f, 0), Color.green, attractionDistance);
					usedNormal = hit.normal;
					curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
					tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
					transform.rotation = tiltToNormal;
				}
				else {
					curNormal = Vector3.Lerp (curNormal, Vector3.up, stickyRotationLerpFactor * Time.deltaTime);
					tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
					transform.rotation = tiltToNormal;
				}
			}
		}

		// Cast ray downwards to detect if we"re on the ground
		var groundedRayFudgeFactor = 0.5f;
		// var groundedRayFudgeFactor = GetComponent<Collider>().bounds.extents.y/2;
		var rayPosition = actualPosition + new Vector3(0, groundedRayFudgeFactor, 0);
		isGrounded = Physics.Raycast(rayPosition, -transform.up, distGround + groundedRayFudgeFactor + 0.05f);

		// Turn left/right with horizontal axis:
		transform.Rotate(0, yawInput * turnSpeed * Time.deltaTime, 0);

		// Move with vertical axis
		if (forwardStick != "") {
			if (isGrounded) {
				transform.Translate(0, 0, forwardMotion * Time.deltaTime);
			}
			else if (airControlFactor != 0) {
				// Move slower in the air
				transform.Translate(0, 0, forwardMotion * airControlFactor * Time.deltaTime);
			}
		}

		if (strafeStick != "" && strafeSpeed != 0) {
			if (isGrounded) {
				// Move at currentSpeed on the ground
				transform.Translate(currentSpeed * strafeInput * Time.deltaTime, 0, 0);
			}
			else {
				// Move at strafeSpeed in the air
				transform.Translate(strafeSpeed * strafeInput * Time.deltaTime, 0, 0);
			}
		}

		// Apply constant force according to character normal
		// This keeps the walker stuck to the wall and acts as gravity
		// If this is applied unconditionally, gravity must be off on the RigidBody
		if (gravity != 0) {
			// Don't bother with function calls if gravity is off
			rb.AddForce(gravity * rb.mass * -transform.up);
		}

		// Set the character speed as a number, -1 to 1
		speed = forwardMotion/8;
		anim.SetFloat("speed", speed);
	}

	// Pick up items
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("Pickup")) {
			other.gameObject.SetActive(false);
	  }
	}
}
