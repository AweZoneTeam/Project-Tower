﻿using UnityEngine;
using System.Collections;

/// <summary>
///Класс передаваемой анимации. Именно объект этого класса передаётся аниматору, 
///чтобы он смог "сказать" своим анимационным частям, какие анимации проигрывать.
/// </summary>
[System.Serializable]
public class AnimClass
{
    public int type;
    public int numb;

    public AnimClass(int _type, int _numb)
    {
        type = _type;
        numb = _numb;
    }

    public AnimClass()
    {
        type = 0;
        numb = 0;
    }
}
