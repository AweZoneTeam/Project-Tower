using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;


/// <summary>
/// Список действий, совершаемых передвижными механизмами
/// </summary>
public class MoveablePlatformActions : InterObjActions, ILeverActivated
{

    #region epsilons

    protected const float moveEps = .2f;

    #endregion //epsilons

    #region parametres

    protected bool activated = false;
    [SerializeField]protected bool loop;

    [SerializeField] protected float speed=4f;

    protected float distanceToTarget = 0f;
    protected bool onMove=false;

    #endregion //parametres

    #region fields

    protected int currentPoint = 0;
    public List<Vector3> relMovPoints=new List<Vector3>();//Относительные смещения, что будет совершать объект при активации

    #endregion //fields

    /// <summary>
    /// Передвижение объектов осуществляется здесь
    /// </summary>
    public void Update()
    {
        if (activated)
        {
            if (!onMove)
            {
                if (currentPoint == relMovPoints.Count)
                {
                    if (loop)
                    {
                        currentPoint = 0;
                    }
                    else
                    {
                        activated = false;//Есть вероятность, что это надо будет убрать
                        return;
                    }
                }
                distanceToTarget = relMovPoints[currentPoint].magnitude;
                onMove = true;
            }
            else
            {
                Vector3 direction = relMovPoints[currentPoint].normalized;
                if (distanceToTarget <= moveEps)
                {
                    transform.position += direction* distanceToTarget;
                    onMove = false;
                    currentPoint++;
                }
                else
                {
                    transform.Translate(direction * Time.deltaTime * speed);
                    distanceToTarget -= Time.deltaTime * speed;
                }
            }
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        currentPoint = 0;
    }

    /// <summary>
    /// Функция, которую вызовет нажатие на тот рычаг, к которому подключен сундук
    /// </summary>
    public void LeverActivation()
    {
        activated = !activated;
    }

}
