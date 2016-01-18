using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using GAF.Core;

//Часть редактора анимаций, которая находится слева. Отвественная за обработку каждого кадра анимации
public class LeftAnimator : EditorWindow
{
	public Rect windowRect2 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect1 = new Rect(100f, 100f, 200f, 200f);
	public string orientation = "Left";
    public string leftSequence, rightSequence;//Последовательности, которые мы задаём для данной части тела в данной анимации
    public int rCurrentIndex=0, lCurrentIndex=0;//Числа, которые нужны для выбора той или иной последовательности из предложеных в GAFMovieClip
    public Object audioClip;//Один из аудиоклипов, проигрываемых в данной частью в данном фрейме данной анимации
	public int layer=0;//layer - порядок прорисовки анимационной части персонажа начиная с данного кадра данной анимации
    public int defaultLayer=0;//defaultLayer - порядок прорисовки анимационной части по дефолту, тот порядок, который используется обычно
	public bool loop=false;//сделать ли данную анимацию зацикленной?
    public int FPS = 30;//Сколько кадров в секунду проигрывать в анимации?
    public bool stepByStep;//Этими параметрами помечаются анимации, которые могут встать на паузу в любой момент
    public int numb = 0, type = 0;//Идентификационные номера анимации
    public int currentFrame;//С каким кадром анимации мы сейчас работаем
    public int mainFrame=0, prevMainFrame=0;//Какой кадр анимации должны иметь все остальные части тела
	public bool saved=true;//параметр, который говорит, были ли сохранены послежние изменениня или нет. Вернёт false, когда научишься отлеживать эти изменения
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
    public string currentSequence = "Default";
	public string savePath;//Путь, по которому будет сохраняться созданный персонаж 

	//Инициализация
	public void Initialize(RightAnimator ra, AnimationEditorData aed, GameObject c, bool s)
	{
		rightAnim = ra;
		animEditor = aed;
		character = c;
        characterPart = null;
        characterAnimation = null;
		saved = s;
	}

	//Что выводится
	void OnGUI () 
	{
		EditorGUILayout.BeginVertical ();
        {
            if (GUILayout.Button("Create New Character"))
                CreateNew();
            if (GUILayout.Button("Save Changes"))
                SaveChanges();
            EditorGUILayout.Space();
            EditorGUILayout.TextField(characterName);
            EditorGUILayout.TextField(partName);
            EditorGUILayout.TextField(animationName);
            EditorGUILayout.Space();
            if (characterPart != null) {
                 PartParamWindow();
            }
			EditorGUILayout.Space();
            if (characterAnimation != null)
            {
                AnimationParamWindow();
            }
			if(GUILayout.Button("Reverse"))
			{
				Debug.Log ("Reverse");
			}
		}
		EditorGUILayout.EndVertical ();
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
            EditorGUILayout.BeginHorizontal();
            {
                defaultLayer = EditorGUILayout.IntField("layer", defaultLayer);
                if (GUILayout.Button("Change Default Layer"))
                {
                    SetLayerInAllFrames(defaultLayer, characterPart);
                }
            }
            EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical ();
	}

	//Параметры анимаций
	void AnimationParamWindow () 
	{
        CharacterAnimator cAnim = character.GetComponent<CharacterAnimator>();
        EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Animation parametres");
            if (characterPart != null)
            {   
                GAFMovieClip mov = characterPart.mov;
                currentSequence = mov.sequenceIndexToName(mov.getCurrentSequenceIndex());
                AnimationInterpretator interp = characterPart.interp;
                List<string> sequenceNames = mov.asset.getSequences(mov.timelineID).ConvertAll(_sequence => _sequence.name);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Right Sequence");
                    rCurrentIndex = EditorGUILayout.Popup(rCurrentIndex, sequenceNames.ToArray());
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Left Sequence");
                    lCurrentIndex = EditorGUILayout.Popup(lCurrentIndex, sequenceNames.ToArray());
                }
                EditorGUILayout.EndHorizontal();
                leftSequence = sequenceNames[lCurrentIndex];
                rightSequence = sequenceNames[rCurrentIndex];
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(rightSequence);
                    if (GUILayout.Button("SetRightSequence"))
                    {
                        SetSequence(rightSequence, true);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(leftSequence);
                    if (GUILayout.Button("SetLeftSequence"))
                    {
                        SetSequence(leftSequence, false);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Current Sequence is "+currentSequence);
                EditorGUILayout.BeginHorizontal();
                {
                    layer = EditorGUILayout.IntField(layer);
                    if (GUILayout.Button("Set Layer"))
                    {
                        SetLayerInFrame(layer);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    FPS = EditorGUILayout.IntField(FPS);
                    if (GUILayout.Button("Set FPS"))
                    {
                        SetFPS(FPS);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    audioClip = EditorGUILayout.ObjectField(audioClip, typeof(AudioClip),true);
                    if (GUILayout.Button("Set Audio"))
                    {
                        SetSound((AudioClip)audioClip);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    loop = EditorGUILayout.Toggle("loop", loop);
                    if (GUILayout.Button("Set Loop"))
                    {
                        SetLoop(loop);
                    }
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    stepByStep = EditorGUILayout.Toggle("stepByStep", stepByStep);
                    if (GUILayout.Button("SetStepByStep"))
                    {
                        SetStepByStep(stepByStep);
                    }
                }
                EditorGUILayout.EndHorizontal();
                currentFrame =EditorGUILayout.IntSlider(currentFrame,
                                        (int)mov.currentSequence.startFrame,
                                        (int)mov.currentSequence.endFrame);
                mainFrame = EditorGUILayout.IntSlider(mainFrame,
                        (int)mov.currentSequence.startFrame,
                        (int)mov.currentSequence.endFrame);
                mov.gotoAndStop((uint)currentFrame);
                if (mainFrame != prevMainFrame)
                {
                    prevMainFrame = mainFrame;
                    currentFrame = mainFrame;
                    foreach (PartController part in cAnim.parts)
                    {
                        part.mov.gotoAndStop((uint)mainFrame);
                    }
                }
            }
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
        character = null;
        characterPart = null;
        characterAnimation = null;
        partName = "Part";
        animationName = "Animation";
		characterName=_name;
		if (asset.visual == null) {
			character = new GameObject (_name);
			character.transform.position = animEditor.gameObject.transform.position;
			CharacterAnimator cAnim = character.AddComponent<CharacterAnimator> ();
            cAnim.SetDefaultAnimator();
            cAnim.visualData = asset;
			asset.visual = PrefabUtility.CreatePrefab (path + _name + ".prefab", character);
			rightAnim.animTypes = cAnim.animTypes;
		}
		else {
			character = PrefabUtility.InstantiatePrefab (asset.visual) as GameObject;
			character.transform.position=animEditor.gameObject.transform.position;
			CharacterAnimator cAnim = character.GetComponent<CharacterAnimator> ();
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

    /// <summary>
    /// Функция, которая ставит в данном интерпретаторе в данном кадре на проигрывание данный звук
    /// </summary>
    /// <param name="аудиодорожка"></param>
    void SetSound(AudioClip _audio)
    {
        int k = -1;
        for (int i = 0; i < characterAnimation.soundData.Count; i++)
        {
            if (characterAnimation.soundData[i].time == currentFrame)
            {
                k = i;
            }
        }
        if (k == -1)
        {
            characterAnimation.soundData.Add(new animationSoundData(false, currentFrame));
            k = characterAnimation.soundData.Count - 1;
        }
        characterAnimation.soundData[k].audios.Add(_audio);
    }

    /// <summary>
    /// Функция, которая говорит, что в данном интерпретаторе в данной аниации при данной ориентации должна проигрывать данная последовательность
    /// </summary>
    /// <param name="последовательность"></param>
    /// <param name="в какую сторону должен быть повёрнут персонаж при проигрывании анимации"></param>
    void SetSequence(string sequence, bool right)
    {
        if (right)
        {
            characterAnimation.rsequence.sequence = sequence;
        }
        else
        {
            characterAnimation.lsequence.sequence = sequence;
        }
        if ((string.Equals(orientation, "right")) == right)
        {
            characterPart.mov.setSequence(sequence, true);
        }
    }

	//Функция, которая ставит в даннном кадре данной анимации часть тела на указанный слой
	void SetLayerInFrame(int _layer)
	{
        if (string.Equals(orientation, "right"))
        {
            characterAnimation.rightOrderData.Add(new animationLayerOrderData(currentFrame, _layer));
            characterAnimation.rightOrderData.Sort();
        }
        else
        {
            characterAnimation.leftOrderData.Add(new animationLayerOrderData(currentFrame, _layer));
            characterAnimation.leftOrderData.Sort();
        }
	}

    /// <summary>
    /// Функция, которая говорит аниматору, что во всех начальных кадрах всех анимаций часть 
    /// тела должна прорисовываться на каком-то дефолтном слое, и ставит её на этот слой
    /// </summary>
    /// <param name="дефолтный слой"></param>
    /// <param name="анимационная часть тела"></param>
    void SetLayerInAllFrames(int _layer, PartController part)
	{
        SpFunctions.ChangeRenderOrder(_layer, part.mov.gameObject);
        part.interp.SetDefaultLayer(_layer, string.Equals(orientation, "right"));
	}

    /// <summary>
    /// Функция, которая в данной анимации выставляет данное количество проигрываемых кадров в секунду
    /// </summary>
    /// <param name="Кадры в секунду"></param>
    void SetFPS(int _FPS)
    {
        CharacterAnimator cAnim = character.GetComponent<CharacterAnimator>();
        AnimClass anim = cAnim.animBase[animationName];
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            cAnim.parts[i].interp.animTypes[anim.type].animInfo[anim.numb].FPS = _FPS;
        }
    }

    /// <summary>
    /// Функция, для выставления параметра StepByStep или StopStepByStep
    /// </summary>
    /// <param name="stepByStep"></param>
    void SetStepByStep(bool _stepByStep)
    {
        CharacterAnimator cAnim = character.GetComponent<CharacterAnimator>();
        AnimClass anim = cAnim.animBase[animationName];
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            if (_stepByStep)
            {
                cAnim.parts[i].interp.animTypes[anim.type].animInfo[anim.numb].stepByStep = true;
            }
            else
            {
                cAnim.parts[i].interp.animTypes[anim.type].animInfo[anim.numb].stopStepByStep = true;
            }
        }
    }

    /// <summary>
    /// Функция, для выставления параметра Loop, причём это параметр проставляется сразу во всех частях
    /// </summary>
    /// <param name="loop"></param>
    void SetLoop(bool _loop)
    {
        CharacterAnimator cAnim = character.GetComponent<CharacterAnimator>();
        AnimClass anim = cAnim.animBase[animationName];
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            cAnim.parts[i].interp.animTypes[anim.type].animInfo[anim.numb].loop = _loop;
        }
    }
}
