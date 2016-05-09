using UnityEngine;
using System.Collections;

public class RemovingObj : MonoBehaviour {

	public float lifeTime = 1000;

	void Update () {
		if(lifeTime>0)
			lifeTime-=Time.deltaTime;
		else
			Destroy(gameObject);
	}
}
