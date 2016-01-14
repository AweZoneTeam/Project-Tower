using UnityEngine;
using System.Collections;

/// <summary>
///Класс, который содержит в себе число и операцию сравнения с этим числом. 
///Используя SpFunctuins.ComprFunctionality, можно в редакторе по сути создать не только поле, в котором будет число
///Но и задавать необходимые операции сравнения
/// </summary>
[System.Serializable]
public class ComparativeClass
{
	public int val;
	public string oper;
}