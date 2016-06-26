using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Класс, представляющий собой масляную дорожку.
/// </summary>
public class OilTraceActions: FlammableActions
{

    #region consts

    protected const float relScaleX = 3f;

    #endregion //consts

    #region fields

    public GameObject flame;//Какой объект представляет собой пламя, которым горит масло
    public FlameTrace flameTrace=null;//Точки возгарания дорожки из масла

    #endregion //fields

    #region parametres

    public float spreadTime = .5f;//Время распространения огня с одной точки на другую
    public float burnTime = 10f;//Время горения
    public float deltaX = 12f;

    #endregion //parametres

    public override void Initialize()
    {
        base.Initialize();
        if (flameTrace == null? true : flameTrace.flamePoints.Count==0)
        {
            CreateFlameTrace();
            burned = false;
        }
        else
        {
            burned = true;
        }
    }

    /// <summary>
    /// Сформировать точки возгорания
    /// </summary>
    protected void CreateFlameTrace()
    {
        flameTrace = new FlameTrace();
        flameTrace.Add(CreateFlamePoint(transform.position.x));
        float x = 0;
        while (x < Mathf.Abs(relScaleX * transform.lossyScale.x / 2))
        {
            x += deltaX;
            flameTrace.Insert(0, CreateFlamePoint(transform.position.x - x));
        }
        x = 0;
        while (x < Mathf.Abs(relScaleX * transform.lossyScale.x / 2))
        {
            x += deltaX;
            flameTrace.Add(CreateFlamePoint(transform.position.x + x));
        }
        flameTrace.ReinitializeFlamePoints(this);
    }

    protected FlamePoint CreateFlamePoint(float x)
    {
        GameObject flamePoint = new GameObject("FlamePoint");
        flamePoint.AddComponent<FlamePoint>();
        flamePoint.GetComponent<FlamePoint>().SetPoint(x);
        flamePoint.transform.position = new Vector3(x, transform.position.y, transform.position.z);
        transform.parent = transform;
        return flamePoint.GetComponent<FlamePoint>();
    }

    /// <summary>
    /// Произвести действие, связанное с возгоранием объекта
    /// </summary>
    public override void BurnAction()
    {
        base.BurnAction();
        FlamePoint nextPoint = flameTrace.flamePoints[flameTrace.flamePoints.Count / 2];
        GameObject _flame = Instantiate(flame, new Vector3(nextPoint.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
        nextPoint.Burn(spreadTime,burnTime);
    }

    /// <summary>
    /// Произвести действие, связанное с возгоранием объекта
    /// </summary>
    public override void BurnAction(Vector3 flamePosition)
    {
        base.BurnAction(flamePosition);
        FlamePoint nextPoint = flameTrace.GetNearestPoint(flamePosition.x);
        GameObject _flame = Instantiate(flame, new Vector3(nextPoint.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
        nextPoint.Burn(spreadTime, burnTime);
    }

    /// <summary>
    /// Функция распространения огня на ближайшие флэймпоинты
    /// </summary>
    public void SpreadFire(FlamePoint point)
    {
        if (flameTrace.Contains(point))
        {
            FlamePoint nextPoint;
            int index = flameTrace.flamePoints.IndexOf(point);
            if (index + 1 < flameTrace.flamePoints.Count)
            {
                nextPoint = flameTrace.flamePoints[index + 1];
                if (!nextPoint.burned)
                {
                    GameObject _flame = Instantiate(flame, new Vector3(nextPoint.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
                    nextPoint.Burn(spreadTime, burnTime);
                }
            }
            if (index - 1 >=0)
            {
                nextPoint = flameTrace.flamePoints[index -1];
                if (!nextPoint.burned)
                {
                    GameObject _flame = Instantiate(flame, new Vector3(nextPoint.x, transform.position.y, transform.position.z), Quaternion.identity) as GameObject;
                    nextPoint.Burn(spreadTime, burnTime);
                }
            }
        }
    }

    /// <summary>
    /// Функция сгорания флэймпоинта
    /// </summary>
    public void OilIsBurnt(FlamePoint point)
    {
        if (flameTrace.Contains(point))
        {
            int index = flameTrace.flamePoints.IndexOf(point);
            if ((index == flameTrace.flamePoints.Count)|| (index == 0))
            {
                flameTrace.Remove(point);
                Destroy(point.gameObject);
                FormOilTrace(flameTrace.flamePoints.Count + 1);
            }
            else
            {
                int count = flameTrace.flamePoints.Count;
                OilTraceActions trace1 = Instantiate(this, transform.position,transform.rotation) as OilTraceActions;
                OilTraceActions trace2 = Instantiate(this, transform.position, transform.rotation) as OilTraceActions;
                trace1.GetComponent<InterObjController>().RegisterObject(GetComponent<InterObjController>().GetRoomPosition(), true);
                trace2.GetComponent<InterObjController>().RegisterObject(GetComponent<InterObjController>().GetRoomPosition(), true);
                trace1.flameTrace = new FlameTrace();
                for (int i = 0; i < index; i++)
                {
                    trace1.flameTrace.Add(flameTrace.flamePoints[i]);
                    flameTrace.flamePoints[i].transform.parent = trace1.transform;
                }
                trace1.FormOilTrace(count);
                trace1.flameTrace.ReinitializeFlamePoints(trace1);

                trace2.flameTrace = new FlameTrace();
                for (int i = index+1; i < count; i++)
                {
                    trace2.flameTrace.Add(flameTrace.flamePoints[i]);
                    flameTrace.flamePoints[i].transform.parent = trace2.transform;
                }
                trace2.FormOilTrace(count);
                trace2.flameTrace.ReinitializeFlamePoints(trace2);

                Destroy(point.gameObject);
                GetComponent<InterObjController>().DestroyInterObj();
            }
        }
    }

    /// <summary>
    /// Сформировать масляной трек, используя количество имеющихся флэймпоинтов и флэймпоинтов родителя
    /// </summary>
    /// <param name="_trace"></param>
    public void FormOilTrace(int _traceCount)
    {
        Vector3 pos = transform.position;
        if (flameTrace.flamePoints.Count > 0)
        {
            int index = flameTrace.flamePoints.Count / 2;
            if (flameTrace.flamePoints.Count % 2 == 1)
            {
                transform.position = new Vector3(flameTrace.flamePoints[index].x, pos.y, pos.z);
            }
            else
            {
                transform.position = new Vector3(flameTrace.flamePoints[index - 1].x / 2f + flameTrace.flamePoints[index].x / 2f, pos.y, pos.z);
            }
            float ratio = flameTrace.flamePoints.Count * 1f / _traceCount;
            pos = transform.localScale;
            transform.localScale = new Vector3(pos.x * ratio, pos.y, pos.z);
            if (transform.childCount > 0)
            {
                Transform child = null;
                Vector3 pos1;
                for (int i = 0; i < 2; i++)
                {
                    child = transform.FindChild("OilTrail"+i.ToString());
                    if (child != null)
                    {
                        pos = child.localScale;
                        child.localScale = new Vector3(pos.x / ratio, pos.y, pos.z);
                        pos1 = child.localPosition;
                        child.localPosition = new Vector3(pos1.x + Mathf.Sign(pos.x) * Mathf.Abs(pos.x - child.localScale.x), pos1.y, pos1.z);
                    }
                }
            }
        }
        else
        {
            GetComponent<InterObjController>().DestroyInterObj();
        }
    }

}

/// <summary>
/// Класс, используемый для создания пути из огня и для учёта затухания этого огня
/// </summary>
[System.Serializable]
public class FlameTrace
{
    public List<FlamePoint> flamePoints = new List<FlamePoint>();

    public FlameTrace()
    { }

    public FlameTrace(List<FlamePoint> _flamePoints, OilTraceActions _oilTrace)
    {
        flamePoints = _flamePoints;
        ReinitializeFlamePoints(_oilTrace);
    }

    public void ReinitializeFlamePoints(OilTraceActions _oilTrace)
    {
        foreach (FlamePoint _flamePoint in flamePoints)
        {
            _flamePoint.OilTrace = _oilTrace;
        }
    }

    public void Add(FlamePoint _flamePoint)
    {
        flamePoints.Add(_flamePoint);
    }

    public void Insert(int index, FlamePoint _flamePoint)
    {
        flamePoints.Insert(index, _flamePoint);
    }

    public void Remove(FlamePoint _flamePoint)
    {
        flamePoints.Remove(_flamePoint);
    }

    public bool Contains(FlamePoint _flamePoint)
    {
        return flamePoints.Contains(_flamePoint);
    }

    public FlamePoint GetNearestPoint(float _x)
    {
        float distance = float.PositiveInfinity;
        FlamePoint aim = null;
        foreach (FlamePoint flamePoint in flamePoints)
        {
            if (Mathf.Abs(flamePoint.x - _x) < distance)
            {
                aim = flamePoint;
                distance = Mathf.Abs(flamePoint.x - _x);
            }
            /*else
            {
                break;
            }*/
        }
        return aim;
    }

}
