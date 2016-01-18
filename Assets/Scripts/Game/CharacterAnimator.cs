using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using GAF.Core;

/// <summary>
/// Аниматор (разумного) персонажа. Он говорит частям тела персонажа, как им надо двигаться.
/// </summary>
public class CharacterAnimator : BaseAnimator
{
	public List<PartController> parts=new List<PartController>();//Части тела, управляемых аниматором
    public VisualData visualData;//Визуальная база данных, в которую мы будем вносить изменения по создаваемому персонажу

	public List<animList> animTypes=new List<animList>();//База данный по анимациям, проигрываемых аниматором, которые отсортированы по типам
    public Dictionary<string,AnimClass> animBase=new Dictionary<string, AnimClass>();//база данных по анимациям, используемых аниматором. 
                                                                                     //В отличие от пердыдущего списка - это одномерный массив. 
                                                                                     //Нужен для удобного написания скриптов.
    public List<string> animNames;//Массив, хранящий ключи для animBase

    /// <summary>
    /// Создать все рабочие списки
    /// </summary>
    public void SetDefaultAnimator()
    {
        parts = new List<PartController>();
        animTypes = new List<animList>();
        animBase = new Dictionary<string, AnimClass>();
        animNames = new List<string>();
    }

	public void Update()
	{
		SpFunctions.AnimateIt (parts, 
		                   anim);
	}

    /// <summary>
    /// Функция, которая всем части аниматора поставит в заданные анимации
    /// </summary>
    /// <param name="тип анимации"></param>
    /// <param name="номер анимации"></param>
    public void setPartAnimations(int type, int numb)
    {
        GAFMovieClip mov;
        AnimationInterpretator interp;
        foreach (PartController part  in parts)
        {
            part.type = type;
            part.numb = numb;
            mov = part.mov;
            interp = part.interp;
            mov.setSequence(interp.animTypes[type].animInfo[numb].rsequence.sequence, true);
        }
    }

}

/// <summary>
/// Класс, используемый для создания отсортированной по типам базы данных по используемым аниматором анимациям
/// </summary>
[System.Serializable]
public class animList
{
	public string typeName;
	public List<string>	animations=new List<string>();

	public animList (string name, string animName)
	{
		animations=new List<string>();
		typeName=name;
		animations.Add (animName);
	}
}

/// <summary>
/// Класс, необходимый для создания нужного нам отображения полей аниматора персонажа
/// </summary>
[CustomEditor(typeof(CharacterAnimator))]
public class CharacterAnimatorEditor : BaseAnimatorEditor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        CharacterAnimator cAnim = (CharacterAnimator)target;
        /*Dictionary<string, AnimClass> animBase = cAnim.animBase;
        Dictionary<string, AnimClass>.KeyCollection animKeys = animBase.Keys;
        for (int i = 0; i < cAnim.animBase.Count; i++)
        {
        }*/
    }
}