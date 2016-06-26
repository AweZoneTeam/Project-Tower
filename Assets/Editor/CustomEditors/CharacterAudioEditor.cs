using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Редактор озвучивателя персонажей
/// </summary>
[CustomEditor(typeof(CharacterAudio))]
public class CharacterAudioEditor : Editor
{

    private Direction direction;
    private OrganismStats orgStats;
    private EnvironmentStats envStats;

    public override void OnInspectorGUI()
    {
        CharacterAudio obj = (CharacterAudio)target;

        EditorGUILayout.BeginVertical();
        {
            EditorGUILayout.LabelField("MaterialsUnderFeetSwitches");
            foreach (MaterialSwitchSoundData matSwitch in obj.materialSwitches)
            {
                EditorGUILayout.BeginHorizontal();
                {
                    matSwitch.materialName = EditorGUILayout.LayerField(matSwitch.materialName);
                    matSwitch.unitIndex = EditorGUILayout.Popup(matSwitch.unitIndex, AkWwiseProjectInfo.GetData().SwitchWwu.ConvertAll<string>(x => x.PhysicalPath).ToArray());
                    matSwitch.unitName = AkWwiseProjectInfo.GetData().SwitchWwu[matSwitch.unitIndex].PhysicalPath;
                    if (!matSwitch.unitName.Equals(string.Empty))
                    {
                        if (matSwitch.groupIndex >= AkWwiseProjectInfo.GetData().SwitchWwu[matSwitch.unitIndex].List.Count)
                        {
                            matSwitch.groupIndex = 0;
                        }
                        matSwitch.groupIndex=EditorGUILayout.Popup(matSwitch.groupIndex, AkWwiseProjectInfo.GetData().SwitchWwu[matSwitch.groupIndex].List.ConvertAll<string>(x => x.Name).ToArray());
                        AkWwiseProjectData.GroupValue switchGroup = AkWwiseProjectInfo.GetData().SwitchWwu[matSwitch.unitIndex].List[matSwitch.groupIndex];
                        string groupName = switchGroup.Name;
                        if (matSwitch.valueIndex >= switchGroup.values.Count)
                        {
                            matSwitch.valueIndex = 0;
                        }
                        matSwitch.valueIndex = EditorGUILayout.Popup(matSwitch.valueIndex, switchGroup.values.ToArray());
                        string valueName = switchGroup.values[matSwitch.valueIndex];
                        matSwitch.switchName = groupName + "/" + valueName;
                        matSwitch.SetIDs(switchGroup.ID, switchGroup.valueIDs[matSwitch.valueIndex]);
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.Space();
            if (GUILayout.Button("+Add"))
            {
                obj.materialSwitches.Add(new MaterialSwitchSoundData());
            }
        }
        EditorGUILayout.EndVertical();

    }
}