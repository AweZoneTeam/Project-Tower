using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;


//Часть аниматора ProjectTower, расположенная справа
public class RightAnimator : EditorWindow
{
	public List<animList> animTypes=new List<animList>();
	public List<PartController> parts = new List<PartController>();
	public List<string> aCharacters = new List<string>();
	public Rect windowRect1 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect2 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect3 = new Rect(100f, 100f, 200f, 200f);

	[HideInInspector] 
	public LeftAnimator leftAnim;
	[HideInInspector] 
	public AnimationEditorData animScene;
	public GameObject character;//Персонаж, что сейчас в поле зрения всего редактора анимаций


	private Vector2 scrollPosition=Vector2.zero;

	//Инициализация
	public void Initialize(AnimationEditorData aed, LeftAnimator la,List<string> ac, GameObject c)
	{
		animScene = aed;
		leftAnim = la;
		aCharacters = ac;
		character = c;
	}

	void OnGUI () 
	{
		CharactersList();
		if (character != null) {
			PartList ();
			AnimationList ();
		}
	}

	//Вывести список анимируемых частей, из которых состоит персонаж
	void PartList() 
	{
		parts = character.GetComponent<CharacterAnimator> ().parts;
		EditorGUILayout.BeginVertical ();
		{
			scrollPosition=GUI.BeginScrollView(new Rect(0f,0f,300f,200f),scrollPosition,new Rect(0,0,300,400));
			{
				for (int i = 0; i < aCharacters.Count; i++) {
					if (GUILayout.Button (parts [i].gameObject.name)) {
						if (!string.Equals (parts [i].gameObject.name, leftAnim.partName)) {
							leftAnim.partName = parts [i].gameObject.name;
							leftAnim.characterPart = parts [i];
						}
					}
				}
			}
			GUI.EndScrollView();		
			GUILayout.Space (210-(parts.Count>11 ? 11: parts.Count)*22);
			if(GUILayout.Button("+Add"))
			{
				AddPart ();
			}

		}
		EditorGUILayout.EndVertical();
	}

	//Вывести список всех анимаций данного персонажа
	void AnimationList () 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Animations");
			EditorGUILayout.Space ();
			animTypes = character.GetComponent<CharacterAnimator> ().animTypes;
			for (int i = 0; i < animTypes.Count; i++) {
				EditorGUILayout.LabelField (animTypes [i].typeName);
				for (int j = 0; j < animTypes [i].animations.Count; j++) {
					EditorGUILayout.LabelField (" "+animTypes [i].animations [j]);
				}
			}
		}
		EditorGUILayout.Space ();
		if(GUILayout.Button("+Add"))
		{
			AddAnimation ();
		}
		EditorGUILayout.EndVertical();	
	}

	//Вывести список персонажей, с которыми мы работаем в данный момент
	void CharactersList () 
	{
		aCharacters = animScene.animBase.usedCharacters;
		GUILayout.BeginVertical ();
		{
			scrollPosition=GUI.BeginScrollView(new Rect(0f,0f,300f,200f),scrollPosition,new Rect(0,0,300,400));
			{
				for (int i = 0; i < aCharacters.Count; i++) {
					if (GUILayout.Button (aCharacters [i])) {
						if (!string.Equals (aCharacters [i], leftAnim.characterName))
							SaveAndCreate (aCharacters [i], "", animScene.FindData (aCharacters [i] + ".asset"));
					}
				}
			}
			GUI.EndScrollView();		
			GUILayout.Space (210-(aCharacters.Count>11 ? 11: aCharacters.Count)*22);
			if (GUILayout.Button ("+Add"))
				AddCharacter ();


		}
		GUILayout.EndVertical ();
	}

	//Открыть окно добавления персонажа для работы с ним
	void AddCharacter()
	{
		
		AddSomethingWindow animScreen=(AddSomethingWindow)EditorWindow.GetWindow(typeof(AddSomethingWindow));
		animScreen.add = true;
		animScreen.rightAnim = this;
		animScreen.leftAnim = leftAnim;
		animScreen.maxSize=new Vector2(400f,400f);
		animScreen.minSize=new Vector2(400f,400f);
	}

	//Этим методом мы сохраняем все сделанные изменения в нашем персонаже и начинаем работать с новым персонажем
	public void SaveAndCreate(string characterName, string path, VisualData characterData)
	{
		if (leftAnim.saved)
			leftAnim.CreateNewInstance (characterName, path, characterData);
		else 
		{
			SaveWindow saveScreen = (SaveWindow)EditorWindow.GetWindow (typeof(SaveWindow));
			saveScreen.leftAnim = leftAnim;
			saveScreen.characterName = characterName;
			saveScreen.characterData = characterData;
		}
	}

	public void AddPart()//Открыть окно добавления новой части персонажу
	{
		AddPartWindow animScreen = (AddPartWindow)EditorWindow.GetWindow (typeof(AddPartWindow));
		animScreen.Initialize(this,leftAnim,character,"Assets/Animations/","Assets/Animations/Parts/");
	}


	public void AddAnimation()//Открыть окно добавления анимации
	{
		AddAnimationWindow animScreen = (AddAnimationWindow)EditorWindow.GetWindow (typeof(AddAnimationWindow));
		animScreen.Initialize (this,leftAnim,character);
	}
}
