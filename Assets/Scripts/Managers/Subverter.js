#pragma strict

function Start () {

}

function Update () {

}

function BecomeDocile (mode : boolean) {
		if (mode == true)
		{
			Debug.Log("Move Controller becoming docile");
			gameObject.GetComponentInChildren(PlayerMoveController).enabled = false;
			gameObject.GetComponentInChildren(PlayerAnimation).enabled = false;
			gameObject.GetComponentInChildren(FreeMovementMotor).enabled = false;
		} else {
			Debug.Log("Move Controller becoming non-docile");
			gameObject.GetComponentInChildren(PlayerMoveController).enabled = true;
			gameObject.GetComponentInChildren(PlayerAnimation).enabled = true;
			gameObject.GetComponentInChildren(FreeMovementMotor).enabled = true;
		}
		
}
