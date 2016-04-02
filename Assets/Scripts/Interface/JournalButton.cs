using UnityEngine;
using System.Collections;

/// <summary>
/// Класс, используемый кнопками в журнале
/// </summary>
public class JournalButton : MonoBehaviour
{
    public JournalData jData;

    public void SetData()
    {
        JournalWindow journal = GameObject.FindGameObjectWithTag(Tags.gameController).GetComponent<InterfaceController>().journal;
        journal.SetData(jData);
    }

}
