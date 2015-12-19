using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimatorScreen : SceneView 
{
	public Camera cam;

	[ExecuteInEditMode]
	void Update()
	{
		in2DMode = true;
		AlignViewToObject (cam.transform);
	}
}
