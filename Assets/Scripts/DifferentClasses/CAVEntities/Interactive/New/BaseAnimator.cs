using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

#if UNITY_EDITOR
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
#endif