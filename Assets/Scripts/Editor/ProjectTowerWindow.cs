using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class ProjectTowerWindow : Editor{


	[MenuItem ("Project Tower/Ultimate Animator")]
	public static void  ShowW () 
	{
	    AnimationEditorData animData = GameObject.Find ("AnimationEdit").GetComponent<AnimationEditorData> ();
        AnimatorScreen animScreen=(AnimatorScreen)EditorWindow.GetWindow(typeof(AnimatorScreen));
		RightAnimator rAnimScreen = (RightAnimator)EditorWindow.GetWindow (typeof(RightAnimator));
		LeftAnimator lAnimScreen = (LeftAnimator)EditorWindow.GetWindow (typeof(LeftAnimator));
		lAnimScreen.position = new Rect (50f, 100f, 0f, 0f);
		animScreen.position = new Rect (400f, 100f, 0f, 0f);
		rAnimScreen.position = new Rect (750f, 100f, 0f, 0f);
		animScreen.maxSize = new Vector2 (400f, 750f);
		animScreen.minSize= new Vector2 (400f, 750f);
		rAnimScreen.maxSize = new Vector2 (300f, 750f);
		rAnimScreen.minSize= new Vector2 (300f, 750f);	
		lAnimScreen.maxSize = new Vector2 (300f, 750f);
		lAnimScreen.minSize= new Vector2 (300f, 750f);
		lAnimScreen.Initialize(rAnimScreen, animData, null, true);
		rAnimScreen.Initialize (animData, lAnimScreen, animData.animBase.usedCharacters, null);
		animScreen.focusObject = animData.gameObject;
    }

	void OnGUI () 
	{

	}
}
