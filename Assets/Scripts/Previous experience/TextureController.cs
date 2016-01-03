using UnityEngine;
using System.Collections;

public class TextureController : MonoBehaviour 
{
	public float texX;
	public float texY;
	public GameObject parents;

	private float koofx, koofy;

	void Awake()
	{
		GetComponent<Renderer>().material.mainTextureScale = new Vector2 (transform.lossyScale.x * texX, transform.lossyScale.z * texY);
	}

	void FixedUpdate()
	{
		GetComponent<Renderer>().material.mainTextureScale = new Vector2 (transform.lossyScale.x * texX, transform.lossyScale.z * texY);
	}

}
