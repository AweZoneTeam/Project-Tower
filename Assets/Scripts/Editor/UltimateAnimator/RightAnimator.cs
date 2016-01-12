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

	//Каждый из векторов обеспечивает независимость трёх списков со слайдером правого редактора
	private Vector2 scrollPosition=Vector2.zero;
	private Vector2 scrollPosition1=Vector2.zero;
	private Vector2 scrollPosition2=Vector2.zero;

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
		CharactersList();/*
		if (character != null) {
			PartList ();
			AnimationList ();
		}
		*/
	}

	//Вывести список анимируемых частей, из которых состоит персонаж
	void PartList() 
	{
		parts = character.GetComponent<CharacterAnimator> ().parts;
		GUILayout.BeginVertical ();
		{
			scrollPosition1=GUI.BeginScrollView(new Rect(0f,140f,300f,100f),scrollPosition1,new Rect(0,20,300,400));
			{
				for (int i = 0; i<parts.Count; i++) {
					if (GUILayout.Button (parts [i].gameObject.name)) {
						if (!string.Equals (parts [i].gameObject.name, leftAnim.partName)) {
							leftAnim.partName = parts [i].gameObject.name;
							leftAnim.characterPart = parts [i];
						}
					}
				}
			}
			GUI.EndScrollView();	
		}
		GUILayout.EndVertical();
		GUILayout.BeginArea(new Rect(0f,260f,300f,15f));
		{
			if(GUILayout.Button("+Add"))
			{
				AddPart ();
			}
		}
		GUILayout.EndArea();
		GUILayout.Space (10);
	}

	//Вывести список всех анимаций данного персонажа
	void AnimationList () 
	{
		
		GUILayout.BeginVertical ();
		{
			GUILayout.TextField("Animations");
			GUILayout.Space (5);
			animTypes = character.GetComponent<CharacterAnimator> ().animTypes;
			scrollPosition2=GUI.BeginScrollView(new Rect(0f,280f,300f,100f),scrollPosition2,new Rect(0,40,300,800));
			{
				for (int i = 0; i < animTypes.Count; i++) {
					GUILayout.TextField (animTypes [i].typeName);
					for (int j = 0; j < animTypes [i].animations.Count; j++) {
						if (GUILayout.Button (animTypes [i].animations [j])) {
							if (!string.Equals (animTypes [i].animations [j], leftAnim.animationName)&& (!string.Equals (leftAnim.partName, "Part"))) {
								leftAnim.animationName = animTypes [i].animations [j];
								leftAnim.characterAnimation = leftAnim.characterPart.interp.animTypes [i].animInfo [j];
							}
						}
					}
				}
			}
			GUILayout.EndScrollView ();
		}
		GUILayout.EndVertical();
		GUILayout.Space (5);
		GUILayout.BeginArea (new Rect(0f,400f,300f,15f));
		{
			if (GUILayout.Button ("+Add")) {
				AddAnimation ();
			}
		}
		GUILayout.EndArea ();	
	}

	//Вывести список персонажей, с которыми мы работаем в данный момент
	void CharactersList () 
	{
		
		aCharacters = animScene.animBase.usedCharacters;
		scrollPosition=GUI.BeginScrollView(new Rect(0f,0f,300f,100f),scrollPosition,new Rect(0,0,300,400));
		{
			for (int i = 0; i < aCharacters.Count; i++) {
				if (GUILayout.Button (aCharacters [i])) {
					if (!string.Equals (aCharacters [i], leftAnim.characterName)) {
						//SaveAndCreate (aCharacters [i], "", animScene.FindData (aCharacters [i] + ".asset"));
					}
				}
			}
		}
		GUI.EndScrollView();	
		GUILayout.BeginArea (new Rect(0f,110f,300f,15f));
		{
			if (GUILayout.Button ("+Add"))
				AddCharacter ();
		}
		GUILayout.EndArea ();
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

	//Комментарий для нормальной работы гит хаба.
}
