// Based on http://wiki.unity3d.com/index.php?title=SmoothFollowWithCameraBumper
using System;
using UnityEngine;
using System.Collections;

public class FollowCamera : MonoBehaviour {
  // The game object to follow
  public GameObject target;

  // How much to angle the camera up from the position of looking at the target
  public float cameraAngle = 0f;

  // Damping for position changes
  public float positionDamping = 0.3f;

  // Speed for rotation changes
  public float rotationSpeed = 0.6f;

  // Damping for position changes that result from a collision
  public float collisionDamping = 0.5f;

  // If true, the camera will avoid clipping other objects
  public bool collisionDetection = false;

  // Distance to move away from collision.
  // Increasing this makes the camera get closer to the subject when a collision happens
  public float collisionOffset = 4f;

  // The offset from the character
  public Vector3 offset;

  // Current camera rotation
  private Vector3 currentRotation;

  // The distance at which collisions should be deteched
  private float cameraRayDistance;

  // Velocity vector for damping
  private Vector3 dampVelocity = Vector3.zero;

  private RaycastHit hit;

  void Start() {
    // Set initial position based on offset
    transform.position = target.transform.position - offset;

    // Detect collisions at most the starting camera position away
    cameraRayDistance = Vector3.Distance(transform.position, target.transform.position);

    currentRotation = target.transform.up;
  }

  // Pay attention to camera jitter
  // https://forum.unity3d.com/threads/camera-jitter-problem.115224/#post-1854046

  void FixedUpdate() {
    // @Todo: Unhook from target when target destroyed

    // Calculate the desired new position 
    var desiredNewPosition = Vector3.SmoothDamp(transform.position, target.transform.position - (target.transform.rotation * offset), ref dampVelocity, positionDamping);

    if (collisionDetection) {
      //   Cast a ray from the player to the new camera position
      var dir = desiredNewPosition - target.transform.position;
      if (
        // Todo: Layermask argument?
        Physics.Raycast(target.transform.position, dir, out hit, cameraRayDistance) &&
        hit.transform != target // ignore ray-casts that hit the user. DR
      ) {
        // Approach 1: Move up just a little bit
        // Bug: bounces
        // var newPosition: Vector3 = hit.point + hit.normal * collisionOffset;

        // Approach 2: Position exactly at hit point
        // Bug: swoops too fast around corners without having character in view
        var newPosition = hit.point;

        transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref dampVelocity, collisionDamping);
      }
      else {
        transform.position = desiredNewPosition;
      }
    }
    else {
      transform.position = desiredNewPosition;
    }

    // Lerp rotation
    currentRotation = Vector3.Lerp(currentRotation, target.transform.up, Time.deltaTime * rotationSpeed);

    // By default, look at the taraget
    var lookAtATarget = target.transform.position;

    transform.LookAt(lookAtATarget, currentRotation);

    if (cameraAngle != 0) {
      // Tilt the camera up
      transform.rotation = transform.rotation * Quaternion.Euler(-cameraAngle, 0, 0);
    }
  }
}
