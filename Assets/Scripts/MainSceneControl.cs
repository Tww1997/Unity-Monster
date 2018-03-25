using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainSceneControl : MonoBehaviour {

    //淡入淡出面板
    public FadePanel m_FadePanel;
    public GameObject m_BeginBtn;

    
    public enum STEP
    {
        NONE = -1,

        TITLE = 0,
        WAIT_SE_END,
        FADE_WAIT,

        NUM
    };

    private STEP step = STEP.NONE;
    private STEP next_step = STEP.NONE;
    private float step_timer = 0.0f;
    
    //淡入淡出时间
    private static float FADE_TIME = 1.5f;

	// Use this for initialization
	void Start () {
        //不允许玩家操作
        PlayerControl player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        player.UnPlayable();

        m_FadePanel.FadeIn(FADE_TIME);

        this.next_step = STEP.TITLE;
	}
	
	// Update is called once per frame
	void Update () {
        this.step_timer += Time.deltaTime;

        //
        switch (this.step)
        {
            case STEP.TITLE:
                if (Input.GetMouseButton(0))
                {
                    
                    
                        //m_BeginBtn.GetComponent<UIButtonScale>().enabled = false;
                        m_BeginBtn.GetComponent<BoxCollider>().enabled = false;
                        this.next_step = STEP.WAIT_SE_END;
                  
                    
                   
                }
                break;
            case STEP.WAIT_SE_END:
                {
                    //播放声音，结束后播放淡入淡出
                    bool to_finish = true;
                    do
                    {
                        if (!this.GetComponent<AudioSource>().isPlaying)
                        {
                            break;
                        }
                        if (this.GetComponent<AudioSource>().time >= this.GetComponent<AudioSource>().clip.length)
                        {
                            break;
                        }
                        to_finish = false;  
                    } while (false);

                    if (to_finish)
                    {
                        m_FadePanel.FadeOut(FADE_TIME);
                        this.next_step = STEP.FADE_WAIT;
                    }
                }
                break;
            case STEP.FADE_WAIT:
                {
                    //淡入淡出结束，载入游戏场景
                    if (!this.m_FadePanel.isActive)
                    {
                        SceneManager.LoadScene(1);
                    }
                }
                break;
            default:
                break;
        }

        if (this.next_step != STEP.NONE)
        {
            switch (this.next_step)
            {
                case STEP.WAIT_SE_END:
                    {
                        this.GetComponent<AudioSource>().Play();
                    }
                    break;
                default:
                    break;
            }

            this.step = this.next_step;
            this.next_step = STEP.NONE;

            this.step_timer = 0.0f;
        }
    }




    GameObject LoadResources(string path)
    {
        GameObject obj = Resources.Load(path) as GameObject;
        return obj;
    }
}
