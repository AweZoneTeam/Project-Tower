using UnityEngine;
using System.Collections;
using UnityEditor;

public class AnimatorScreen : SceneView 
{
	public GameObject focusObject;

	[ExecuteInEditMode]
	void Update()
	{
		if (focusObject!=null)
			AlignViewToObject (focusObject.transform);
		in2DMode = true;
        size = 20f;
	}
}
