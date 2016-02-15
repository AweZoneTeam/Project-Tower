/* 
 * Created by Alex 'Extravert' Kasaurov
 * From  xgm.guru community
 */
using UnityEditor;
using UnityEngine;

namespace XGM.GURU
{
    [CustomEditor(typeof(WallConstructor))]
    public class WallConstructorEditor : Editor
    {
        public bool showGeneral = true, showArc = true, showLine = true;

        [MenuItem("GameObject/Create Advanced/Wall Constructor", false, 0)]
        public static void CreateWallGenerator()
        {
            Selection.activeObject = new GameObject("Wall").AddComponent<WallConstructor>();
        }

        public WallConstructor o { get { return (WallConstructor)target; } }

        public void OnEnable()
        {
            while (o.transform.childCount < 2)
                new GameObject((o.transform.childCount + 1).ToString()).AddComponent<WallPoint>().transform.parent = o.transform;
            o.GetComponent<MeshFilter>().hideFlags = HideFlags.HideInInspector;
            o.GetComponent<MeshRenderer>().hideFlags = HideFlags.HideInInspector;
        }

        public override void OnInspectorGUI()
        {
            showGeneral = DrawTitle("Основные настройки", showGeneral);
            if (showGeneral)
            {
                EditorGUI.BeginChangeCheck();
                var h = Mathf.Max(EditorGUILayout.FloatField("Высота стены", o.height), 0.001f);
                if (EditorGUI.EndChangeCheck() || h != o.height)
                {
                    o.height = h;
                    EditorUtility.SetDirty(o);
                }

                EditorGUI.BeginChangeCheck();
                var d = Mathf.Max(EditorGUILayout.FloatField("Толщина стены", o.deep), 0.001f);
                if (EditorGUI.EndChangeCheck() || d != o.deep)
                {
                    o.deep = d;
                    EditorUtility.SetDirty(o);
                }
                var r = o.GetComponent<MeshRenderer>();
                if (r.sharedMaterials.Length != 2)
                {
                    r.sharedMaterials = new Material[2];
                }

                EditorGUI.BeginChangeCheck();
                var m1 = EditorGUILayout.ObjectField("Материал сбоку", r.sharedMaterials[0], typeof (Material), false) as Material;
                if (EditorGUI.EndChangeCheck() || m1 != r.sharedMaterials[0])
                    r.sharedMaterials = new[] {m1, r.sharedMaterials[1]};

                EditorGUI.BeginChangeCheck();
                var m2 = EditorGUILayout.ObjectField("Материал сверху", r.sharedMaterials[1], typeof(Material), true) as Material;
                if (EditorGUI.EndChangeCheck() || m2 != r.sharedMaterials[1])
                    r.sharedMaterials = new[] { r.sharedMaterials[0], m2 };

                EditorGUI.BeginChangeCheck();
                var uv = Mathf.Max(EditorGUILayout.FloatField("Размер развертки", o.uvSize), 0f);
                if (EditorGUI.EndChangeCheck() || uv != o.uvSize)
                {
                    o.uvSize = uv;
                    EditorUtility.SetDirty(o);
                }
            }
            showArc = DrawTitle("Кривые", showArc);
            if (showArc)
            {
                EditorGUI.BeginChangeCheck();
                var s = Mathf.Max(EditorGUILayout.FloatField("Сглаживание", o.smoothDistance), 0.01f);
                if (EditorGUI.EndChangeCheck() || s != o.smoothDistance)
                {
                    o.smoothDistance = s;
                    EditorUtility.SetDirty(o);
                }
            }
            if (GUILayout.Button(!o.updateMode ? "Начать редактирование" : "<color=red>Закончить редактирование</color>", o.updateMode  ? new GUIStyle("button") { richText = true, fontStyle = FontStyle.Bold} : new GUIStyle("button")))
                o.updateMode = !o.updateMode;

            if (o.updateMode)
                EditorGUILayout.HelpBox("Когда вы закончите редактирование меша - не забудьте выключить режим редактирования", MessageType.Info);
            EditorGUILayout.HelpBox("Не забудьте удалить компонент после создания финальной версии меша", MessageType.Warning);
            if (GUILayout.Button("Удалить компонент"))
                D();
        }

        public void D()
        {
            o.GetComponent<MeshFilter>().hideFlags = HideFlags.None;
            o.GetComponent<MeshRenderer>().hideFlags = HideFlags.None;
            while (o.transform.childCount != 0)
                DestroyImmediate(o.transform.GetChild(0).gameObject);
            DestroyImmediate(o);
        }

        private bool DrawTitle(string title, bool toggle)
        {
            return GUILayout.Toggle(toggle, title, "IN Title", GUILayout.ExpandWidth(true));
        }
    }
}