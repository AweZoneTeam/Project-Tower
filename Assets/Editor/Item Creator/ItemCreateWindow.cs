using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

/// <summary>
/// Окно создания нового игрового предмета (который можно будет отобразить в инвентаре)
/// </summary>
public class ItemCreateWindow : EditorWindow
{
    protected string itemName = "New Item";
    protected int partsNumb = 0;
    protected float x = 0f, y = 0f, z = 0f;

    protected virtual void OnGUI()
    {
        itemName = EditorGUILayout.TextField(itemName);
        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.TextField("Parts Number");
            partsNumb = EditorGUILayout.IntField(partsNumb);
        }
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        {
            x = EditorGUILayout.FloatField(x);
            y = EditorGUILayout.FloatField(y);
            z = EditorGUILayout.FloatField(z);
        }
        EditorGUILayout.EndHorizontal();
        if (GUILayout.Button("Create New"))
        {
            CreateNewItem();
        }
    }

    //Создаём новый предмет.
    protected virtual void CreateNewItem()
    {
        Initialize();
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
    }

    protected virtual void Initialize()
    {
        ItemClass asset = ScriptableObject.CreateInstance<ItemClass>();
        asset.itemName = itemName;
        asset.type = "item";
        asset.itemVisuals = new List<ItemVisual>();
        for (int i = 0; i < partsNumb; i++)
        {
            asset.itemVisuals.Add(new ItemVisual(new Vector3(x, y, z)));
        }
        AssetDatabase.CreateAsset(asset, "Assets/Database/Items/" + itemName + ".asset");
        Selection.activeObject = asset;
    }
}

/// <summary>
/// Окно создания новой базы данных по оружию
/// </summary>
public class WeaponCreateWindow : ItemCreateWindow
{
    protected string weaponType = "sword";

    protected override void OnGUI()
    {
        base.OnGUI();
        weaponType = EditorGUILayout.TextField(weaponType);
    }

    protected override void Initialize()
    {
		ItemClass asset;
		if(weaponType == "bow")
		{
			asset = ScriptableObject.CreateInstance<Bow>();
		}
		else if(weaponType == "shield")
		{
			asset = ScriptableObject.CreateInstance<Shield>();
		}
		else
		{
			asset = ScriptableObject.CreateInstance<SimpleWeapon>();
		}
        asset.itemName = itemName;
        asset.type = "weapon";
        asset.itemVisuals = new List<ItemVisual>();
        for (int i = 0; i < partsNumb; i++)
        {
            asset.itemVisuals.Add(new ItemVisual(new Vector3(x, y, z)));
        }
		if(asset is WeaponClass)
		{
			((WeaponClass)asset).weaponType = weaponType;
		}
        AssetDatabase.CreateAsset(asset, "Assets/Database/Items/Weapons/" + itemName + ".asset");
        Selection.activeObject = asset;
    }
}

/// <summary>
/// Окно создания новой базы данных по доспехам
/// </summary>
public class ArmorCreateWindow : ItemCreateWindow
{
    public string armorType = "helmet";


    protected override void OnGUI()
    {
        base.OnGUI();
        armorType = EditorGUILayout.TextField(armorType);
    }

    protected override void Initialize()
    {
        ArmorClass asset = ScriptableObject.CreateInstance<ArmorClass>();
        asset.itemName = itemName;
        asset.type = "armor";
        ArmorClass armor = (ArmorClass)asset;
        armor.armorType = armorType;
        armor.itemVisuals = new List<ItemVisual>();
        for (int i = 0; i < partsNumb; i++)
        {
            armor.itemVisuals.Add(new ItemVisual(new Vector3(x, y, z)));
        }
        AssetDatabase.CreateAsset(asset, "Assets/Database/Items/Armor/" + itemName + ".asset");
        AssetDatabase.SaveAssets();
    }
}

/// <summary>
/// Окно создания новой базы данных по одноразовому используемому предмет
/// </summary>
public class UseItemCreateWindow : ItemCreateWindow
{
    public string useItemType = "potion";

    protected override void OnGUI()
    {
        base.OnGUI();
        useItemType = EditorGUILayout.TextField(useItemType);
    }

    //Создаём новый юзабельный предмет.
    protected override void CreateNewItem()
    {
        UsableItemClass asset = ScriptableObject.CreateInstance<UsableItemClass>();
        asset.itemName = itemName;
        asset.itemVisuals = new List<ItemVisual>();
        for (int i = 0; i < partsNumb; i++)
        {
            asset.itemVisuals.Add(new ItemVisual(new Vector3(x, y, z)));
        }
        asset.type = "usable";
        UsableItemClass useItem = (UsableItemClass)asset;
        useItem.useItemType = useItemType;
        AssetDatabase.CreateAsset(asset, "Assets/Database/Items/UsableItems/" + itemName + ".asset");
        Selection.activeObject = asset;
    }
}