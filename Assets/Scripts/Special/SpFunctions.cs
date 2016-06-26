using UnityEngine;
using System;
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
	public static bool ComprFunctionality(int arg1, string opr, int arg2)
	{
		return (((arg1 < arg2) && (string.Equals(opr,"<"))) ||
						((arg1 <= arg2) && (string.Equals(opr,"<="))) ||
						((arg1 == arg2) && (string.Equals(opr,"="))) ||
						((arg1 > arg2) && (string.Equals(opr,">"))) ||
						((arg1 >= arg2) && (string.Equals(opr,">="))) ||
						((arg1 != arg2) && (string.Equals(opr,"!=")))||
		        		(string.Equals(opr,"!")));
	}

    /// <summary>
    /// Функция, которая позволяет использовать ComparativeClass и по сути ей можно заменять 
    /// простейшие операции сравнения float c float'ом.
    /// Зачем это нужно? Да чтобы можно было операции сравнения с нужным числом задавать в самом редакторе.
    /// </summary>
    public static bool ComprFunctionality(float arg1, string opr, float arg2)
    {
        return ((arg1 < arg2) && (string.Equals(opr, "<")) ||
                        (arg1 <= arg2) && (string.Equals(opr, "<=")) ||
                        (arg1 == arg2) && (string.Equals(opr, "=")) ||
                        (arg1 > arg2) && (string.Equals(opr, ">")) ||
                        (arg1 >= arg2) && (string.Equals(opr, ">=")) ||
                        (arg1 != arg2) && (string.Equals(opr, "!=")) ||
                        (string.Equals(opr, "!")));
    }

    /// <summary>
    /// Запрашивает 2 клавиши, и в зависимости от того, какая из них зажата дольше,
    ///  направляет действие (например, движение) либо влево, либо вправо
    /// </summary>
    public static int ChooseDirection(ButtonClass b1, ButtonClass b2)
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
			RealSign(vect1.x*vect2.y-vect1.y*vect2.x)*
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
    /// Вспомогательная функция, возвращающая вектор направления движения для существ, которые могут передвигаться на разных поверхностях, когда они повёрнуты вправо
    /// </summary>
    public static Vector3 GetSurfaceRightDirection(groundOrientationEnum grEnum)
    {
        switch (grEnum)
        {
            case groundOrientationEnum.down:
                {
                    return new Vector3(1f, 0f,0f);
                    break;
                }
            case groundOrientationEnum.right:
                {
                    return new Vector3(0f, 1f,0f);
                    break;
                }
            case groundOrientationEnum.up:
                {
                    return new Vector3(-1f, 0f,0f);
                    break;
                }
            case groundOrientationEnum.left:
                {
                    return new Vector3(0f, -1f,0f);
                    break;
                }
            default:
                {
                    return Vector3.zero;
                    break;
                }
        }
    }

    /// <summary>
    /// На какой поверхности окажется персонаж при повороте
    /// </summary>
    /// <param name="изначальная ориентация"></param>
    /// <param name="поворот будет произведён по часовой стрелке на 90 градусов"></param>
    public static groundOrientationEnum GetNextSurfaceOrientation(groundOrientationEnum _grOrientation, bool sunwise)
    {
        int enumValue = (int)_grOrientation;
        enumValue = enumValue + (sunwise ? -1 : 1);
        enumValue = enumValue < 0 ? 3 : (enumValue > 3 ? 0 : enumValue);
        return (groundOrientationEnum)enumValue;
    }

    /// <summary>
    /// На какой угол надо повернуться, чтобы оказаться на заданной поверхности
    /// </summary>
    public static float GetAngleOfSurfaceOrientation(groundOrientationEnum grEnum)
    {
        switch (grEnum)
        {
            case groundOrientationEnum.down:
                {
                    return 0f;
                    break;
                }
            case groundOrientationEnum.right:
                {
                    return 90f;
                    break;
                }
            case groundOrientationEnum.up:
                {
                    return 180f;
                    break;
                }
            case groundOrientationEnum.left:
                {
                    return 270f;
                    break;
                }
            default:
                {
                    return 0f;
                    break;
                }
        }
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

    public static KeyboardActorController GetPlayer()
    {
        return GameObject.FindGameObjectWithTag(Tags.player).GetComponent<KeyboardActorController>();
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
			obj=FindPart(parts[i].childParts, name);
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
	/// Функция, меняющая цвет персонажа, состоящего из анимационных частей.
	/// </summary>
	public static void ChangePartColor (List<PartController> parts, Color color)
	{
		int i;
		for (i=0;i<parts.Count;i++)
		{
			if (parts[i].mov.individualMaterials[0]!=null)
				parts[i].mov.setMaterialColor(color);
			ChangePartColor(parts[i].childParts, color);
		}
	}

	/// <summary>
	/// Функция, вызываемая аниматором, которая принимает на вход два числа, 
	/// а потом передаёт их всем анимационным частям данного персонажа 
	/// </summary>
	public static void AnimateIt(List<PartController> parts, AnimClass anim)
	{
		for (int i=0;i<parts.Count;i++)
		{
			parts[i].type=anim.type;
			parts[i].numb=anim.numb;
			AnimateIt(parts[i].childParts,anim);
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
    /// Изменить данные о комнатах, а также о своём текущем местоположении при переходе в другую комнату.
    /// </summary>
    public static void ChangeRoomData(AreaClass room)
    {
        foreach (GameObject obj in GameStatistics.currentArea.hideForeground)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
        foreach (GameObject obj in GameStatistics.currentArea.lightSources)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        GameStatistics.currentArea = room;
        GameStatistics.lumControl.SetIntensity(room.DayAmbIntensity, room.NightAmbIntensity);
        foreach (GameObject obj in GameStatistics.currentArea.hideForeground)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }
        foreach (GameObject obj in GameStatistics.currentArea.lightSources)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Поставить игру на паузу
    /// </summary>
    public static void Pause(string windowName)
    {
        InterfaceController interControl = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<InterfaceController>();
        if (GameStatistics.paused)
        {
            Time.timeScale = 1f;
            GameStatistics.paused = false;
            interControl.CloseActiveWindow();
        }
        else 
        {
            Time.timeScale = 0f;
            GameStatistics.paused = true;
            interControl.ChangeWindow(windowName);
        }
    }

    /// <summary>
    /// Поставить игру на паузу
    /// </summary>
    public static void ChangeWindow(string windowName)
    {
        InterfaceController interControl = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<InterfaceController>();
        interControl.ChangeWindow(windowName);
        if (!GameStatistics.paused)
        {
            Time.timeScale = 0f;
            GameStatistics.paused = true;
        }
    }

    public static void BeginDialog(NPCActions _NPC)
    {
        InterfaceController interControl = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<InterfaceController>();
        interControl.OpenDialogWindow(_NPC);
    }

    /// <summary>
    /// Сохранить игру
    /// </summary>
    public static void OpenSaveWindow(bool save)
    {
        SaveMenu saveMenu = GameObject.FindGameObjectWithTag(Tags.interfaceWindows).GetComponentInChildren<SaveMenu>();
        saveMenu.ChangeMod(save ? "Save" : "Load");
        ChangeWindow("saves");
    }

    /// <summary>
    /// Сохранить игру
    /// </summary>
    public static void OpenSaveWindow(bool save, CheckpointActions checkpoint)
    {
        GameStatistics gameStats = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameStatistics>();
        if (gameStats.lastCheckPoint != null)
        {
            gameStats.lastCheckPoint.Activated = false;
        }
        gameStats.lastCheckPoint = checkpoint;
        SaveMenu saveMenu = GameObject.FindGameObjectWithTag(Tags.interfaceWindows).GetComponentInChildren<SaveMenu>();
        saveMenu.ChangeMod(save ? "Save" : "Load");
        ChangeWindow("saves");
    }

    /// <summary>
    /// Сделать игровое сообщение
    /// </summary>
    public static void SendMessage(MessageSentEventArgs e)
    {
        GameStatistics gameStats = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameStatistics>();
        gameStats.OnMessageSent(e);
    }

    /// <summary>
    /// Начать показывать здоровье данного босса (или наоборот скрыть)
    /// </summary>
    public static void SetUpBossHealth(BossController boss, bool setUp)
    {
        GameUI gameUI = GameObject.FindGameObjectWithTag(Tags.interfaceWindows).GetComponentInChildren<GameUI>();
        if (setUp)
        {
            gameUI.SetBossHp(boss);
        }
        else
        {
            gameUI.UnsetBossHp(boss);
        }
    }

    public static int RealSign(float x)
    {
        if (x == 0)
            return 0;
        else return Mathf.RoundToInt(Mathf.Sign(x));
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
        GAF.Objects.GAFRenderProcessor rend = new GAF.Objects.GAFRenderProcessor ();
		obj.GetComponent<GAF.Core.GAFMovieClip>().settings.spriteLayerValue = order;
		rend.init(obj.GetComponent<GAF.Core.GAFMovieClip>(),
		          obj.GetComponent<MeshFilter>(),
		          obj.GetComponent<Renderer>());
	}

    /// <summary>
    /// Добавить новые данные в журнал
    /// </summary>
    public static void SendNewJournalData(JournalRefreshEventArgs e)
    {
        GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameHistory>().journalEvents.OnNewJournalData(e);
    }

    public static void QuestComplete(JournalRefreshEventArgs e)
    {
        GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<GameHistory>().journalEvents.OnQuestCompleted(e);
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

    public static AreaClass GetCurrentArea()
    {
        return GameStatistics.currentArea;
    }

}
