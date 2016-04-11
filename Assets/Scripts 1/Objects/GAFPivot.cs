using UnityEngine;
using System.Collections;
using GAF.Core;
using GAF.Data;

/// <summary>
/// А это мной (Матвеем) созданный класс, нужный для удерживания лиц у головы, да и вообще для создания дочерних анимаций. Во как!
/// </summary>
public class GAFPivot: MonoBehaviour
{
    public Vector3 pivotPosition;
    public Vector3 offset;

    public void SetPosition(Vector3 _pos, GAFObjectStateData _State)
    {

        transform.localPosition=offset+ _pos;  
        Matrix4x4 _transform = Matrix4x4.identity;
        _transform[0, 0] = _State.a;
        _transform[0, 1] = -_State.c;
        _transform[1, 0] = -_State.b;
        _transform[1, 1] = _State.d;
        _transform[2, 3] = 0;
        Vector3 vect = _transform.MultiplyPoint3x4(new Vector3(0f,1f,0f));
        float angle = Vector3.Angle(transform.up, vect);
        transform.RotateAround(new Vector3(0f, 0f, 1f), angle);
    }

}
