using UnityEngine;
using System.Collections;
using UnityEditor;

/// <summary>
/// Окно создания нового игрового предмета (который можно будет отобразить в инвентаре)
/// </summary>
public class ItemCreateWindow : EditorWindow
{
    public string itemName = "New Item";

    void OnGUI()
    {
        itemName = EditorGUILayout.TextField(itemName);
        if (GUILayout.Button("Create New"))
        {
            CreateNewItem();
        }
    }

    //Создаём новый предмет.
    private void CreateNewItem()
    {
        ItemClass asset = ScriptableObject.CreateInstance<ItemClass>();
        asset.itemName = itemName;
        asset.type = "item";
        AssetDatabase.CreateAsset(asset, "Assets/Database/Items/" + itemName  + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}

/// <summary>
/// Окно создания новой базы данных по оружию
/// </summary>
public class WeaponCreateWindow : EditorWindow
{
    public string weaponName = "New Weapon";

    void OnGUI()
    {
        weaponName = EditorGUILayout.TextField(weaponName);
        if (GUILayout.Button("Create New"))
        {
            CreateNewWeapon();
        }
    }

    //Создаём новое оружие.
    private void CreateNewWeapon()
    {
        WeaponClass asset = ScriptableObject.CreateInstance<WeaponClass>();
        asset.itemName = weaponName;
        asset.type = "weapon";
        AssetDatabase.CreateAsset(asset, "Assets/Database/Items/Weapons/" + weaponName + ".asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}
