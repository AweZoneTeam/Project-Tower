using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Данные, которые использует редактор анимаций - список активынх персонажей и список всех созданных персонажей
[System.Serializable]
public class AnimationEditorDataBase : ScriptableObject 
{
	public List<string> allCharacters = new List<string>();
	public List<string> usedCharacters = new List<string>();

}
