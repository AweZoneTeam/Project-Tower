﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GAF.Core;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Простейший аниматор CAV-объекта
/// </summary>
public class InterObjAnimator : MonoBehaviour
{
    #region fields

    protected AnimClass anim=new AnimClass(0,0);//Идентификатор проигрываемой в данный момент анимации.
    public List<PartController> parts = new List<PartController>();//Части тела, управляемых аниматором
    public VisualData visualData;//Визуальная база данных, в которую мы будем вносить изменения по создаваемому персонажу
    //public ShadowScript shadow;//Тень, что отбрасывает персонаж
    //private LayerMask whatIsGround;//На какие поверхности отбрасывается тень

    public List<animList> animTypes = new List<animList>();//База данный по анимациям, проигрываемых аниматором, которые отсортированы по типам
    public List<NamedAnimClass> animBase = new List<NamedAnimClass>();//база данных по анимациям, используемых аниматором. 
                                                                      //В отличие от пердыдущего списка - это одномерный массив. 
                                                                      //Нужен для удобного написания скриптов.
    //public bool play = false, stop = true;//два весёлых була, обеспечивающие проигрывание анимации непосредственно в самом редакторе.

    private Direction direction;//Параметры персонажа

    #endregion //fields

    /// <summary>
    /// Создать все рабочие списки
    /// </summary>
    public virtual void SetDefaultAnimator()
    {
        parts = new List<PartController>();
        animTypes = new List<animList>();
        animBase = new List<NamedAnimClass>();
    }

    /*public  virtual void FixedUpdate()
    {
        if (stats.groundness == (int)groundness.inAir)
        {
            RaycastHit ray;
            if (Physics.Raycast(new Ray(transform.parent.position, new Vector3(0f, -1f, 0f)), out ray, 40f, whatIsGround))
            {
                shadow.SetY(ray.point.y);
            }
        }
    }*/

    public virtual void FixedUpdate()
    {
        Sinchronize();
    }

    public virtual void Update()
    {
     //   Sinchronize();

        /*
#if UNITY_EDITOR
        if (play && !stop)//Пусть продолжается работа частей
        {
            SpFunctions.AnimateIt(parts, anim);
        }
        else if (play && stop)//Пусть начнётся работа частей
        {
            stop = false;
            SetPartAnimationsAtBegining(anim.type, anim.numb, SpFunctions.realSign(gameObject.transform.lossyScale.x)>0, true);
        }
        else if (!stop && !play)//Всё спокойно, ничего не происходит
        {
        }

        else if (stop && !play)//Остановить анимацию и поставить её в самое начало
        {
            stop = false;
            SetPartAnimationsAtBegining(anim.type, anim.numb, SpFunctions.realSign(gameObject.transform.lossyScale.x) > 0, false);
        }
        #endif //UNITY_EDITOR*/
    }

    /// <summary>
    /// Функция, которая всем части аниматора поставит в заданные анимации
    /// </summary>
    /// <param name="тип анимации"></param>
    /// <param name="номер анимации"></param>
    public void setPartAnimations(int type, int numb, bool right)
    {
        GAFMovieClip mov;
        AnimationInterpretator interp;
        List<PartController> childParts = new List<PartController>();
        foreach (PartController part in parts)
        {
            childParts = null;
            part.type = type;
            part.numb = numb;
            mov = part.mov;
            interp = part.interp;
            if (part.childParts!=null)
            {
                childParts = part.childParts;
            }
            if (right)
            {
                mov.setSequence(interp.animTypes[type].animInfo[numb].rsequence.sequence, true);
                if (childParts != null)
                {
                    if (childParts.Count > 0)
                    {
                        if (childParts[childParts.Count - 1].interp.animTypes[type].animInfo[numb].rsequence.parentSequence.Length > 1)
                        {
                            mov.setSequence(childParts[childParts.Count - 1].interp.animTypes[type].animInfo[numb].rsequence.parentSequence, true);
                        }
                    }
                }
            }
            else
            {
                mov.setSequence(interp.animTypes[type].animInfo[numb].lsequence.sequence, true);
                if (childParts != null)
                {
                    if (childParts.Count > 0)
                    {
                        if (childParts[childParts.Count - 1].interp.animTypes[type].animInfo[numb].lsequence.parentSequence.Length > 1)
                        {
                            mov.setSequence(childParts[childParts.Count - 1].interp.animTypes[type].animInfo[numb].lsequence.parentSequence, true);
                        }
                    }
                }
            }
        }
    }

    /// <summary>
    /// Функция, которая по заданному названию анимации выдаёт её тип и номер
    /// </summary>
    /// <param name="название анимации"></param>
    /// <returns>тип и номер анимации</returns>
    public AnimClass FindAnimData(string animName)
    {
        AnimClass a = anim;
        foreach (NamedAnimClass s in animBase)
        {
            if (string.Equals(s.animName, animName))
            {
                a = new AnimClass(s.type, s.numb);
            }
        }
        return a;
    }

    /// <summary>
    /// Функция, которая определяет, какая анимация должна проигрываться в данный момент
    /// </summary>
    public void Animate(string animName)
    {
        anim = FindAnimData(animName);
        if (parts.Count > 0)
        {
            if (!parts[0].mov.isPlaying())
            {
                foreach (PartController part in parts)
                {
                    part.mov.play();
                }
            }
        }
        SpFunctions.AnimateIt(parts, anim);
        Sinchronize();
    }

    /// <summary>
    /// Функция, что ставит на паузу все части
    /// </summary>
    public void Pause()
    {
        if (parts != null)
        {
            if (parts.Count > 0)
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    parts[i].mov.stop();
                }
            }
        }
        Sinchronize();
    }

    public bool CompareAnimation(string animName)
    {
        return string.Equals(animName, animTypes[anim.type].animations[anim.numb]);
    }

    /// <summary>
    /// Функция, визуально меняющая ориентацию объекта
    /// </summary>
    public void Flip()
    {
        gameObject.transform.localScale = new Vector3(-1 * gameObject.transform.localScale.x,
                                                          gameObject.transform.localScale.y,
                                                          gameObject.transform.localScale.z);
    }

    /// <summary>
    /// Контроль синхронности движений
    /// </summary>
    protected void Sinchronize()
    {
        int frameGap;
        animationInfo aInfo;
        if (parts != null)
        {
            if (parts.Count > 0)
            {
                frameGap = (int)parts[0].mov.getCurrentFrameNumber() - (int)parts[0].mov.currentSequence.startFrame;
                for (int i = 1; i < parts.Count; i++)
                {
                    aInfo = parts[i].interp.animTypes[anim.type].animInfo[anim.numb];
                    parts[i].mov.gotoAndStop((uint)((int)parts[i].mov.currentSequence.startFrame + frameGap));
                    if (!aInfo.stepByStep || !aInfo.stopStepByStep)
                    {
                        parts[i].mov.setPlaying(true);
                    }
                    /*else if (aInfo.stepByStep)
                    {
                        parts[i].mov.gotoAndStop((uint)((int)parts[i].mov.currentSequence.startFrame + frameGap));
                    }*/
                }
            }
        }
    }

    /// <summary>
    /// Задать поле статов
    /// </summary>
    /// <param name="задаваемые параметры"></param>
    public void SetStats(Direction _direction)
    {
        direction = _direction;
    }

}

#region AuxillaryClasses
/// <summary>
/// Класс, используемый для создания отсортированной по типам базы данных по используемым аниматором анимациям
/// </summary>
[System.Serializable]
public class animList
{
    public string typeName;
    public List<string> animations = new List<string>();

    public animList(string name)
    {
        animations = new List<string>();
        typeName = name;
    }

    public animList(string name, string animName)
    {
        animations = new List<string>();
        typeName = name;
        animations.Add(animName);
    }

}

[System.Serializable]
public class NamedAnimClass : AnimClass
{
    public string animName;

    public NamedAnimClass(string _name, int _type, int _numb)
    {
        animName = _name;
        type = _type;
        numb = _numb;
    }

    public NamedAnimClass(NamedAnimClass origin)
    {
        animName = origin.animName;
        type = origin.type;
        numb = origin.numb;
    }
}
#endregion //AuxillaryClasses