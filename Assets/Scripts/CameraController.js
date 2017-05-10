#pragma strict

public var player: GameObject;
private var offset: Vector3;

private var startPosition: Vector3;

function Awake() {
	startPosition = transform.position;
}

function Start () {
	offset = this.transform.position - player.transform.position;
}

function LateUpdate () {
	this.transform.position = player.transform.position + offset;
	this.transform.position.y = startPosition.y;
}
