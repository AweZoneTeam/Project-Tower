using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;


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

    //Данные, что используются для создания шаблонных аниматоров. В данном окне использование шаблона подразумевает собой использование шаблонного аниматора и используемых им анимаций.
    private string stencilPath = "Assets/Animations/Stencils/";//В этом пути находятся шаблоны, уже созданные объекты, используемые для быстрого старта создания нового аниматора.
    public List<string> stencils=new List<string>();
    public string chosenStencil = "";
    private int stencilIndex = 0;

    //Инициализация
    public void Initialize(AnimationEditorData aed, LeftAnimator la,List<string> ac, GameObject c)
	{
		animScene = aed;
		leftAnim = la;
		aCharacters = ac;
		character = c;
        stencils.Clear();
        DirectoryInfo dInfo = new DirectoryInfo(stencilPath);
        FileInfo[] fList = dInfo.GetFiles();
        for (int i = 0; i < fList.Length; i++)
        {
            if (!fList[i].Name.EndsWith("meta"))
            {
                stencils.Add(fList[i].Name);
            }
        }
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
	//Список выводится в виде кнопок, нажав которые можно выбрать часть тела, с которой пользователь желает работать
	//Также можно добавить новую анимационную часть
	void PartList() 
	{
        AnimClass anim=null;
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
		parts = character.GetComponent<InterObjAnimator> ().parts;
		GUILayout.BeginVertical ();
		{
			scrollPosition1=GUI.BeginScrollView(new Rect(0f,140f,300f,100f),scrollPosition1,new Rect(0,20,300,400));
			{
				for (int i = 0; i<parts.Count; i++) {
					if (GUILayout.Button (parts [i].gameObject.name)) {
						if (!string.Equals (parts [i].gameObject.name, leftAnim.partName)) {
							leftAnim.partName = parts [i].gameObject.name;
							leftAnim.characterPart = parts [i];
                            if (leftAnim.characterAnimation!=null)
                            {
                                anim = cAnim.FindAnimData(leftAnim.animationName);
                                if (anim!=null)
                                leftAnim.characterAnimation = parts[i].interp.animTypes[anim.type].animInfo[anim.numb];           
                            }
                            leftAnim.defaultLayer = parts[i].mov.settings.spriteLayerValue;

                        }
					}
				}
			}
			GUI.EndScrollView();	
		}
		GUILayout.EndVertical();
		GUILayout.BeginArea(new Rect(0f,260f,300f,60f));
		{
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("+Add"))
                {
                    AddPart();
                }
                if (GUILayout.Button("-Delete"))
                {
                    DeletePart();
                }
            }
            EditorGUILayout.EndHorizontal();
            if (stencils.Count > 0)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.TextField("Stencil");
                    stencilIndex = EditorGUILayout.Popup(stencilIndex, stencils.ToArray());
                    chosenStencil = stencils[stencilIndex];
                }
                if (GUILayout.Button("Use Stencil"))
                {
                    VisualData stencilVis= AssetDatabase.LoadAssetAtPath(stencilPath + chosenStencil, typeof(VisualData)) as VisualData;
                    InterObjAnimator sAnim = stencilVis.visual.GetComponent<InterObjAnimator>();
                    UseStencil(sAnim);
                }
            }
		}
		GUILayout.EndArea();
		GUILayout.Space (10);
	}

	//Вывести список всех анимаций данного персонажа. 
	//Анимации выводятся в виде кнопок, нажав которые, игрок выберет анимацию над которой он хочет работать
	//Также можно добавить новую анимацию
	void AnimationList () 
	{
		
		GUILayout.BeginVertical ();
		{
			GUILayout.Space (5);
			animTypes = character.GetComponent<InterObjAnimator> ().animTypes;
			scrollPosition2=GUI.BeginScrollView(new Rect(0f,340f,300f,100f),scrollPosition2,new Rect(0f,50f,300f,2000f));
			{
				for (int i = 0; i < animTypes.Count; i++) {
					GUILayout.TextField (animTypes [i].typeName);
					for (int j = 0; j < animTypes [i].animations.Count; j++) {
						if (GUILayout.Button (animTypes [i].animations [j])) {
							if (!string.Equals (animTypes [i].animations [j], leftAnim.animationName)&& (!string.Equals (leftAnim.partName, "Part"))) {
								leftAnim.animationName = animTypes [i].animations [j];
								leftAnim.characterAnimation = leftAnim.characterPart.interp.animTypes [i].animInfo [j];
                                character.GetComponent<InterObjAnimator>().setPartAnimations(i, j,true);
                                leftAnim.currentSequence = leftAnim.characterPart.mov.currentSequence.name;
							}
						}
					}
				}
			}
			GUI.EndScrollView ();
		}
		GUILayout.EndVertical();
		GUILayout.Space (5);
		GUILayout.BeginArea (new Rect(0f,450f,300f,20f));
		{
            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("+Add"))
                {
                    AddAnimation();
                }
                if (GUILayout.Button("-Delete"))
                {
                    DeleteAnimation();
                }
            }
            EditorGUILayout.EndHorizontal();
		}
		GUILayout.EndArea ();	
	}

	//Вывести список персонажей, с которыми мы работаем в данный момент
	void CharactersList () 
	{
        character = leftAnim.character;
		aCharacters = animScene.animBase.usedCharacters;
		scrollPosition=GUI.BeginScrollView(new Rect(0f,0f,300f,100f),scrollPosition,new Rect(0,0,300,400));
		{
			for (int i = 0; i < aCharacters.Count; i++) {
				if (GUILayout.Button (aCharacters [i])) {
					if (!string.Equals (aCharacters [i], leftAnim.characterName)) {
						SaveAndCreate (aCharacters [i], "", animScene.FindData (aCharacters [i] + ".asset"),"");
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

    /// <summary>
    /// Сохранить произведённые над персонажем изменения и, если указано, создать нового
    /// </summary>
    /// <param name="Имя персонажа"></param>
    /// <param name="Путь, по которому сохраняем персонажа"></param>
    /// <param name="Визуальные данные персонажа, с которым мы работаем"></param>
    /// <param name="Путь, ведущий к шаблону"></param>
	public void SaveAndCreate(string characterName, string path, VisualData characterData, string stencilName)
	{
        if (leftAnim.saved)
        {
            if (string.Equals(stencilName, ""))
            {
                leftAnim.CreateNewInstance(characterName, path, characterData);
            }
            else
            {      
                VisualData stencilVis = AssetDatabase.LoadAssetAtPath(stencilName, typeof(VisualData)) as VisualData;
                leftAnim.CreateNewInstance(characterName, path, characterData, stencilVis);
            }
        }
        else
        {
            SaveWindow saveScreen = (SaveWindow)EditorWindow.GetWindow(typeof(SaveWindow));
            saveScreen.leftAnim = leftAnim;
            saveScreen.characterName = characterName;
            saveScreen.characterData = characterData;
        }
	}

	public void AddPart()//Открыть окно добавления новой части персонажу
	{
		AddPartWindow animScreen = (AddPartWindow)EditorWindow.GetWindow (typeof(AddPartWindow));
		animScreen.Initialize(this,leftAnim,character,"Assets/Animations/");
	}

    /// <summary>
    /// Открыть окно удаления анимационной части
    /// </summary>
    public void DeletePart()
    {
        DeletePartWindow animScreen = (DeletePartWindow)EditorWindow.GetWindow(typeof(DeletePartWindow));
        animScreen.Initialize(character);
    }

    void UseStencil(InterObjAnimator stencil)
    {
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        cAnim.animTypes.Clear();
        for (int i = 0; i < stencil.animTypes.Count; i++)
        {
            cAnim.animTypes.Add(new animList(stencil.animTypes[i].typeName));
            for (int j = 0; j < stencil.animTypes[i].animations.Count; j++)
            {
                cAnim.animTypes[i].animations.Add(stencil.animTypes[i].animations[j]);
            }
        }
        cAnim.animBase.Clear();
        for (int i = 0; i < stencil.animBase.Count; i++)
        {
            cAnim.animBase.Add(new NamedAnimClass(stencil.animBase[i]));
        }
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            cAnim.parts[i].interp.animTypes.Clear();
            cAnim.parts[i].interp.setInterp(cAnim.animTypes);
        }
    }

	public void AddAnimation()//Открыть окно добавления анимации
	{
		AddAnimationWindow animScreen = (AddAnimationWindow)EditorWindow.GetWindow (typeof(AddAnimationWindow));
		animScreen.Initialize (this,leftAnim,character);
	}

    /// <summary>
    /// Открыть окно удаления анимации
    /// </summary>
    public void DeleteAnimation()
    {
        DeleteAnimationWindow animScreen = (DeleteAnimationWindow)EditorWindow.GetWindow(typeof(DeleteAnimationWindow));
        animScreen.Initialize(character);
    }
}
