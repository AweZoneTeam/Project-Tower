using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Скрипт, прикрепляющийся к текстурируемым объектам, которым нужна коррекция масштаба текстуры.
/// </summary>
public class AutoTileScript : MonoBehaviour {

    public float ratioX = 0.1f;
    public float ratioY = 0.1f;

    [ExecuteInEditMode]
    void Start()
    {
        AutoTile();
    }

    public void AutoTile()
    {
        Vector3 scale = transform.localScale;
        float scaleX, scaleY;
        if ((scale.x > scale.z) && (scale.y > scale.z))
        {
            scaleX = transform.localScale.x;
            scaleY = transform.localScale.y;
        }
        else if ((scale.x > scale.y) && (scale.z > scale.y))
        {
            scaleX = transform.localScale.x;
            scaleY = transform.localScale.z;
        }
        else
        {
            scaleX = transform.localScale.z;
            scaleY = transform.localScale.y;
        }
        gameObject.GetComponent<Renderer>().material.SetTextureScale("_MainTex", new Vector2(scaleX*ratioX, scaleY*ratioY));
    }
}

#if UNITY_EDITOR
/// <summary>
/// ПОзволяет в редакторе производить коррекии тектуры.
/// </summary>
[CustomEditor(typeof(AutoTileScript))]
public class AutoTileEditor : Editor
{
    public override void OnInspectorGUI()
    {
        AutoTileScript aTile = (AutoTileScript)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Correct tiles of this texture"))
        {
            aTile.AutoTile();
        }
    }
}
#endif
