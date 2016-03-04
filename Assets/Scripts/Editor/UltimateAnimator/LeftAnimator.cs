using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using GAF.Core;

//Часть редактора анимаций, которая находится слева. Отвественная за обработку каждого кадра анимации
public class LeftAnimator : EditorWindow
{
    #region fields
    public Rect windowRect2 = new Rect(100f, 100f, 200f, 200f);
	public Rect windowRect1 = new Rect(100f, 100f, 200f, 200f);
	public string orientation = "Right";
    public string leftSequence, rightSequence, leftPSequence, rightPSequence;//Последовательности, которые мы задаём для данной части 
                                                                           //тела в данной анимации
    public int rCurrentIndex=0, lCurrentIndex=0, rPCurrentIndex = 0, lPCurrentIndex = 0;//Числа, которые нужны для выбора той или иной 
                                                                                       //последовательности из предложеных в GAFMovieClip
    public Object audioClip;//Один из аудиоклипов, проигрываемых в данной частью в данном фрейме данной анимации
	public int layer=0;//layer - порядок прорисовки анимационной части персонажа начиная с данного кадра данной анимации
    public int defaultLayer=0;//defaultLayer - порядок прорисовки анимационной части по дефолту, тот порядок, который используется обычно
	public bool loop=false;//сделать ли данную анимацию зацикленной?
    public int FPS = 30;//Сколько кадров в секунду проигрывать в анимации?
    public bool stepByStep;//Этими параметрами помечаются анимации, которые могут встать на паузу в любой момент
    public int numb = 0, type = 0;//Идентификационные номера анимации
    public int currentFrame;//С каким кадром анимации мы сейчас работаем
    public int mainFrame=0, prevMainFrame=0;//Какой кадр анимации должны иметь все остальные части тела
	public bool saved=true;//параметр, который говорит, были ли сохранены послежние изменениня или нет. Вернёт false, когда научишься отслеживать эти изменения
	public GameObject character;//Какой персонаж сейчас интересует левый редактор
	public PartController characterPart;//Какая часть тела в центре внимация всего редактора анимаций
    public PartController parentPart;//Какая часть тела является родительской по отношению к characterPart
	public animationInfo characterAnimation; // Какая анимация сейчас обрабаытвается редактором
    public string animationTurnTo = "Play";//Строка, которая будет находится на кнопке, включающую/выключающую проигрывание анимации в редакторе

	[HideInInspector]
	public RightAnimator rightAnim;
    public AnimatorScreen animatorScreen;
	public AnimationEditorData animEditor;

	//Строки, которые обозначают, с чем мы сейчас работаем
	public string characterName="Name";
	public string partName="Part";
	public string animationName="Animation";
    public string currentSequence = "Default";
	public string savePath;//Путь, по которому будет сохраняться созданный персонаж 

    private string stencilPath = "Assets/Animations/Stencils/";//В этом пути находятся шаблоны, уже созданные объекты, используемые для быстрого старта создания нового аниматора
    
    #endregion //fields

    //Инициализация
    public void Initialize(RightAnimator ra, AnimatorScreen ans, AnimationEditorData aed, GameObject c, bool s)
	{
		rightAnim = ra;
		animEditor = aed;
		character = c;
        characterPart = null;
        characterAnimation = null;
        animatorScreen = ans;
		saved = s;
	}

	//Что выводится
	void OnGUI () 
	{
		EditorGUILayout.BeginVertical ();
        {
            if (GUILayout.Button("Focus"))
            {
                animatorScreen.FocusToPoint();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Create New Character"))
                CreateNew();
            if (GUILayout.Button("Save Changes"))
                SaveChanges();
            if (GUILayout.Button("Make a Stencil"))
            {
                if (character!=null)
                {
                    MakeStencil();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.TextField(characterName);
            EditorGUILayout.TextField(partName);
            EditorGUILayout.TextField(animationName);
            EditorGUILayout.Space();
            if (characterPart != null)
            {
                 PartParamWindow();
            }
			EditorGUILayout.Space();
            if (characterAnimation != null)
            {
                AnimationParamWindow();
            }
			if(GUILayout.Button("Reverse"))
			{
                if (character != null)
                {
                    character.GetComponent<InterObjAnimator>().Flip();
                    orientation = SpFunctions.realSign(character.transform.localScale.x) > 0 ? "Right" : "Left";
                }
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
            EditorGUILayout.Space();
            if (GUILayout.Button("Orientate Sequences"))
            {
                characterPart.interp.OrientateSequences();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Inverse Sequences"))
            {
                characterPart.interp.InverseSequeces();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("Add Reversed Sequences"))
            {
                characterPart.interp.AddOrientatedSequences();
            }

        }
		EditorGUILayout.EndVertical ();
	}

	//Параметры анимаций
	void AnimationParamWindow () 
	{
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        EditorGUILayout.BeginVertical ();
		{
			EditorGUILayout.TextField("Animation parametres");
            if (characterPart != null)
            {
                GAFMovieClip mov = characterPart.mov;
                currentSequence = mov.sequenceIndexToName(mov.getCurrentSequenceIndex());
                AnimationInterpretator interp = characterPart.interp;
                List<string> sequenceNames = mov.asset.getSequences(mov.timelineID).ConvertAll(_sequence => _sequence.name);
                
                #region setSequences
                #region setRightSequence
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Right Sequence");
                    rCurrentIndex = EditorGUILayout.Popup(rCurrentIndex, sequenceNames.ToArray());
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    rightSequence = sequenceNames[rCurrentIndex];
                    EditorGUILayout.LabelField(rightSequence);
                    if (GUILayout.Button("SetRightSequence"))
                    {
                        SetSequence(rightSequence, true, false);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion //SetRightSequence

                #region setLeftSequence
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField("Left Sequence");
                    lCurrentIndex = EditorGUILayout.Popup(lCurrentIndex, sequenceNames.ToArray());
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                {
                    leftSequence = sequenceNames[lCurrentIndex];
                    EditorGUILayout.LabelField(leftSequence);
                    if (GUILayout.Button("SetLeftSequence"))
                    {
                        SetSequence(leftSequence, false,false);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion //setLeftSequence
                #endregion //SetSequences

                #region SetParentSequences
                if (parentPart != null)
                {
                    GAFMovieClip pMov = parentPart.mov ;
                    List<string> pSequenceNames = mov.asset.getSequences(mov.timelineID).ConvertAll(_sequence => _sequence.name);
                    #region setRightPSequence
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Right Parent Sequence");
                        rPCurrentIndex = EditorGUILayout.Popup(rCurrentIndex, pSequenceNames.ToArray());
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        rightPSequence = pSequenceNames[rPCurrentIndex];
                        EditorGUILayout.LabelField(rightPSequence);
                        if (GUILayout.Button("SetRightPSequence"))
                        {
                            SetSequence(rightPSequence, true,true);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion //setRightPSequence

                    #region setLeftPSequence
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("Left Parent Sequence");
                        lPCurrentIndex = EditorGUILayout.Popup(lCurrentIndex, pSequenceNames.ToArray());
                    }
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.BeginHorizontal();
                    {
                        leftPSequence = pSequenceNames[lPCurrentIndex];
                        EditorGUILayout.LabelField(leftPSequence);
                        if (GUILayout.Button("SetLeftPSequence"))
                        {
                            SetSequence(leftPSequence, false, true);
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    #endregion //setLeftPSequence
                }
                #endregion //SetParentSequences

                EditorGUILayout.LabelField("Current Sequence is "+currentSequence);

                #region setLayer
                EditorGUILayout.BeginHorizontal();
                {
                    layer = EditorGUILayout.IntField(layer);
                    if (GUILayout.Button("Set Layer"))
                    {
                        SetLayerInFrame(layer);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion //setLayer

                #region setFPS
                EditorGUILayout.BeginHorizontal();
                {
                    FPS = EditorGUILayout.IntField(FPS);
                    if (GUILayout.Button("Set FPS"))
                    {
                        SetFPS(FPS);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion //setFPS

                #region setAudio
                EditorGUILayout.BeginHorizontal();
                {
                    audioClip = EditorGUILayout.ObjectField(audioClip, typeof(AudioClip),true);
                    if (GUILayout.Button("Set Audio"))
                    {
                        SetSound((AudioClip)audioClip);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion //setAudio

                #region setLoop
                EditorGUILayout.BeginHorizontal();
                {
                    loop = EditorGUILayout.Toggle("loop", loop);
                    if (GUILayout.Button("Set Loop"))
                    {
                        SetLoop(loop);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion //loop

                #region setStepByStep
                EditorGUILayout.BeginHorizontal();
                {
                    stepByStep = EditorGUILayout.Toggle("stepByStep", stepByStep);
                    if (GUILayout.Button("SetStepByStep"))
                    {
                        SetStepByStep(stepByStep);
                    }
                }
                EditorGUILayout.EndHorizontal();
                #endregion //setStepByStep

                #region sliders
                currentFrame = EditorGUILayout.IntSlider(currentFrame,
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
                #endregion //sliders

                #region turnAnimation
                /*EditorGUILayout.Space();
                if (GUILayout.Button(animationTurnTo))
                {
                    if (string.Equals(animationTurnTo, "Play"))
                    {
                        animationTurnTo = "Stop";
                        cAnim.play = true;
                        cAnim.stop = true;
                        cAnim.anim = cAnim.FindAnimData(animationName);
                    }
                    else if (string.Equals(animationTurnTo, "Stop"))
                    {
                        animationTurnTo = "Play";
                        cAnim.stop = true;
                        cAnim.play = false;
                    }
                } */
                #endregion //turnAnimation
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

    /// <summary>
    /// Функция, для инициализации части, с которой работает левый аниматор
    /// </summary>
    /// <param name="part"></param>
    public void SetPart(PartController part)
    {
        characterPart = part;
        parentPart = null;
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            for (int j = 0; j < cAnim.parts[i].childParts.Count; j++)
            {
                if (cAnim.parts[i].childParts[j] == part)
                    parentPart = cAnim.parts[i];
            }
        }
    }

	//Окно создания нового персонажа
	void CreateNew()
	{
		CreateNewAnimWindow animScreen=(CreateNewAnimWindow)EditorWindow.GetWindow(typeof(CreateNewAnimWindow));
        animScreen.Initialize();
		animScreen.rightAnim = rightAnim;
		animScreen.leftAnim = this;
	}

	/// <summary>
    /// Создать новый объект или загрузить из базы данных старый, для дальнейшей работы
    /// </summary>
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
            if (string.Equals("character", asset.type))
            {
                character.AddComponent<CharacterAnimator>();
            }
            else if (string.Equals("interactive", asset.type))
            {
                character.AddComponent<InterObjAnimator>();
            }
            InterObjAnimator cAnim= character.GetComponent<InterObjAnimator>();
            cAnim.SetDefaultAnimator();
            cAnim.visualData = asset;
			asset.visual = PrefabUtility.CreatePrefab (path + _name + ".prefab", character);
			rightAnim.animTypes = cAnim.animTypes;
		}
		else {
			character = PrefabUtility.InstantiatePrefab (asset.visual) as GameObject;
			character.transform.position=animEditor.gameObject.transform.position;
			InterObjAnimator cAnim = character.GetComponent<InterObjAnimator> ();
			for (int i = 0; i < cAnim.parts.Count; i++) {
				cAnim.parts [i].interp = new AnimationInterpretator (" ");
				cAnim.parts [i].interp.setInterp(cAnim.visualData.animInterpretators [i]);
			}
			rightAnim.animTypes = cAnim.animTypes;			
		}
		rightAnim.character = character;
		//saved = false;
	}

    /// <summary>
    /// Создать новый объект или загрузить из базы данных старый, для дальнейшей работы
    /// </summary>
    public void CreateNewInstance(string _name, string path, VisualData asset,VisualData stencilVis)
    {
        rightAnim.parts.Clear();
        rightAnim.animTypes.Clear();
        if (!string.Equals(characterName, "Name"))
        {
            DestroyImmediate(GameObject.Find(characterName));
        }
        character = null;
        characterPart = null;
        characterAnimation = null;
        partName = "Part";
        animationName = "Animation";
        characterName = _name;
        character = PrefabUtility.InstantiatePrefab(stencilVis.visual) as GameObject;
        character.name = _name;
        character.transform.position = animEditor.gameObject.transform.position;
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        InterObjAnimator stencil = stencilVis.visual.GetComponent<InterObjAnimator>();
        PartController cPart;
        AnimationInterpretator cInterp;
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            cPart = cAnim.parts[i];
            cInterp = new AnimationInterpretator(" ");
            cInterp.animTypes = new List<animationInfoTypes>();
            cInterp.setInterp(cAnim.visualData.animInterpretators[i]);
            cPart.interp = cInterp;
            AnimationInterpretator interp = ScriptableObject.CreateInstance<AnimationInterpretator>();
            interp.animTypes = new List<animationInfoTypes>();
            interp.setInterp(cInterp);
            interp.partPath = path+"Parts/";
            AssetDatabase.CreateAsset(interp, interp.partPath + cPart.gameObject.name + ".asset");
            cPart.path = path + cPart.gameObject.name + ".asset";
            GameObject asset1 = cPart.gameObject;
            asset1 = PrefabUtility.CreatePrefab(interp.partPath + cPart.gameObject.name + ".prefab", asset1);
            AssetDatabase.SaveAssets();
            cPart.interp = new AnimationInterpretator(cInterp);
        }
        cAnim.animTypes.Clear();
        for (int i = 0; i < stencil.animTypes.Count;i++)
        {
            cAnim.animTypes.Add(new animList(stencil.animTypes[i].typeName));
            for (int j = 0; j < stencil.animTypes[i].animations.Count;j++)
            {
                cAnim.animTypes[i].animations.Add(stencil.animTypes[i].animations[j]);
            }
        }
        cAnim.animBase.Clear();
        for (int i = 0; i < stencil.animBase.Count; i++)
        {
            cAnim.animBase.Add(new NamedAnimClass(stencil.animBase[i]));
        }
        cAnim.visualData = asset;
        asset.visual = PrefabUtility.CreatePrefab(path +"Visuals/"+ _name + ".prefab", character);
        SaveChanges();
        rightAnim.animTypes = cAnim.animTypes;
        rightAnim.character = character;
        //saved = false;
    }

    /// <summary>
    ///Сохранить изменения, проделанные в ходе работы над персонажем
    ///Пока что я не научил саму программу отслеживать эти изменения, 
    ///поэтому надо не забывать нажимать SaveChange, если надо чтобы эти изменения сохранились 
    /// </summary>
	public void SaveChanges()
	{
		AnimationInterpretator changeInterp;
		AnimationInterpretator saveInterp;
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator> ();
        VisualData vis = cAnim.visualData;
        vis.animInterpretators.Clear ();
        List<PartController> dependedParts = new List<PartController>();
        for (int i = 0; i < cAnim.parts.Count; i++) //Сохраняются все AnimationInterpretators.
        {
            if (cAnim.parts[i].childParts != null)
            {
                for (int j = 0; j < cAnim.parts[i].childParts.Count; j++)
                {
                    dependedParts.Add(cAnim.parts[i].childParts[j]);
                }
                cAnim.parts[i].childParts.Clear();
            }
            saveInterp = cAnim.parts[i].interp;
            changeInterp = saveInterp.FindInterp(saveInterp.partPath + cAnim.parts[i].gameObject.name + ".asset");
            changeInterp.setInterp(saveInterp);
            cAnim.parts[i].interp = changeInterp;
            EditorUtility.SetDirty(changeInterp);
        }
        for (int i = 0; i < dependedParts.Count; i++)
        {
            cAnim.parts.Remove(dependedParts[i]);
            DestroyImmediate(dependedParts[i].gameObject);
        }
        for (int i = 0; i < cAnim.parts.Count; i++) {
			saveInterp = cAnim.parts [i].interp;
			changeInterp = saveInterp.FindInterp (saveInterp.partPath + cAnim.parts [i].gameObject.name + ".asset");
			cAnim.parts [i].interp = changeInterp;
            EditorUtility.SetDirty(changeInterp);
            vis.animInterpretators.Add(changeInterp);
		}
		vis.visual=PrefabUtility.ReplacePrefab (character,vis.visual);
        EditorUtility.SetDirty(vis);
		saved = true;
	}

    void MakeStencil()
    {
        SaveChanges();
        VisualData asset = ScriptableObject.CreateInstance<VisualData>();
        asset.SetData(character.GetComponent<InterObjAnimator>().visualData);
        AssetDatabase.CreateAsset(asset, stencilPath + character.name + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
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
    void SetSequence(string sequence, bool right, bool parent)
    {
        if (!parent)
        {
            if (right)
            {
                characterAnimation.rsequence.sequence = sequence;
            }
            else
            {
                characterAnimation.lsequence.sequence = sequence;
            }
            if ((string.Equals(orientation, "Right")) == right)
            {
                characterPart.mov.setSequence(sequence, true);
            }
        }
        else
        {
            if (right)
            {
                characterAnimation.rsequence.parentSequence = sequence;
            }
            else
            {
                characterAnimation.lsequence.parentSequence = sequence;
            }
        }
    }

	//Функция, которая ставит в даннном кадре данной анимации часть тела на указанный слой
	void SetLayerInFrame(int _layer)
	{
        if (string.Equals(orientation, "Right"))
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
        part.interp.SetDefaultLayer(_layer, string.Equals(orientation, "Right"));
	}

    /// <summary>
    /// Функция, которая в данной анимации выставляет данное количество проигрываемых кадров в секунду
    /// </summary>
    /// <param name="Кадры в секунду"></param>
    void SetFPS(int _FPS)
    {
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        AnimClass anim = cAnim.FindAnimData(animationName);
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
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        AnimClass anim = cAnim.FindAnimData(animationName);
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
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        AnimClass anim = cAnim.FindAnimData(animationName);
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            cAnim.parts[i].interp.animTypes[anim.type].animInfo[anim.numb].loop = _loop;
        }
    }
}
