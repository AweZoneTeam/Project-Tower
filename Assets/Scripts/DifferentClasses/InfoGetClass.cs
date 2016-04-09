using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, ранее используемый для анализа окружающей персонажа среды. 
/// Все данные об информации, которую персонажу нужно проверить.
/// </summary>
[System.Serializable]
public class InfoGetClass
{
	public string name;//Какую информацию мы хотим подтвердить
	public EnvironmentStats stats, elseStats; //Какие должны стать параметры персонажа, если проверяемая информация подтвердилась
	public int typeOfInfo;//Тип информации
	public int numbOfInfo;//Номер информации
	public GameObject indicator;//Объект, используемый для сбора информации (индикатор)
	public List<int> intParametres;//Параметры сбора информации типа int (не самый нужеый параметр, здесь он на всякий случай) 
	public List<float> floatParametres;//Параметры сбора информации типа float (например, радиус просматриваемой области)
	public List<Vector2> infoVectors;//Куда надо "смотреть", чтобы подтвердить информацию
	public LayerMask whatToCheck;//Какие игровые слои надо проверить
	public List<string> whoToCheck;//Объекты с какими тегами надо проверить
}
