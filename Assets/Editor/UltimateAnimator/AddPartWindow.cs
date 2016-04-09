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
    public int currentIndex=0, stencilPartIndex = 0;//Вспомогательные числа
    public bool setDepended = false;//Ставить ли создаваемую часть в зависимость от уже существующей части?

	public string partName;//Имя части
    private string partType;//Тип части
	public string partPath;//Путь, в который добавится сама часть тела и её интерпретатор анимаций

    private int pathIndex;
    private bool usePartAsStencil = false;
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
        string s;
		character = leftAnim.character;
		partName = EditorGUILayout.TextField (partName);
        partType = EditorGUILayout.TextField(partType);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.LabelField("partPath");
            pathIndex=EditorGUILayout.Popup(pathIndex, AssetDatabase.GetAllAssetPaths());
            s = AssetDatabase.GetAllAssetPaths()[pathIndex];
            if (s.Contains("."))
            {
                partPath = s.Substring(0, s.LastIndexOf("/") + 1);
            }
            else
            {
                partPath = s+"/";
            }
        }
        EditorGUILayout.EndHorizontal();
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
        EditorGUILayout.BeginHorizontal();
        {
            stencilPartIndex = EditorGUILayout.Popup(stencilPartIndex, partNames.ToArray());
            EditorGUILayout.LabelField("Use This Part as a Stencil");
            usePartAsStencil = EditorGUILayout.Toggle(usePartAsStencil);
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Add Created Part"))
        {
            AddPart();
        }
        if (setDepended)
        {
            if (GUILayout.Button("Correct Depended Part"))
            {
                CorrectDependedPart();
            }
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
        partMov.transform.localScale =new Vector3(partMov.transform.localScale.x, partMov.transform.localScale.y,0f);
        partMov.transform.position = new Vector3(character.transform.position.x+xOffset,
												 character.transform.position.y+yOffset,
												 character.transform.position.z);
		GameObject part = new GameObject(partName);
		part.transform.position =  new Vector3(character.transform.position.x+xOffset,
			character.transform.position.y+yOffset,
			character.transform.position.z);
		partMov.transform.parent = part.transform;
		part.transform.parent = character.transform;
        part.transform.localScale = new Vector3(part.transform.localScale.x * SpFunctions.RealSign(character.transform.localScale.x),
                                              part.transform.localScale.y,
                                              part.transform.localScale.z);
		AnimationInterpretator asset = ScriptableObject.CreateInstance<AnimationInterpretator>();
		asset.partPath = partPath;
		asset.animTypes = new List<animationInfoType> ();
		AssetDatabase.CreateAsset (asset, partPath + partName + ".asset");
		PartController cPart = part.AddComponent<PartController> ();
        cPart.partType = partType;
        cPart.childParts = new List<PartController>();
		cPart.interp=new AnimationInterpretator(asset.partPath);
		cPart.mov = partMov.GetComponent<GAF.Core.GAFMovieClip> ();
        cPart.interp = new AnimationInterpretator(asset);
        cPart.path = partPath + partName + ".asset";
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator> ();
        if ((character.GetComponent<InterObjAnimator>().parts.Count > 0)&& (usePartAsStencil))
        {
            cPart.interp.setInterp(cAnim.parts[stencilPartIndex].interp);   
        }
        else
        {
            cPart.interp.setInterp(cAnim.animTypes);
        }
        cAnim.parts.Add (part.GetComponent<PartController>());
		GameObject asset1 = part;
		asset1=PrefabUtility.CreatePrefab(partPath + partName + ".prefab",asset1);
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
        part.transform.localScale = new Vector3(part.transform.localScale.x * SpFunctions.RealSign(character.transform.localScale.x),
                                                part.transform.localScale.y,
                                                part.transform.localScale.z);
        part.name = partName;
		AnimationInterpretator asset = ScriptableObject.CreateInstance<AnimationInterpretator>();
		PartController cPart = part.GetComponent<PartController> ();
        asset.setInterp(AnimationInterpretator.FindInterp(cPart.path));
        asset.partPath = partPath;
        AssetDatabase.CreateAsset(asset, partPath + partName + ".asset");
        cPart.interp=new AnimationInterpretator(asset.partPath);
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator> ();
		cAnim.parts.Add (part.GetComponent<PartController>());
		cPart.interp = new AnimationInterpretator (asset);
        cPart.path = partPath + partName + ".asset";
        if (character.GetComponent<InterObjAnimator> ().parts.Count > 0)
        {
            if (usePartAsStencil)
            {
                cPart.interp.setInterp(cAnim.parts[stencilPartIndex].interp);
            }   
		}
        GameObject asset1 = part;
        asset1 = PrefabUtility.CreatePrefab(partPath + partName + ".prefab", asset1);
        AssetDatabase.SaveAssets();
        if (setDepended)
        {
            AddDependedPart(cPart, character.GetComponent<InterObjAnimator>().parts[currentIndex]);
        }
	}

    /// <summary
    ///Функция, добавляющая новую часть, а также связывает её с родительской частью. 
    /// </summary>
    /// <param name="часть-родитель"></param>
    void AddDependedPart(PartController part, PartController parentPart)
    {
        if (parentPart.childParts == null)
            parentPart.childParts = new List<PartController>();
        parentPart.childParts.Add(part);
        leftAnim.parentPart = parentPart;
    }

    /// <summary>
    /// Добавить персонажу ранее созданную часть тела и проделать в ней необходимые изменения
    /// </summary>
    void CorrectDependedPart()
    {
        GameObject part = Instantiate((GameObject)movPart) as GameObject;
        part.name = movPart.name;
        part.transform.position = new Vector3(character.transform.position.x + xOffset,
            character.transform.position.y + yOffset,
            character.transform.position.z);
        part.transform.parent = character.transform;
        part.transform.localScale = new Vector3(part.transform.localScale.x * SpFunctions.RealSign(character.transform.localScale.x),
                                                part.transform.localScale.y,
                                                part.transform.localScale.z);
        PartController cPart = part.GetComponent<PartController>();
        cPart.interp = new AnimationInterpretator(AssetDatabase.LoadAssetAtPath(cPart.path, typeof(AnimationInterpretator)) as AnimationInterpretator); 
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        cAnim.parts.Add(part.GetComponent<PartController>());
        if (cAnim.parts.Count > 0)
        {
            AnimationInterpretator interp0;
            if (usePartAsStencil)
            {
                interp0 = cAnim.parts[stencilPartIndex].interp;
            }
            else
            {
                interp0 = cAnim.parts[0].interp;
            }
            AnimationInterpretator interp1 = cPart.interp;
            int rOrder = 0, lOrder = 0;
            if (interp1.animTypes.Count > 0)
            {
                if (interp1.animTypes[0].animInfo.Count > 0)
                {
                    if (interp1.animTypes[0].animInfo[0].leftOrderData.Count > 0)
                    {
                        lOrder = interp1.animTypes[0].animInfo[0].leftOrderData[0].order;
                    }
                    if (interp1.animTypes[0].animInfo[0].rightOrderData.Count > 0)
                    {
                        rOrder = interp1.animTypes[0].animInfo[0].rightOrderData[0].order;
                    }
                }
            }
            //удалим все ненужные типы
            for (int i = 0; i < interp1.animTypes.Count; i++)
            {
                bool k = false;
                for (int j = 0; j < interp0.animTypes.Count; j++)
                {
                    if (string.Equals(interp1.animTypes[i].typeName, interp0.animTypes[j].typeName))
                    {
                        k = true;
                        break;
                    }
                }
                if (!k)
                {
                    interp1.animTypes.RemoveAt(i);
                    i--;
                }
            }
            //Теперь пройдёмся по всем типам оригинала и будем добавлять и корректировать типы в зависимой части при надобности
            for (int i = 0; i < interp0.animTypes.Count; i++)
            {
                if (i >= interp1.animTypes.Count)
                {
                    interp1.animTypes.Insert(i, new animationInfoType(interp0.animTypes[i]));
                }
                else if (!string.Equals(interp0.animTypes[i].typeName, interp1.animTypes[i].typeName))
                {
                    interp1.animTypes.Insert(i, new animationInfoType(interp0.animTypes[i]));
                }
                else
                {
                    //удалим все ненужные анимации
                    for (int j = 0; j < interp1.animTypes[i].animInfo.Count; j++)
                    {
                        bool k = false;
                        for (int l = 0; l < interp0.animTypes[i].animInfo.Count; l++)
                        {
                            if (string.Equals(interp1.animTypes[i].animInfo[j].animName, interp0.animTypes[i].animInfo[l].animName))
                            {
                                k = true;
                                break;
                            }
                        }
                        if (!k)
                        {
                            interp1.animTypes[i].animInfo.RemoveAt(j);
                            j--;
                        }
                    }
                    //Теперь пройдёмся по всем анимациям оригинала и будем добавлять и корректировать анимации в зависимой части при надобности
                    for (int j = 0; j < interp0.animTypes[i].animInfo.Count; j++)
                    {
                        if (j >= interp1.animTypes[i].animInfo.Count)
                        {
                            interp1.animTypes[i].animInfo.Insert(j, new animationInfo(interp0.animTypes[i].animInfo[j], rOrder, lOrder));
                        }
                        else if (!string.Equals(interp0.animTypes[i].animInfo[j].animName, interp1.animTypes[i].animInfo[j].animName))
                        {
                            interp1.animTypes[i].animInfo.Insert(j, new animationInfo(interp0.animTypes[i].animInfo[j], rOrder, lOrder));
                        }
                    }
                    //Удалим все оставшиеся анимации
                    if (interp0.animTypes[i].animInfo.Count < interp1.animTypes[i].animInfo.Count)
                    {
                        for (int j = interp1.animTypes[i].animInfo.Count - 1; j >= interp0.animTypes[i].animInfo.Count; j--)
                        {
                            interp1.animTypes[i].animInfo.RemoveAt(j);
                        }
                    }
                }
            }
            //Удалим оставшиеся типы
            if (interp0.animTypes.Count < interp1.animTypes.Count)
            {
                for (int i = interp1.animTypes.Count - 1; i >= interp0.animTypes.Count; i--)
                {
                    interp1.animTypes.RemoveAt(i);
                }
            }           
        }
        AddDependedPart(cPart, character.GetComponent<InterObjAnimator>().parts[currentIndex]);
    }

}
