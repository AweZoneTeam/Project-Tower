using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой вид иникаторов, которые детектируют наличие проходов - объектов, связывающих между собой комнаты.
/// </summary>
public class EnterIdentifier : MonoBehaviour
{

    public List<EnterClass> enters;

    private CameraController cam;

    public void Awake()
    {
        cam = GameObject.FindGameObjectWithTag(Tags.cam).GetComponent<CameraController>();
    }


    public void OnTriggerEnter(Collider other)
    {
        if (string.Equals(other.gameObject.tag, Tags.enter))
        {
            EnterClass enter = other.gameObject.GetComponent <EnterClass>();
            bool k = true;
            for (int i = 0; i < enters.Count; i++)
            {
                if (enters[i] == enter)
                {
                    k = false;
                    break;
                }
            }
            if (k)
            {
                enters.Add(enter);
                if (GameStatistics.currentArea != enter.nextRoom)
                {
                    ChangeRoom(enter.nextRoom);
                }
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (string.Equals(other.gameObject.tag, Tags.enter))
        {
            EnterClass enter = other.gameObject.GetComponent<EnterClass>();
            bool k = false;
            for (int i = 0; i < enters.Count; i++)
            {
                if (enters[i] == enter)
                {
                    k = true;
                    break;
                }
            }
            if (k)
            {
                enters.Remove(enter);
                if (enters.Count > 0)
                {
                    if (GameStatistics.currentArea == enter.nextRoom)
                    {
                        ChangeRoom(enter.nextRoom);
                    }
                }
            }
        }
    }

    public void ChangeRoom(AreaClass nextRoom)
    {
        SpFunctions.ChangeRoomData(nextRoom);
        cam.ChangeRoom();
    }

}
