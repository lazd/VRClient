// Based on http://wiki.unity3d.com/index.php?title=SmoothFollowWithCameraBumper
using System;
using UnityEngine;
using System.Collections;

public class MeleeWeapon : MonoBehaviour {
	public Droid playerComponent;

	// Hit enemies
	void OnTriggerEnter (Collider other) {
		if (other.gameObject.CompareTag("Enemy")) {
			if (playerComponent.isAttacking) {
				Debug.Log("Hitting item "+other.gameObject.name);

			  other.gameObject.SetActive(false);
			}		
			else {
				Debug.Log("Would have hit item "+other.gameObject.name);
			}
	  }
	}
}
