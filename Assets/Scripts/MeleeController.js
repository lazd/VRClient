#pragma strict

public var player: GameObject;

private var playerComponent : DroidControl;

function Start() {
	playerComponent = player.GetComponent('DroidControl');
}

// Hit enemies
function OnTriggerEnter (other: Collider) {
	if (other.gameObject.CompareTag('Enemy')) {
		if (playerComponent.isAttacking) {
			Debug.Log('Hitting item '+other.gameObject.name);

		    other.gameObject.SetActive(false);
		}		
		else {
			Debug.Log('Would have hit item '+other.gameObject.name);
		}
  }
}
