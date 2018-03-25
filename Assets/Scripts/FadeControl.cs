using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeControl : MonoBehaviour
{
    private float timer;
    private float fadeTime;
    private Color colorStart;
    private Color colorTarget;



	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void fade(float time, Color start, Color target)
    {
        this.fadeTime = time;
        this.timer = 0.0f;
        this.colorStart = start;
        this.colorTarget = target;
    }
}
