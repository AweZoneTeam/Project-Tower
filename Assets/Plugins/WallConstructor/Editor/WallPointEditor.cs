/* 
 * Created by Alex 'Extravert' Kasaurov
 * From  xgm.guru community
 */
using UnityEditor;
using UnityEngine;

namespace XGM.GURU
{
    [CustomEditor(typeof(WallPoint))]
    public class WallPointEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            var o = (WallPoint) target;
            var transformParent = o.transform.parent;

            EditorGUI.BeginChangeCheck();
            var ho = EditorGUILayout.FloatField("Смещение высоты", o.heightOffset);
            if (EditorGUI.EndChangeCheck() || ho != o.heightOffset)
            {
                o.heightOffset = ho;
                transformParent.GetComponent<WallConstructor>().Calculate();
                SceneView.RepaintAll();
            }

            
            int index = -1;
            for (int i = 0; i < transformParent.childCount; i++)
            {
                if (transformParent.GetChild(i) == o.transform)
                    index = i;
            }
            var prevPoint = index != 0 ? transformParent.GetChild(index - 1).GetComponent<WallPoint>() : null;
            var nextPoint = index != transformParent.childCount-1 ? transformParent.GetChild(index + 1).GetComponent<WallPoint>() : null;
            var notCurveCenter = (prevPoint == null || nextPoint == null || prevPoint.isCurveCenter || nextPoint.isCurveCenter) ;
            if (notCurveCenter && o.isCurveCenter)
                o.isCurveCenter = false;

            if (!notCurveCenter)
            {
                GUILayout.Label(o.isCurveCenter ? "Эта точка является центром кривой" : "Это обычная точка прямой");
                if (GUILayout.Button(o.isCurveCenter ? "Сделать обычной точкой" : "Сделать центром кривой"))
                {
                    o.isCurveCenter = !o.isCurveCenter;
                    transformParent.GetComponent<WallConstructor>().Calculate();
                    SceneView.RepaintAll();
                }
            }

            if (nextPoint == null)
            {
                if (GUILayout.Button("Продлить стену"))
                {
                    int ind;
                    if (!int.TryParse(o.name, out ind))
                    {
                        Debug.LogError("Имя объекта нельзя представить числом, это плохо");
                        ind = 665;
                    }

                    var a = new GameObject((ind + 1).ToString()).AddComponent<WallPoint>();
                    a.transform.parent = transformParent;
                    a.transform.position = o.transform.position;
                    Selection.activeObject = a.gameObject;
                }
            }

            if (nextPoint == null)
                EditorGUILayout.HelpBox("Эта точка является конечной - вы можете создать для нее продолжение",
                    MessageType.Info);
            else if (prevPoint == null)
                EditorGUILayout.HelpBox(
                    "Эта точка является начальной - вы не можете произвести с ней какие-либо операции", MessageType.Info);
            else if (prevPoint.isCurveCenter || nextPoint.isCurveCenter)
                EditorGUILayout.HelpBox("Эта точка имеет среди соседей центр кривой и не может быть обработана",
                    MessageType.Info);
            else
                EditorGUILayout.HelpBox("Эта точка имеет соседов по бокам, и может быть преобразована в центр кривой",
                    MessageType.Info);
        }
    }
}
