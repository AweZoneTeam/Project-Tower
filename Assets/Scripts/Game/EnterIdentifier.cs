using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Класс, представляющий собой вид иникаторов, которые детектируют наличие проходов - объектов, связывающих между собой комнаты.
/// </summary>
public class EnterIdentifier : MonoBehaviour
{

    [SerializeField]protected List<EnterClass> enters=new List<EnterClass>();

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
                    ChangeRoom(enter);

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
                    ChangeRoom(enters[0]);
                }
            }
        }
    }

    /// <summary>
    /// Перейти в комнату, в которую ведёт проход 
    /// </summary>
    public void ChangeRoom(EnterClass _enter)
    {
        Transform parent = transform.parent.parent;
        Vector3 pos = parent.position;
        PersonController person = parent.GetComponent<PersonController>();
        if (_enter.nextRoom != null)
        {
            if (person is KeyboardActorController)
            {
                cam.ChangeRoom(GameStatistics.currentArea, _enter.nextRoom);
                _enter.OnEnterUse(new JournalEventArgs());
                SpFunctions.ChangeRoomData(_enter.nextRoom);
            }
            parent.position = new Vector3(pos.x, pos.y, _enter.nextRoom.id.coordZ);
            person.ChangeRoom(_enter.nextRoom);
            if (_enter.subRoom != null)
            {
                if (person is KeyboardActorController)
                {
                    cam.ChangeRoom(GameStatistics.currentArea, _enter.subRoom);
                    SpFunctions.ChangeRoomData(_enter.subRoom);
                }
                parent.position = new Vector3(pos.x, pos.y, _enter.subRoom.id.coordZ);
                person.ChangeRoom(_enter.subRoom);
            }
        }
    }

}
