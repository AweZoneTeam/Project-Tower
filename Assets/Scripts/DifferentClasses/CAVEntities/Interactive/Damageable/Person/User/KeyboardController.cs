using UnityEngine;
using System.Collections;

public class KeyboardController : MonoBehaviour
{
	public delegate void FirePressed();
	public event FirePressed onFire = delegate { };


	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

