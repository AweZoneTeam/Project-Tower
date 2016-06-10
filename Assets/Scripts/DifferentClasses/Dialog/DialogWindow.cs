using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class DialogWindow : MonoBehaviour
{
    #region WindowElements
    Text text;
    GameObject textBlock, answerBlock;
    Button[] buttons = new Button[5];
    Image player, NPCimg;
    #endregion//WindowElements

    //нпс с которым в данный момент говорим
    NPCActions NPC;


    void Awake()
    {
        text = GameObject.Find("TextDialog").GetComponent<Text>();
        for(int i=1; i<5; i++)
        {
            buttons[i - 1] = GameObject.Find("Button" + i).GetComponent<Button>();
        }
        buttons[4] = GameObject.Find("Button0").GetComponent<Button>();
        player = GameObject.Find("Left Person").GetComponent<Image>();
        NPCimg = GameObject.Find("Right Person").GetComponent<Image>();
        textBlock = GameObject.Find("Text Block");
        answerBlock = GameObject.Find("Answer Block");
    }

    public void Init(NPCActions _NPC)
    {
        NPC = _NPC;
        gameObject.GetComponent<Canvas>().enabled = true;
        NextSpeech(NPC.speeches[0]);
        SpFunctions.Pause("dialog");
    }

    public void NextSpeech(NPCSpeech nextSpeech)
    {
        if (nextSpeech.SpeechJournalEvent != null)
        {
            nextSpeech.SpeechJournalEvent.Invoke(this, new JournalEventArgs());
        }
        if (nextSpeech.NPCFace != null)
        {
            NPCimg.sprite = nextSpeech.NPCFace;
        }
        if (nextSpeech.playerFace != null)
        {
            player.sprite = nextSpeech.playerFace;
        }
        #region TextBlock
        if (nextSpeech.text != "")
        {
            answerBlock.active = false;
            textBlock.active = true;
            text.text = nextSpeech.text;
            if (nextSpeech.nextSpeech!=null)
            {
                buttons[4].onClick.RemoveAllListeners();
                buttons[4].onClick.AddListener(delegate { NextSpeech(nextSpeech.nextSpeech); });
            }
            else
            {
                buttons[4].onClick.AddListener(delegate { Deinit(); });
            }
        }
        #endregion//TextBlock
        #region AnswerBlock
        else
        {
            answerBlock.active = true;
            textBlock.active = false;
            for (int i = 0; i < 4; i++)
            {
                if (nextSpeech.answers.Count > i)
                {
                    buttons[i].gameObject.active = true;
                    buttons[i].GetComponentInChildren<Text>().text = nextSpeech.answers[i].text;
                    buttons[i].onClick.RemoveAllListeners();
                }
                else
                {
                    buttons[i].gameObject.active = false;
                }
            }
            #region SLOZHNA
            //наверное ты спросишь: "нахуя? где цикл?"
            //но так надо
            if (nextSpeech.answers.Count > 0)
            {
                if (nextSpeech.answers[0].nextSpeech != null)
                {
                    buttons[0].onClick.AddListener(() => NextSpeech(nextSpeech.answers[0].nextSpeech));
                }
                else
                {
                    buttons[0].onClick.AddListener(() => Deinit());
                }
            }
            if (nextSpeech.answers.Count > 1)
            { 
                if (nextSpeech.answers[1].nextSpeech != null)
                {
                    buttons[1].onClick.AddListener(() => NextSpeech(nextSpeech.answers[1].nextSpeech));
                }
                else
                {
                    buttons[1].onClick.AddListener(() => Deinit());
                }
            }
            if (nextSpeech.answers.Count > 2)
            {
                if (nextSpeech.answers[2].nextSpeech != null)
                {
                    buttons[2].onClick.AddListener(() => NextSpeech(nextSpeech.answers[2].nextSpeech));
                }
                else
                {
                    buttons[2].onClick.AddListener(() => Deinit());
                }
            }
            if (nextSpeech.answers.Count > 3)
            {
                if (nextSpeech.answers[3].nextSpeech != null)
                {
                    buttons[3].onClick.AddListener(() => NextSpeech(nextSpeech.answers[3].nextSpeech));
                }
                else
                {
                    buttons[3].onClick.AddListener(() => Deinit());
                }
            }
            #endregion//SLOZHNA
        }
        #endregion//AnswerBlock

    }



    public void Deinit()
    {
        gameObject.GetComponent<Canvas>().enabled = false;
        NPC = null;
        SpFunctions.Pause("dialog");
    }
}
