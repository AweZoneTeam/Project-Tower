using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//Окно, в котором происходит создание новой части для заданного персонажа и его аниматора
public class AddPartWindow : EditorWindow
{
	//Константы, которые говорят, в какой должен быть transform у созданных аниматором частей
	const float animScale = 0.0223928f;
	const float xOffset = -5.5f;
	const float yOffset = 4.51f;

	public RightAnimator rightAnim;
	public LeftAnimator leftAnim;
	public GameObject character;
	public Object movPart;

	public string name;//Имя части
	public string movPath;//Путь, по которому надо искать начинку - саму анимацию
	public string partPath;//Путь, в который добавится сама часть тела и её интерпретатор анимаций

	//Инициализация
	public void Initialize (RightAnimator ra, LeftAnimator la, GameObject c, string mp, string pp)
	{
		rightAnim = ra; 
		leftAnim = la;
		character = c;
		movPath = mp;
		partPath = pp;
	}

	//Что отображается в окне
	void OnGUI()
	{
		character = leftAnim.character;
		name = EditorGUILayout.TextField (name);
		movPath = EditorGUILayout.TextField (movPath);
		partPath = EditorGUILayout.TextField (partPath);
		movPart=EditorGUILayout.ObjectField (movPart, typeof(Object), true);
		if (GUILayout.Button ("Create New Part"))
		{
			CreateNewPart ();
		}
		EditorGUILayout.Space ();
		if (GUILayout.Button ("Add Created Part"))
		{
			AddPart ();
		}
	}

	//Создание новой части и добавление её в базу визуальных данных персонажа, 
	//а также создание ассетов и сохранение их в базе данных игры
	void CreateNewPart()
	{
		GameObject partMov;
		partMov = Instantiate ((GameObject)movPart) as GameObject;
		partMov.transform.localScale *= animScale;
		partMov.transform.position = new Vector3(character.transform.position.x+xOffset,
												 character.transform.position.y+yOffset,
												 character.transform.position.z);
		GameObject part = new GameObject(name);
		part.transform.position =  new Vector3(character.transform.position.x+xOffset,
			character.transform.position.y+yOffset,
			character.transform.position.z);
		partMov.transform.parent = part.transform;
		part.transform.parent = character.transform;
		AnimationInterpretator asset = ScriptableObject.CreateInstance<AnimationInterpretator>();
		asset.partPath = partPath;
		asset.animTypes = new List<animationInfoTypes> ();
		AssetDatabase.CreateAsset (asset, partPath + name + ".asset");
		PartController cPart = part.AddComponent<PartController> ();
		cPart.interp=new AnimationInterpretator(asset.partPath);
		cPart.mov = partMov.GetComponent<GAF.Core.GAFMovieClip> ();
		CharacterAnimator cAnim = character.GetComponent<CharacterAnimator> ();
		cAnim.parts.Add (part.GetComponent<PartController>());
		GameObject asset1 = part;
		asset1=PrefabUtility.CreatePrefab(partPath + name + ".prefab",asset1);
		AssetDatabase.SaveAssets ();
		cPart.interp = new AnimationInterpretator (asset);
		if (character.GetComponent<CharacterAnimator> ().parts.Count > 0) {
			cPart.interp.setInterp (character.GetComponent<CharacterAnimator> ().parts [0].interp);
		}
	}

	//Добавление уже созданной части в базу визуальных данных персонажа. Добавляемая часть должна находиться в указанном в movPart пути 
	void AddPart()
	{
		GameObject part=Instantiate((GameObject)movPart)as GameObject;
		part.transform.position =  new Vector3(character.transform.position.x+xOffset,
			character.transform.position.y+yOffset,
			character.transform.position.z);
		part.transform.parent = character.transform;
		part.name = name;
		AnimationInterpretator asset = ScriptableObject.CreateInstance<AnimationInterpretator>();
		asset.partPath = partPath;
		asset.animTypes = new List<animationInfoTypes> ();
		PartController cPart = part.GetComponent<PartController> ();
		cPart.interp=new AnimationInterpretator(asset.partPath);
		CharacterAnimator cAnim = character.GetComponent<CharacterAnimator> ();
		cAnim.parts.Add (part.GetComponent<PartController>());
		GameObject asset1 = part;
		asset1=PrefabUtility.CreatePrefab(partPath + name + ".prefab",asset1);
		AssetDatabase.SaveAssets ();
		Selection.activeObject = asset1;
		cPart.interp = new AnimationInterpretator (asset);
		if (character.GetComponent<CharacterAnimator> ().parts.Count > 0) {
			cPart.interp.setInterp (character.GetComponent<CharacterAnimator> ().parts [0].interp);
		}
	}

}
