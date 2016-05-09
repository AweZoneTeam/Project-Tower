using UnityEngine;
using System.Collections;

public class Arrow : MonoBehaviour {

	void Update()
	{
		if(GetComponent<Rigidbody>().velocity.magnitude>0)
		{
			transform.rotation = Quaternion.LookRotation(GetComponent<Rigidbody>().velocity);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(GetComponent<HitController>().enemies.Contains(other.tag))
		{
			GetComponent<RemovingObj>().lifeTime = 3;
			transform.parent = other.transform;
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

}
