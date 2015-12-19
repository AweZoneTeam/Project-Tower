using UnityEngine;
using System.Collections;

public class Indicator : MonoBehaviour {

	public bool activated=true;

	public void Activate(bool a)
	{
		activated = a;
	}

	public bool isItActive()
	{
		return activated;
	}
}
