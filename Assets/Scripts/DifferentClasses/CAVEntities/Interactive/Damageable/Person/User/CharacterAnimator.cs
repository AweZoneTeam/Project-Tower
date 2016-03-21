using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GAF.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Аниматор (разумного) персонажа. Он говорит частям тела персонажа, как им надо двигаться.
/// </summary>
public class CharacterAnimator : InterObjAnimator
{
    #region fields
    public ShadowScript shadow;//Тень, что отбрасывает персонаж
    private LayerMask whatIsGround;//На какие поверхности отбрасывается тень

    public bool play = false, stop = true;//два весёлых була, обеспечивающие проигрывание анимации непосредственно в самом редакторе.
    private Stats stats;//Параметры персонажа

    #endregion //fields

    /// <summary>
    /// Создать все рабочие списки
    /// </summary>
    public override void SetDefaultAnimator()
    {
        base.SetDefaultAnimator();
    }

    public virtual void Start()
    {
        whatIsGround = GetComponentInParent<PersonController>().whatIsGround;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (stats.groundness == groundnessEnum.inAir)
        {
            RaycastHit ray;
            if (shadow != null)
            {
                if (Physics.Raycast(new Ray(transform.parent.position, new Vector3(0f, -1f, 0f)), out ray, 200f, whatIsGround))
                {
                    shadow.SetY(ray.point.y);
                }
            }
        }    
    }

	public override void Update()
	{
        base.Update();
    }

    /// <summary>
    /// Функция, вызываемая внутри самого аниматора для контроля анимаций в редакторе
    /// </summary>
    void SetPartAnimationsAtBegining(int type, int numb, bool right, bool _play)
    {
        setPartAnimations(type, numb, right);
        GAFMovieClip mov;
        foreach (PartController part in parts)
        {
            mov = part.mov;
            mov.gotoAndPlay(mov.currentSequence.startFrame);
            part.SetPlay(_play);
        }
    }

    /// <summary>
    /// Задать поле статов
    /// </summary>
    /// <param name="задаваемые параметры"></param>
    public void SetStats(Stats _stats)
    {
        stats = _stats;
    }
}

#if UNITY_EDITOR
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
        if (GUILayout.Button("Delete anim Base"))
        {
            cAnim.animBase = new List<NamedAnimClass>();
        }
        if (GUILayout.Button("Create Database"))
        {
            if (cAnim.animBase == null)
            {
                cAnim.animBase = new List<NamedAnimClass>();
            }
            if (cAnim.animBase.Count == 0)
            {
                for (int i = 0; i < cAnim.animTypes.Count; i++)
                {
                    for (int j = 0; j < cAnim.animTypes[i].animations.Count;j++)
                    {
                        cAnim.animBase.Add(new NamedAnimClass(cAnim.animTypes[i].animations[j], i, j));
                    }
                } 
            }
        }
        /*if (GUILayout.Button("Set Part Paths"))
        {
            SetPartPath(cAnim);
        }
        Dictionary<string, AnimClass> animBase = cAnim.animBase;
        Dictionary<string, AnimClass>.KeyCollection animKeys = animBase.Keys;
        for (int i = 0; i < cAnim.animBase.Count; i++)
        {
        }*/
    }

    /*void SetPartPath(CharacterAnimator cAnim)
    {
        foreach (PartController part in cAnim.parts)
        {
            part.path = part.interp.partPath + part.gameObject.name + ".asset";
        }
    }*/

}
#endif