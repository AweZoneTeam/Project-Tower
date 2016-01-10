using UnityEngine;
using System.Collections;

public class KeyboardActorController : MonoBehaviour
{

	public HumanoidActorActions Actions;
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKey(KeyCode.D)) {
			print ("kek");
			Actions.Turn (OrientationEnum.Right);
			Actions.StartWalking (OrientationEnum.Right);
		}
		if (Input.GetKeyUp(KeyCode.D)) {
			Actions.StopWalking ();
		}
		if (Input.GetKey(KeyCode.A)) {
			print ("kek");
			Actions.Turn (OrientationEnum.Left);
			Actions.StartWalking (OrientationEnum.Left);
		}
		if (Input.GetKeyUp(KeyCode.A)) {
			Actions.StopWalking ();
		}
		if (Input.GetKeyDown (KeyCode.Space)) {
			Actions.Jump();
		}
	}
}

