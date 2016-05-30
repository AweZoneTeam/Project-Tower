using UnityEngine;
using System.Collections;

/// <summary>
/// Простейший набор простейших функций, которые может исполнять CAV-объект
/// </summary>
public class InterObjActions : MonoBehaviour
{

    #region fields
    protected Direction direction;
    protected InterObjVisual anim;
    protected InterObjController interactor;
    #endregion //fields

    public virtual void Initialize()
    {
        anim = transform.GetComponentInChildren<InterObjVisual>();
    }


    /// <summary>
    /// Установить ориентацию
    /// </summary>
    /// <param name="_direction"></param>
    public void SetDirection(Direction _direction)
    {
        direction = _direction;
    }

    #region Interface

    /// <summary>
    /// Произвести взаимодействие
    /// </summary>
    public virtual void Interact()
    {
    }

    /// <summary>
    /// Установить набор предметов, используемых интерактивным объектом
    /// </summary>
    public virtual void SetBag(BagClass _chestClass)
    { }

    /// <summary>
    /// Установить, кто взаимодействуетс интерактивным объектом в данный момент
    /// </summary>
    public virtual void SetInteractor(InterObjController _interactor)
    {
        interactor = _interactor;
    }

    /// <summary>
    /// Функция, обычно вызываемая при нажатии на объект другим объектом
    /// </summary>
    public virtual void OnPushDown()
    {
    }

    /// <summary>
    /// Функция, обычно вызываемая, когда на объекте кто-то стоит
    /// </summary>
    public virtual void OnPush()
    {
    }

    /// <summary>
    /// Функция, обычно вызываемая, когда кто-то сошёл с объекта
    /// </summary>
    public virtual void OnPushUp()
    {
    }

    #endregion //Interface

}
