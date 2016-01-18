using UnityEngine;
using System.Collections;
using UnityEditor;

public class BaseAnimator : MonoBehaviour
{

	public AnimClass anim;

	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}

/// <summary>
/// Класс, необходимый для создания нужного нам отображения полей аниматора
/// </summary>
[CustomEditor(typeof(BaseAnimator))]
public class BaseAnimatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
    }
}