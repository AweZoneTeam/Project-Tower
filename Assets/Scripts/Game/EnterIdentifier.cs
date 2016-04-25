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
            if (!enters.Contains(enter))
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
            if (enters.Contains(enter))
            {
                enters.Remove(enter);
                if (enters.Count > 0)
                {
                    ChangeRoom(enters[0].nextRoom);
                }
            }
        }
    }

    public void ChangeRoom(AreaClass nextRoom)
    {
        Transform parent = transform.parent.parent;
        Vector3 pos = parent.position;
        cam.ChangeRoom(GameStatistics.currentArea,nextRoom);
        parent.position = new Vector3(pos.x, pos.y, nextRoom.id.coordZ);
        parent.GetComponent<KeyboardActorController>().currentRoom = nextRoom;
        SpFunctions.ChangeRoomData(nextRoom);
    }

}
