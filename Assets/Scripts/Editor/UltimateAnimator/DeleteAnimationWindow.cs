using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

public class DeleteAnimationWindow : EditorWindow
{
    private GameObject character;

    public List<animList> animTypes = new List<animList>();

    private Vector2 scrollPosition = Vector2.zero;

    public void Initialize(GameObject c)
    {
        character = c;

        animTypes = character.GetComponent<InterObjAnimator>().animTypes;
    }

    public void OnGUI()
    {
        if (character != null)
        {
            scrollPosition = GUI.BeginScrollView(new Rect(0f, 0f, 300f, 100f), scrollPosition, new Rect(0f, 0f, 300f, 800f));
            {
                for (int i = 0; i < animTypes.Count; i++)
                {
                    if (GUILayout.Button(animTypes[i].typeName))
                    {
                        DeleteAnimationType(i);
                    }
                    for (int j = 0; j < animTypes[i].animations.Count; j++)
                    {
                        if (GUILayout.Button(animTypes[i].animations[j]))
                        {
                            DeleteAnimation(i,j);
                        }
                    }
                }
            }
        }
    }

    void DeleteAnimationType(int type)
    {
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            if (cAnim.parts[i].interp.animTypes.Count > type)
            {
                cAnim.parts[i].interp.animTypes.RemoveAt(type);
            }
        }
        cAnim.animTypes.RemoveAt(type);
        for (int i = 0; i < cAnim.animBase.Count; i++)
        {
            if (cAnim.animBase[i].type == type)
            {
                cAnim.animBase.RemoveAt(i);
                i--;
            }
        }
    }

    void DeleteAnimation(int type, int numb)
    {
        InterObjAnimator cAnim = character.GetComponent<InterObjAnimator>();
        for (int i = 0; i < cAnim.parts.Count; i++)
        {
            cAnim.parts[i].interp.animTypes[type].animInfo.RemoveAt(numb);
        }
        cAnim.animTypes[type].animations.RemoveAt(numb);
        for (int i = 0; i < cAnim.animBase.Count; i++)
        {
            if ((cAnim.animBase[i].type == type)&&(cAnim.animBase[i].numb==numb))
            {
                cAnim.animBase.RemoveAt(i);
                break;
            }
        }
    }

}
