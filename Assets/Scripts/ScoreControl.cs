using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreControl : MonoBehaviour
{
    //public UISprite m_Sprite1;
    //public UISprite m_Sprite10;
    //public UISprite m_Sprite100;

    private int score = 0;

    //public readonly string texNum0 = "ui_number00";
    //public readonly string texNum1 = "ui_number01";
    //public readonly string texNum2 = "ui_number02";
    //public readonly string texNum3 = "ui_number03";
    //public readonly string texNum4 = "ui_number04";
    //public readonly string texNum5 = "ui_number05";
    //public readonly string texNum6 = "ui_number06";
    //public readonly string texNum7 = "ui_number07";
    //public readonly string texNum8 = "ui_number08";
    //public readonly string texNum9 = "ui_number09";

    public UISprite[] m_Sprite;



    public readonly string[] numTex = {
        "ui_number00",
        "ui_number01",
        "ui_number02",
        "ui_number03",
        "ui_number04",
        "ui_number05",
        "ui_number06",
        "ui_number07",
        "ui_number08",
        "ui_number09"
    };

    public AudioClip CountUpSound = null;
    public AudioClip[] CountUpSounds = null;
    private int CountLevel;


    private int targetNum;
    private int currentNum;
    private float timer;

    public void AddScore(int num)
    {
        score += num;
        ShowScore();
    }

    void ShowScore()
    {
        string scoreStr = score.ToString();
        for (int i = scoreStr.Length-1; i >= 0; i--)
        {
            Debug.Log(int.Parse(scoreStr[0].ToString()));
            m_Sprite[i].spriteName = numTex[int.Parse(scoreStr[0].ToString())];
        }
       
    }
}
