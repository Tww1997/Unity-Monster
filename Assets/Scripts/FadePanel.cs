using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadePanel : MonoBehaviour {


    public GameObject mask = null;
    public bool isActive = true;

    private void Awake()
    {
        mask = transform.Find("Mask").gameObject as GameObject;

    }


    public void Init()
    {
        mask = transform.Find("Mask").gameObject as GameObject;
    }

    /// <summary>
    /// 淡入
    /// </summary>
    /// <param name="duration"></param>
    public void FadeIn(float duration = 1.5f)
    {
        //Init();
        mask.SetActive(true);
        TweenAlpha ta = mask.GetComponent<TweenAlpha>();
        isActive = true;
        ta.enabled = true;
        ta.from = 1;
        ta.to = 0;
       
        ta.duration = duration;
        ta.ResetToBeginning();
        ta.PlayForward();
       
    }

    /// <summary>
    /// 淡出
    /// </summary>
    public void FadeOut(float duration = 1.5f)
    {
        mask.SetActive(true);
        TweenAlpha ta = mask.GetComponent<TweenAlpha>();
        isActive = true;
        ta.enabled = true;
        ta.from = 0f;
        ta.to = 1f;
        ta.duration = duration;
        ta.ResetToBeginning();
        ta.PlayForward();
        ta.onFinished.Clear();
        EventDelegate.Add(ta.onFinished, OnFinshed);

    }


    void OnFinshed()
    {
        isActive = false;
    }


}
