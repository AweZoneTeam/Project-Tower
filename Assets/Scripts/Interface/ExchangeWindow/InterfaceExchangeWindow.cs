using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Окно обмена предметами между двумя объектами класса BagClass
/// </summary>
public class InterfaceExchangeWindow : InterfaceWindow
{
    public BagClass leftBag, rightBag;
    public List<GameObject> leftBagObj, rightBagObj;


    public override void Initialize()
    {
        leftBag = new BagClass();
        rightBag = new BagClass();
        leftBagObj = new List<GameObject>();
        rightBagObj = new List<GameObject>();
        Transform leftBagTrans = transform.FindChild("LeftBag");
        Transform rightBagTrans = transform.FindChild("RightBag");
        for (int i = 0; i < leftBagTrans.GetChildCount(); i++)
        {
            leftBagObj.Add(leftBagTrans.GetChild(i).gameObject);
        }
        for (int i = 0; i < rightBagTrans.GetChildCount(); i++)
        {
            rightBagObj.Add(rightBagTrans.GetChild(i).gameObject);
        }
    }

    public void Start()
    {
        Initialize();
    }

    public void SetImages()
    {
        for (int i = 0; i < leftBag.bag.Count; i++)
        {
            leftBagObj[i].GetComponent<Image>().sprite = leftBag.bag[i].item.image;
        }
        for (int i = 0; i < rightBag.bag.Count; i++)
        {
            rightBagObj[i].GetComponent<Image>().sprite = rightBag.bag[i].item.image;
        }
    }

    public void Exit()
    {
        gameObject.GetComponent<Canvas>().enabled=false;
    }
}
