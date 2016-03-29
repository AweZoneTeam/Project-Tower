using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

class ProjectTowerWindow : Editor
{


    [MenuItem("Project Tower/Ultimate Animator")]
    public static void ShowAnimatorEditor()
    {
        AnimationEditorData animData = GameObject.Find("AnimationEdit").GetComponent<AnimationEditorData>();
        AnimatorScreen animScreen = (AnimatorScreen)EditorWindow.GetWindow(typeof(AnimatorScreen));
        RightAnimator rAnimScreen = (RightAnimator)EditorWindow.GetWindow(typeof(RightAnimator));
        LeftAnimator lAnimScreen = (LeftAnimator)EditorWindow.GetWindow(typeof(LeftAnimator));
        lAnimScreen.position = new Rect(50f, 100f, 0f, 0f);
        animScreen.position = new Rect(400f, 100f, 0f, 0f);
        rAnimScreen.position = new Rect(750f, 100f, 0f, 0f);
        animScreen.maxSize = new Vector2(400f, 750f);
        animScreen.minSize = new Vector2(400f, 750f);
        rAnimScreen.maxSize = new Vector2(300f, 750f);
        rAnimScreen.minSize = new Vector2(300f, 750f);
        lAnimScreen.maxSize = new Vector2(300f, 750f);
        lAnimScreen.minSize = new Vector2(300f, 750f);
        lAnimScreen.Initialize(rAnimScreen, animScreen, animData, null, true);
        rAnimScreen.Initialize(animData, lAnimScreen, animData.animBase.usedCharacters, null);
        animScreen.focusObject = animData.gameObject;
    }

    [MenuItem("Project Tower/Ultimate Room Creator")]
    public static void ShowRoomCreator()
    {
        MainRCreatorScreen mainScreen = (MainRCreatorScreen)EditorWindow.GetWindow(typeof(MainRCreatorScreen));
        AuxRCreatorScreen auxScreen = (AuxRCreatorScreen)EditorWindow.GetWindow(typeof(AuxRCreatorScreen)); 
        mainScreen.position = new Rect(100f, 100f, 0f, 0f);
        auxScreen.position = new Rect(750f, 100f, 0f, 0f);
        mainScreen.maxSize = new Vector2(800f, 750f);
        mainScreen.minSize = new Vector2(800f, 750f);
        auxScreen.maxSize = new Vector2(300f, 750f);
        auxScreen.minSize = new Vector2(300f, 750f);
        //auxScreen.Initialize();
        //animScreen.focusObject = animData.gameObject;
    }

    [MenuItem("Project Tower/Item Creator/Create Item")]
    public static void CreateItem()
    {
        EditorWindow.GetWindow(typeof(ItemCreateWindow));
    }

    [MenuItem("Project Tower/Item Creator/Create Weapon")]
    public static void CreateWeapon()
    {
       EditorWindow.GetWindow(typeof(WeaponCreateWindow));
    }

    [MenuItem("Project Tower/Item Creator/Create Usable Item")]
    public static void CreateUsableItem()
    {
        EditorWindow.GetWindow(typeof(UseItemCreateWindow));
    }

    [MenuItem("Project Tower/Buff Creator")]
    public static void CreateBuff()
    {
        EditorWindow.GetWindow(typeof(BuffCreateWindow));
    }

    [MenuItem("Project Tower/AI/Create Behaviour")]
    public static void CreateBehaviour()
    {
        EditorWindow.GetWindow(typeof(BehaviourCreateWindow));
    }

    void OnGUI()
    {

    }
}
