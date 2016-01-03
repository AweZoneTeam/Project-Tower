using UnityEngine;
using System.Collections;

public class TManager : MonoBehaviour {

	public Texture tex;
	public float texPropX;
	public float texPropY;
	
	void Awake()
	{
		gameObject.GetComponent<Renderer>().material.GetTexture (tex.name);
	}
}
