using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, представляющий собой проход из одной комнаты в другую - если персонаж коснётся этого объекта, он перейдёт в соседнюю комнату
//о/ </summary>
public class EnterClass : MonoBehaviour
{
    public doorEnum enterType;//Тип прохода
    public AreaClass nextRoom;// Комната, в которую ведёт проход
}
