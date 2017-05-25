// Based on http://answers.unity3d.com/questions/155907/basic-movement-walking-on-walls.html
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.CrossPlatformInput;

public class WallWalker : MonoBehaviour {

	/** Configuration */
	public float moveSpeed = 8f; // move speed
	public float turnSpeed = 90f; // turning speed (degrees/second)
	public float strafeSpeed = 8f; // turning speed (degrees/second)
	public float gravity = 20f; // gravity acceleration
	public float airControlFactor = 0.5f;
	public float accelTime = 1f;

	// Control style
	public enum ControlStyle
	{
		Simple,
		Mode1,
		Mode2,
		Mode3,
		Mode4
	}

	public ControlStyle controlStyle = ControlStyle.Mode2;

	// The expo to apply to stick inputs
	// This eases twitchyness
	public float moveExpo = 1.8f;

	// The distance at which the character should start rotating towards a detected face normal to stick to it
	public float attractionDistance = 2f;

	// The lerp factor for rotations when sticking to walls
	public float stickyRotationLerpFactor = 6f;

	public bool isGrounded;

	// Perform auto leveling against the up normal
	public bool autoLevel = true;

	/** API */
	protected float speed = 0; // The speed of the character
	protected float forwardMotion = 0; // The current forward motion of the character

	// Sticks to use for each action
	public String forwardStick;
	public String strafeStick;
	public String yawStick;
	public String throttleStick;

	protected float distGround; // distance from character position to ground

	protected Animator anim;
	protected Rigidbody rb;

	protected float currentSpeed = 0;
	protected float yVelocity = 0.0f;
	protected float startSpeed;

	protected float throttleInput;
	protected float forwardInput;
	protected float yawInput;
	protected float strafeInput;

	/** Internal */
	private Vector3 surfaceNormal; // current surface normal
	private RaycastHit hit;

	private Vector3 curNormal = Vector3.zero;
	private Vector3 usedNormal = Vector3.zero;
	private Quaternion tiltToNormal;

	private bool rayHit = false;

	protected virtual void Start() {
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

	public void ApplyControlStyle() {
		if (controlStyle == ControlStyle.Simple) {
			forwardStick = "Throttle";
			strafeStick = "";
			yawStick = "Yaw";
			throttleStick = "";
		}
		else if (controlStyle == ControlStyle.Mode1) {
			throttleStick = "Pitch";
			forwardStick = "Throttle";
			strafeStick = "Roll";
			yawStick = "Yaw";
		}
		else if (controlStyle == ControlStyle.Mode2) {
			throttleStick = "Throttle";
			forwardStick = "Pitch";
			strafeStick = "Roll";
			yawStick = "Yaw";
		}
		else if (controlStyle == ControlStyle.Mode3) {
			throttleStick = "Pitch";
			forwardStick = "Throttle";
			strafeStick = "Yaw";
			yawStick = "Roll";
		}
		else if (controlStyle == ControlStyle.Mode4) {
			throttleStick = "Throttle";
			forwardStick = "Pitch";
			strafeStick = "Yaw";
			yawStick = "Roll";
		}
	}

	protected virtual void FixedUpdate() {
		// Uncomment to catch inspector changes to controlStyle
		// ApplyControlStyle();
	}

	protected virtual void calculate() {
		if (throttleStick != "") {
			throttleInput = Mathf.Sign(CrossPlatformInputManager.GetAxis(throttleStick)) * Mathf.Pow(Mathf.Abs(CrossPlatformInputManager.GetAxis(throttleStick)), moveExpo);
		}
		if (forwardStick != "") {
			forwardInput = Mathf.Sign(CrossPlatformInputManager.GetAxis(forwardStick)) * Mathf.Pow(Mathf.Abs(CrossPlatformInputManager.GetAxis(forwardStick)), moveExpo);
		}
		if (yawStick != "") {
			yawInput = Mathf.Sign(CrossPlatformInputManager.GetAxis(yawStick)) * Mathf.Pow(Mathf.Abs(CrossPlatformInputManager.GetAxis(yawStick)), moveExpo);
		}
		if (strafeStick != "") {
			strafeInput = Mathf.Sign(CrossPlatformInputManager.GetAxis(strafeStick)) * Mathf.Pow(Mathf.Abs(CrossPlatformInputManager.GetAxis(strafeStick)), moveExpo);
		} 

		// Cast ray downwards to detect if we"re on the ground
		// var groundedRayFudgeFactor = GetComponent<Collider>().bounds.extents.y/2;
		// var rayPosition = transform.position + new Vector3(0, groundedRayFudgeFactor, 0);
		// isGrounded = Physics.Raycast(rayPosition, -transform.up, distGround + groundedRayFudgeFactor + 0.05f);

		isGrounded = Physics.Raycast(transform.position, -transform.up, distGround + 0.005f);
	}

	protected virtual void beSticky() {
		// From previous attempt to be more true to the capsule
		// var transform.position = transform.position + GetComponent<CapsuleCollider>().center;

		rayHit = false;
		if (Physics.Raycast (transform.position, transform.forward, out hit, attractionDistance)) {
			// Debug.DrawRay (transform.position, transform.forward, Color.blue, attractionDistance);
			usedNormal = hit.normal;
			curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
			tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
			transform.rotation = tiltToNormal;
			rayHit = true;
		}
		else { 
			if (Physics.Raycast (transform.position, -transform.up, out hit, attractionDistance)) {
	 			// Debug.DrawRay (transform.position, -transform.up, Color.green, attractionDistance);
	 			usedNormal = hit.normal;
	 			curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
	 			tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
	 			transform.rotation = tiltToNormal;
	 			rayHit = true;
			}
			else {
	      // Todo: why 0.3?
				if (Physics.Raycast (transform.position + (-transform.up), -transform.forward + new Vector3 (0, .03f, 0), out hit, attractionDistance)) {
					// Debug.DrawRay (transform.position + (-transform.up), -transform.forward + new Vector3 (0, .3f, 0), Color.red, attractionDistance);
					usedNormal = hit.normal;
					curNormal = Vector3.Lerp (curNormal, usedNormal, stickyRotationLerpFactor * Time.deltaTime);
					tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
					transform.rotation = tiltToNormal;
					rayHit = true;
				}
				else if (autoLevel) {
					curNormal = Vector3.Lerp (curNormal, Vector3.up, stickyRotationLerpFactor * Time.deltaTime);
					tiltToNormal = Quaternion.FromToRotation (transform.up, curNormal) * transform.rotation;
					transform.rotation = tiltToNormal;
					rayHit = true;
				}
			}
		}
	}

	protected virtual void wallWalk() {
		calculate();

		// Add accelation component
		if (forwardInput != 0 || strafeInput != 0) {
			// Slowly increase speed
			currentSpeed = Mathf.SmoothDamp(currentSpeed, moveSpeed, ref yVelocity, accelTime);
		}
		else {
			// Reset speed if we stop moving
			currentSpeed = startSpeed;
		}

		// Calculate forward motion based on stick input
		forwardMotion = forwardInput * currentSpeed;

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
		if (gravity != 0 && (isGrounded || rayHit)) {
			// Don't bother with function calls if gravity is off
			rb.AddForce(gravity * rb.mass * -transform.up);
		}

		// Set the character speed as a number, -1 to 1
		speed = forwardMotion/8;
		anim.SetFloat("speed", speed);
	}

	// Pick up items
	protected void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("Pickup")) {
			other.gameObject.SetActive(false);
	  }
	}
}
