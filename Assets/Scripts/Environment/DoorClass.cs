using UnityEngine;
using System.Collections;

/// <summary>
/// Двери - тип объектов, которые связывают комнаты. Взаимодействуя с дверью, персонаж переносится в заданное место в другой комнате.
/// </summary>
[System.Serializable]
public class DoorClass: MonoBehaviour
{
    public enum doorTypes { left, right, back, forward, down, up, everywhere };//За какой тип двери отвечает каждый номер типа
    public int doorType;//Тип двери
    public AreaClass roomPath;//Куда ведёт данная дверь
    public Vector3 nextPosition;//Где будет находитmся персонаж при переходе в следующую комнату
    public LockScript locker;//Какой замок стоит на двери

    public DoorClass(int _type, AreaClass _path, LockScript _locker)
    {
        doorType = _type; roomPath = _path; locker = _locker;
    }
}