using UnityEngine;
using System.Collections;

/// <summary>
/// Скрипт, контролирующий положение теней, а также их степень затемнения (в зависимости от окружающих источников света)
/// </summary>
public class ShadowScript : MonoBehaviour {

    #region consts
    const float maxHeight = 30f;//На какой максимальной высоте объект над землёй ещё будет формировать тень. 
    const float xScale = 7.698193f;//Изначальные размеры тени по оси x
    const float yScale = 2.323381f;//и по оси y
    const float offsetY = 1.5f;//Насколько высоко нужно поднять тень над землёй
    #endregion //consts

    public float minHeight = 0f;//Расстояние от объекта до тени при нахождении объекта на земле
    private float y;//На какой высоте рисуется тень
    private float offsetX, offsetZ;//Относительное смещение тени от отбрасываемого тень объекта
    private float radius = 1f;

    public Transform charPosition;

	void Start ()
    {
        offsetX = transform.position.x - charPosition.position.x;
        offsetZ = transform.position.z - charPosition.position.z;
        y = transform.position.y;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.position = new Vector3(charPosition.position.x+offsetX, y, charPosition.position.z+offsetZ);
        float newRadius;
        if (charPosition.transform.position.y - transform.position.y < maxHeight-minHeight)
        {
            newRadius = 0.01f + radius * (1f - (charPosition.transform.position.y - transform.position.y) / (maxHeight - minHeight));
        }
        else
        {
            newRadius = 0.01f;
        }
        transform.localScale = new Vector3(xScale * newRadius, yScale * newRadius, 1f);
	}

    public void SetY(float _y)
    {
        y = _y + offsetY;
    }
}
