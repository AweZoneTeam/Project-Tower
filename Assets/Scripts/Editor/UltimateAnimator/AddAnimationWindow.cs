using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

//Это окно просто добавляет новую анимацию во все части перонажа сразу.
public class AddAnimationWindow : EditorWindow 
{
	public RightAnimator rightAnim;
	public LeftAnimator leftAnim;
	public GameObject character;

	public string typeName, animName;
	public int type=0, numb=0;

	//Инициализация
	public void Initialize(RightAnimator ra, LeftAnimator la, GameObject c)
	{
		rightAnim = ra;
		leftAnim = la;
		character = c;
	}

	//Что мы видим
	public void OnGUI()
	{
		if (character != null) {
			typeName = EditorGUILayout.TextField (typeName);
			animName = EditorGUILayout.TextField (animName);
			type = EditorGUILayout.IntField (type);
			CharacterAnimator cAnim = character.GetComponent<CharacterAnimator> ();
			numb = EditorGUILayout.IntField (numb);
			if (type < 0) {
				type *= -1;
			}
			if (numb < 0) {
				numb *= -1;
			}
			if (cAnim.animTypes.Count > 0) {
				if (type > cAnim.animTypes.Count) {
					type = cAnim.animTypes.Count;
				}
				if (type < cAnim.animTypes.Count) {
					if (numb > cAnim.animTypes [type].animations.Count) {
						numb = cAnim.animTypes [type].animations.Count;
					}
				}
			} 
			else {
				type = 0;
				numb = 0;
			}
			if (GUILayout.Button ("Add Animation")) {
				AddAnimation ();
			}
		}
	}

	//Что происходит при нажатии на кнопку
	public void AddAnimation()
	{
		//Сначала говорим аниматору, что него появилась новая анимация
		CharacterAnimator cAnim = character.GetComponent<CharacterAnimator> ();
        List<animList> animTypes = cAnim.animTypes;
        Dictionary<string, AnimClass> animBase = cAnim.animBase;
		if (type == cAnim.animTypes.Count) {
			animTypes.Add (new animList (typeName, animName));
            AddAnimBase(animBase, animTypes, cAnim.animNames, animName, type, numb);
        } 
		else {
			if (numb != cAnim.animTypes [type].animations.Count) {
				animTypes [type].animations.Insert (numb, animName);
                AddAnimBase(animBase, animTypes, cAnim.animNames, animName, type, numb);
            } 
			else {
				animTypes [type].animations.Add (animName);
                AddAnimBase(animBase, animTypes, cAnim.animNames, animName, type, numb);
            }
		}
		rightAnim.animTypes = cAnim.animTypes;
		//Потом говорим это всем частям персонажа
		for (int k = 0; k < cAnim.parts.Count; k++) {
			if (type == cAnim.parts [k].interp.animTypes.Count) {
				numb = 0;
				cAnim.parts [k].interp.animTypes.Add (new animationInfoTypes (typeName, animName));
			}
			else {
				if (numb != cAnim.parts [k].interp.animTypes [type].animInfo.Count) {
					cAnim.parts [k].interp.animTypes [type].animInfo.Insert (numb, new animationInfo (animName));
				} else {
					cAnim.parts [k].interp.animTypes [type].animInfo.Add (new animationInfo (animName));
				}
			}
		}
	}

    void AddAnimBase(Dictionary<string,AnimClass> animBase, List<animList> animTypes,List<string> animNames, string animName, int type, int numb)
    {
        if (animBase == null)
        {
            animBase = new Dictionary<string, AnimClass>();
        }
        if (animNames == null)
        {
            animNames = new List<string>();
        }
        animBase.Add(animName, new AnimClass(type, numb));
        animNames.Add(animName);
        RefreshAnimBase(animBase, animTypes);
    }

    void RefreshAnimBase(Dictionary<string, AnimClass> animBase, List<animList> animTypes)
    {
        for (int i = 0; i < animTypes.Count; i++)
        {
            for (int j = 0; j < animTypes[i].animations.Count; j++)
            {
                string anim = animTypes[i].animations[j];
                animBase[anim].type = i;
                animBase[anim].numb = j;
            }
        }
    }

}
