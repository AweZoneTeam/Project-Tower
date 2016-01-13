﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//В этот скрипт я заносил все специальные функции, которые могут использоваться каким угодно скриптом. 
//Есть идея, сделать из этих функций отдельную библиотеку
public static class SpFunctions {

	/// <summary>
	/// Функция, которая позволяет использовать ComparativeClass и по сути ей можно заменять 
	/// простейшие операции сравнения int c int'ом.
	/// Зачем это нужно? Да чтобы можно было операции сравнения с нужным числом задавать в самом редакторе.
	/// </summary>
	public static bool ComprFunctionality(int f, ComparativeClass cpr)
	{
		return ((f < cpr.val) && (cpr.oper == "<") ||
						(f <= cpr.val) && (cpr.oper == "<=") ||
						(f == cpr.val) && (cpr.oper == "=") ||
						(f >= cpr.val) && (cpr.oper == ">") ||
						(f >= cpr.val) && (cpr.oper == ">=") ||
						(f != cpr.val) && (cpr.oper == "!=")||
		        		(cpr.oper=="!"));
	}

	/// <summary>
	/// Функция, которая позволяет использовать ComparativeClass и по сути ей можно заменять 
	/// простейшие операции сравнения float c float'ом.
	/// Зачем это нужно? Да чтобы можно было операции сравнения с нужным числом задавать в самом редакторе.
	/// </summary>
	public static bool ComprFunctionality(float f, FComparativeClass cpr)
	{
		return ((f < cpr.val) && (cpr.oper == "<") ||
		        (f <= cpr.val) && (cpr.oper == "<=") ||
		        (f == cpr.val) && (cpr.oper == "=") ||
		        (f >= cpr.val) && (cpr.oper == ">") ||
		        (f >= cpr.val) && (cpr.oper == ">=") ||
		        (f != cpr.val) && (cpr.oper == "!=")||
		        (cpr.oper=="!"));
	}

	/// <summary>
	/// Запрашивает 2 клавиши, и в зависимости от того, какая из них зажата дольше,
	///  направляет действие (например, движение) либо влево, либо вправо
	/// </summary>
	public static int ChooseDirection(ButtonClass.button b1, ButtonClass.button b2)
	{
		if (b1.timer>b2.timer)
			return 1;
		else if (b1.timer<b2.timer)
			return -1;
		else return 0;
	}

	/// <summary>
	/// Эту функция используется для того, чтобы задать направление полёта стрелы
	/// </summary>
	public static float RealAngle(Vector2 vect1, Vector2 vect2)
	{
		return 180/Mathf.PI*
			realSign(vect1.x*vect2.y-vect1.y*vect2.x)*
				(Mathf.Acos((vect1.x * vect2.x + vect1.y * vect2.y) /vect1.magnitude /vect2.magnitude));
	}

	/// <summary>
	/// Создаёт 2-мерный вектор из x и y координат данного 3-мерного вектора
	/// </summary>
	public static Vector2 VectorConvert(Vector3 vect)
	{
		return new Vector2 (vect.x, vect.y);
	}

	/// <summary>
	/// Функция, следящая за временем исполнения деятельности - вызов функции уменьшает это время
	/// </summary>
	public static void ChangeTimer(int timer, ActivityClass.activites act, ActivityClass.activites[] acts)
	{
		if (act.weapon == null)
			acts [act.numb].timer = timer;
		else 
			act.weapon.moveset [act.numb].timer = timer;
	}

	/// <summary>
	/// Функция, которая позволяет узнать, сколько времени уже длится деятельность
	/// </summary>
	public static int EmployTime(ActivityClass.activites act, ActivityClass.activites[] acts)
	{
		if (act.weapon == null)
			return acts [act.numb].timer;
		else 
			return act.weapon.moveset [act.numb].timer;
	}

	/// <summary>
	/// Узнаёт проекцию vect1 по vect2 и отнимает её от vect1.
	/// </summary>
	public static Vector2 Ortog(Vector2 vect1, Vector2 vect2)
	{
		Vector2 vect3;
		float scal;
		scal = vect1.x * vect2.x + vect1.y + vect2.y;
		vect3 = scal * vect2 / vect2.sqrMagnitude;
		return vect1 - vect3;
	}

	/// <summary>
	/// Функция, которая рыскает по данному списку анимационных частей,
	///  причём учитывая все зависимые части, и возвращает часть с данным именем
	/// </summary>
	public static GameObject FindPart(List<PartController> parts, string name)
	{
		GameObject obj=null;
		int i;
		for (i=0;i<parts.Count;i++)
		{
			if ((string.Equals(name, parts[i].gameObject.name))||(string.Equals(name+"(Clone)", parts[i].gameObject.name)))
			{
				obj=parts[i].gameObject;
				break;
			}
			obj=FindPart(parts[i].parts, name);
			if (obj!=null) break;
		}
		return obj;
	}

	/// <summary>
	/// Функция, которая рыскает по зависимым частям данной анимационной части,
	///  причём учитывая все зависимые части, и возвращает часть с данным именем
	/// </summary>
	public static GameObject FindObject(GameObject obj1, string name)
	{
		GameObject obj=null;
		if (string.Equals(name, obj1.name))
		    return obj1;
		int i;
		for (i=0;i<obj1.transform.childCount;i++)
		{
			if (string.Equals(name, obj1.transform.GetChild(i).gameObject.name))
			{
				obj=obj1.transform.GetChild(i).gameObject;
				break;
			}
			obj=FindObject(obj1.transform.GetChild(i).gameObject, name);
			if (obj!=null) break;
		}
		return obj;
	}

	/// <summary>
	/// Функция, узнающая, в какой руке персонаж держит интересующее нас оружие, и ставящая в этом оружии соответствуюший мувсет
	/// </summary>
	public static void SetMoveset (GameObject obj, WeaponClass weapon)
	{
		for (int i=0; i<weapon.moveset.Length; i++)
		{
			for (int j=0; j<weapon.moveset[i].what.Length; j++)
				for (int k=0; k<weapon.moveset[i].what[j].OBJDescription.Length; k++)
					weapon.moveset [i].what [j].OBJ [k] = FindObject (obj, weapon.moveset [i].what [j].OBJDescription[k]);
			for (int j=0; j<weapon.moveset[i].whatIf.Length; j++)
				for (int k=0; k<weapon.moveset[i].whatIf[j].OBJDescription.Length; k++)
					weapon.moveset [i].whatIf [j].OBJ [k] = FindObject (obj, weapon.moveset [i].whatIf [j].OBJDescription[k]);
		}
	}

	/// <summary>
	/// Функция, меняющая цвет персонажа, состоящего из анимационных частей.
	/// </summary>
	public static void ChangePartColor (List<PartController> parts, Color color)
	{
		int i;
		for (i=0;i<parts.Count;i++)
		{
			if (parts[i].mov.individualMaterials[0]!=null)
				parts[i].mov.setMaterialColor(color);
			ChangePartColor(parts[i].parts, color);
		}
	}

	/// <summary>
	/// Функция, вызываемая аниматором, которая принимает на вход два числа, 
	/// а потом передаёт их всем анимационным частям данного персонажа 
	/// </summary>
	public static void AnimateIt(List<PartController> parts, animClass anim)
	{
		for (int i=0;i<parts.Count;i++)
		{
			parts[i].type=anim.type;
			parts[i].numb=anim.numb;
			AnimateIt(parts[i].parts,anim);
		}
	}

	/// <summary>
	/// Функция, которая меняет один кадр в данном анимационном клипею 
	/// Нужна для нормального проигрывания таких анимаций, как поднятие по лестнице.
	/// </summary>
	public static void FrameToFrame(GAF.Core.GAFMovieClip mov, bool loop)
	{
		int i;
		uint k1=1;
		i=(int)mov.getCurrentFrameNumber ();
		i++;
		if ((i>(int)mov.currentSequence.endFrame)&&(loop))
			i=(int)mov.currentSequence.startFrame;
		else if ((i>(int)mov.currentSequence.endFrame))
		if (i<(int)mov.currentSequence.startFrame)
			i=(int)mov.currentSequence.startFrame;
		mov.gotoAndStop(k1*(uint)i);
	}

	/// <summary>
	/// Функция, зеркально отражающая заданный объект.
	/// </summary>
	public static void Flip(Transform trans, int x)
	{
		if (Mathf.Sign (trans.localScale.x)!=x*1f)
			trans.localScale=new Vector3(trans.localScale.x*-1,
			                             trans.localScale.y,
			                             trans.localScale.z);
	}

	/// <summary>
	/// Функция, которая работает, как Mathf.Sign, но возвращает 0 при аргументе=0
	/// </summary>
	public static float realSign(float x)
	{
		if (x==0)
			return 0f;
		else return Mathf.Sign(x);
	}

	/// <summary>
	/// Функция, нормально округляющая число (Mathf.Round всегда возвращает число больше вводимого)
	/// </summary>
	public static float Div(float x, float y)
	{
		if ((x/y-Mathf.Round (x/y))<0)
			return Mathf.Round(x/y)-1;
		else 
			return Mathf.Round (x/y);
	}

	/// <summary>
	/// Меняет порядок прорисовки объекта (основанного на технологии GAF) на указанный.
	/// </summary>
	public static void ChangeRenderOrder(int order, GameObject obj)
	{
		GAF.Objects.GAFRenderProcessor rend;
		rend = new GAF.Objects.GAFRenderProcessor ();
		obj.GetComponent<GAF.Core.GAFMovieClip>().settings.spriteLayerValue = order;
		rend.init(obj.GetComponent<GAF.Core.GAFMovieClip>(),
		          obj.GetComponent<MeshFilter>(),
		          obj.GetComponent<Renderer>());
	}

	/// <summary>
	/// Является ли округленное число чётным?
	/// </summary>
	public static bool IsItEven(float x)
	{
		if (Div (x,1)-2*Div (x,2)>0)
			return false;
		else 
			return true;
	}
}
