using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//Часть редактора анимаций, которая находится слева. Отвественная за обработку каждого кадра анимации
public class LeftAnimator : EditorWindow
{
	public Rect windowRect2 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect1 = new Rect(100f, 100f, 200f, 200f);
	public string orientation = "Left";
	public int index=0;
	public bool depended=false;
	public int sound, layer=0;
	public bool loop=false;
	public int numb=0, type=0, number=0;
	public bool saved=true;//параметр, который говорит, были ли сохранены послежние изменениня или нет. Вернёть false, когда научишься отлеживать эти изменения
	public GameObject character;//Какой персонаж сейчас интересует левый редактор
	public PartController characterPart;//Какая часть тела в центре внимация всего редактора анимаций
	public animationInfo characterAnimation; // Какая анимация сейчас обрабаытвается редактором


	[HideInInspector]
	public RightAnimator rightAnim;
	public AnimationEditorData animEditor;

	//Строки, которые обозначают, с чем мы сейчас работаем
	public string characterName="Name";
	public string partName="Part";
	public string animationName="Animation";
	public string savePath;//Путь, по которому будет сохраняться созданный персонаж 

	//Инициализация
	public void Initialize(RightAnimator ra, AnimationEditorData aed, GameObject c, bool s)
	{
		rightAnim = ra;
		animEditor = aed;
		character = c;
		saved = s;
	}

	//Что выводится
	void OnGUI () 
	{
		EditorGUILayout.BeginVertical ();
		{
			if (GUILayout.Button ("Create New Character"))
				CreateNew();
			if (GUILayout.Button ("Save Changes"))
				SaveChanges ();
			EditorGUILayout.Space ();
			EditorGUILayout.TextField(characterName);
			EditorGUILayout.TextField("Animation Name");
			EditorGUILayout.Space();
			PartParamWindow();
			EditorGUILayout.Space();
			AnimationParamWindow();
			if(GUILayout.Button("Reverse"))
			{
				Debug.Log ("Reverse");
			}
		}
		EditorGUILayout.EndVertical ();
		if (GUI.changed) 
		{

		}
	}
		
	//Настраиваемые параметры частей
	void PartParamWindow() 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Part parametres");
			if(GUILayout.Button(orientation))
			{
				ChangeAnimaionOrientation();
			}
			depended=EditorGUILayout.Toggle("depend",depended);
			sound=EditorGUILayout.IntField("sound",sound);
			layer=EditorGUILayout.IntField("layer",layer);
		}
		EditorGUILayout.EndVertical ();
	}

	//Параметры анимаций
	void AnimationParamWindow () 
	{
		EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Animation parametres");
			loop=EditorGUILayout.Toggle("loop",loop);
			number=EditorGUILayout.IntField("number", number);
			type=EditorGUILayout.IntField("type", type);
			characterName=EditorGUILayout.TextField(characterName);
		}
		EditorGUILayout.EndVertical();

	}

	//Повернуть персонажа
	void ChangeAnimaionOrientation()
	{
		if (string.Equals(orientation,"Right"))
			orientation="Left";
		else 
			orientation="Right";
	}

	//Окно создания нового персонажа
	void CreateNew()
	{
		CreateNewAnimWindow animScreen=(CreateNewAnimWindow)EditorWindow.GetWindow(typeof(CreateNewAnimWindow));
		animScreen.rightAnim = rightAnim;
		animScreen.leftAnim = this;
	}

	//Создаём на сцене редактора экземпляр того персонажа, которого зададим
	public void CreateNewInstance(string _name, string path, VisualData asset)
	{
		rightAnim.parts.Clear();
		rightAnim.animTypes.Clear();
		if (!string.Equals (characterName, "Name")) {
			DestroyImmediate (GameObject.Find (characterName));
		}
		characterName=_name;
		if (asset.visual == null) {
			character = new GameObject (_name);
			character.transform.position = animEditor.gameObject.transform.position;
			CharacterAnimator cAnim = character.AddComponent<CharacterAnimator> ();
			cAnim.visualData = asset;
			cAnim.animTypes = new List<animList> ();
			asset.visual = PrefabUtility.CreatePrefab (path + _name + ".prefab", character);
			rightAnim.animTypes = cAnim.animTypes;
		}
		else {
			character = PrefabUtility.InstantiatePrefab (asset.visual) as GameObject;
			character.transform.position=animEditor.gameObject.transform.position;
			CharacterAnimator cAnim = character.AddComponent<CharacterAnimator> ();
			for (int i = 0; i < cAnim.parts.Count; i++) {
				cAnim.parts [i].interp = new AnimationInterpretator (" ");
				cAnim.parts [i].interp.setInterp(cAnim.visualData.animInterpretators [i]);
			}
			rightAnim.animTypes = cAnim.animTypes;			
		}
		rightAnim.character = character;
		//saved = false;
	}

	//Сохранить изменения, проделанные в ходе работы над персонажем
	//Пока что я не научил саму программу отслеживать эти изменения, поэтому надо не забывать нажимать SaveChange, если надо чтобы эти изменения сохранились
	public void SaveChanges()
	{
		AnimationInterpretator changeInterp;
		AnimationInterpretator saveInterp;
		CharacterAnimator cAnim = character.GetComponent<CharacterAnimator> ();
		cAnim.visualData.animInterpretators.Clear ();
		for (int i = 0; i < cAnim.parts.Count; i++) {
			saveInterp = cAnim.parts [i].interp;
			changeInterp = saveInterp.FindInterp (saveInterp.partPath + cAnim.parts [i].gameObject.name + ".asset");
			changeInterp.setInterp (saveInterp);
			cAnim.parts [i].interp = changeInterp;
			cAnim.visualData.animInterpretators.Add(changeInterp);
		}
		cAnim.visualData.visual=PrefabUtility.ReplacePrefab (character,cAnim.visualData.visual);
		saved = true;
	}
}
