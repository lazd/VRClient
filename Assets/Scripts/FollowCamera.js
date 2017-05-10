public var target: GameObject;
public var damping: float = 1;
private var offset: Vector3;
 
function Start() {
    offset = target.transform.position - transform.position;
}
 
function LateUpdate() {
	// Todo: Custom camera work for walking upside down
    var currentAngle: float = transform.eulerAngles.y;
    var desiredAngle: float = target.transform.eulerAngles.y;
    var angle: float = Mathf.LerpAngle(currentAngle, desiredAngle, Time.deltaTime * damping);
     
    var rotation: Quaternion = Quaternion.Euler(0, angle, 0);
    transform.position = target.transform.position - (rotation * offset);

    transform.LookAt(target.transform);
}
