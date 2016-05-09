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

    /// <summary>
    /// Исправить срочно!!!!
    /// </summary>
	void OnTriggerEnter(Collider other)
	{
		if(string.Equals(other.tag, "Enemy"))
		{
			GetComponent<RemovingObj>().lifeTime = 3;
			transform.parent = other.transform;
			GetComponent<Rigidbody>().isKinematic = true;
		}
	}

}
