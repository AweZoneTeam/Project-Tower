using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class DeletePartWindow : EditorWindow
{

    private GameObject character;

    private List<PartController> parts = new List<PartController>();

    private Vector2 scrollPosition = Vector2.zero;

    public void Initialize(GameObject c)
    {
        character = c;
        parts = character.GetComponent<CharacterAnimator>().parts;
    }

    public void OnGUI()
    {
        if (character != null)
        {
            scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, 300f, 100f), scrollPosition, new Rect(0f, 0f, 300f, 400f));
            {
                for (int i = 0; i < parts.Count; i++)
                {
                    if (GUILayout.Button(parts[i].gameObject.name))
                    {
                        DeletePart(parts[i]);    
                    }
                }
            }
            GUI.EndScrollView();
        }
    }

    /// <summary>
    /// Процесс удаления анимационной части
    /// </summary>
    void DeletePart(PartController part)
    {
        parts.Remove(part);
        for (int i = 0; i < parts.Count; i++)
        {
            if (parts[i].childParts != null)
            {
                if (parts[i].childParts.Contains(part))
                {
                    parts[i].childParts.Remove(part);
                }
            }
        }
        DestroyImmediate(part.gameObject);
    }

}
