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
	public Object movPart;//GAF анимация, которая поставиться в поле mov экземпляра класса PartController
    public int currentIndex=0;//Вспомогательное число
    public bool setDepended = false;//Ставить ли создаваемую часть в зависимость от уже существующей части?

	public string name;//Имя части
	public string partPath;//Путь, в который добавится сама часть тела и её интерпретатор анимаций

	//Инициализация
	public void Initialize (RightAnimator ra, LeftAnimator la, GameObject c, string pp)
	{
		rightAnim = ra; 
		leftAnim = la;
		character = c;
		partPath = pp;
	}

	//Что отображается в окне
	void OnGUI()
	{
		character = leftAnim.character;
		name = EditorGUILayout.TextField (name);
		partPath = EditorGUILayout.TextField (partPath);
		movPart=EditorGUILayout.ObjectField (movPart, typeof(Object), true);
        List<string> partNames = character.GetComponent<InterObjAnimator>().parts.ConvertAll(_part => _part.gameObject.name);
        setDepended = EditorGUILayout.Toggle(setDepended);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("Parent part");
            currentIndex = EditorGUILayout.Popup(currentIndex, partNames.ToArray());
        }
        EditorGUILayout.EndHorizontal();
		if (GUILayout.Button ("Create New Part"))
		{
			CreateNewPart ();
		}
        if (GUILayout.Button("Add Created Part"))
        {
            AddPart();
        }
		EditorGUILayout.Space ();
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
        part.transform.localScale = new Vector3(part.transform.localScale.x * SpFunctions.realSign(character.transform.localScale.x),
                                              part.transform.localScale.y,
                                              part.transform.localScale.z);
		AnimationInterpretator asset = ScriptableObject.CreateInstance<AnimationInterpretator>();
		asset.partPath = partPath;
		asset.animTypes = new List<animationInfoTypes> ();
		AssetDatabase.CreateAsset (asset, partPath + name + ".asset");
		PartController cPart = part.AddComponent<PartController> ();
		cPart.interp=new AnimationInterpretator(asset.partPath);
		cPart.mov = partMov.GetComponent<GAF.Core.GAFMovieClip> ();
        cPart.interp = new AnimationInterpretator(asset);
        cPart.path = partPath + name + ".asset";
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator> ();
        if (cAnim.parts.Count > 0)
        {
            cPart.interp.setInterp(cAnim.parts[0].interp);
        }
        else {
            cPart.interp.setInterp(cAnim.animTypes);
        }
        cAnim.parts.Add (part.GetComponent<PartController>());
		GameObject asset1 = part;
		asset1=PrefabUtility.CreatePrefab(partPath + name + ".prefab",asset1);
		AssetDatabase.SaveAssets ();
        if (setDepended)
        {
            AddDependedPart(cPart, character.GetComponent<InterObjAnimator>().parts[currentIndex]);
        }
    }

	//Добавление уже созданной части в базу визуальных данных персонажа. 
	void AddPart()
	{
		GameObject part=Instantiate((GameObject)movPart)as GameObject;
		part.transform.position =  new Vector3(character.transform.position.x+xOffset,
			character.transform.position.y+yOffset,
			character.transform.position.z);
		part.transform.parent = character.transform;
        part.transform.localScale = new Vector3(part.transform.localScale.x * SpFunctions.realSign(character.transform.localScale.x),
                                                part.transform.localScale.y,
                                                part.transform.localScale.z);
        part.name = name;
		AnimationInterpretator asset = ScriptableObject.CreateInstance<AnimationInterpretator>();
		asset.partPath = partPath;
		asset.animTypes = new List<animationInfoTypes> ();
		PartController cPart = part.GetComponent<PartController> ();
		cPart.interp=new AnimationInterpretator(asset.partPath);
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator> ();
		cAnim.parts.Add (part.GetComponent<PartController>());
		cPart.interp = new AnimationInterpretator (asset);
        cPart.path = partPath + name + ".asset";
        if (character.GetComponent<InterObjAnimator> ().parts.Count > 0) {
			cPart.interp.setInterp (character.GetComponent<InterObjAnimator> ().parts [0].interp);
		}
        GameObject asset1 = part;
        asset1 = PrefabUtility.CreatePrefab(partPath + name + ".prefab", asset1);
        AssetDatabase.SaveAssets();
        if (setDepended)
        {
            AddDependedPart(cPart, character.GetComponent<InterObjAnimator>().parts[currentIndex]);
        }
	}

    /// <summary>
    ///Функция, добавляющая новую часть, а также связывает её с родительской частью. 
    /// </summary>
    /// <param name="часть-родитель"></param>
    void AddDependedPart(PartController part, PartController parentPart)
    {
        if (parentPart.childParts == null)
            parentPart.childParts = new List<PartController>();
        parentPart.childParts.Add(part);
    }

}
