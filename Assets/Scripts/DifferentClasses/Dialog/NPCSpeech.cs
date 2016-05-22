using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Текст, который выдаёт нпс
/// </summary>
public class NPCSpeech : ScriptableObject
{
    [TextArea(3, 10)]
    public string text;
    public Sprite NPCFace, playerFace;
    public List<PlayerSpeech> answers;
    public NPCSpeech nextSpeech;
    public EventHandler<JournalEventArgs> SpeechJournalEvent;
}

/// <summary>
/// Вариант ответа игрока
/// </summary>
[System.Serializable]
 public class PlayerSpeech
{
    public string text;
    public NPCSpeech nextSpeech;
}

