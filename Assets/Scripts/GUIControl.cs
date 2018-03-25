using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIControl : MonoBehaviour
{


    public SceneControl scene_control = null;
    public ScoreControl score_control = null;

    //评价的文字
    private float gui_eval_scale = 1.0f;   //缩放比例
    private float gui_eval_alpha = 1.0f;

    public static float EVAL_ZOOM_TIME = 4.0f;

    //"开始"文字
    public float start_texture_x = 0.0f;
    public float start_texture_y = 50.0f;

    //评价文字
    public float defeat_base_texture_x = 0.0f;
    public float defeat_base_texture_y = 70.0f;
    public float defeat_texture_x = 70.0f;
    public float defeat_texture_y = 70.0f;
    public float eval_base_texture_x = 0.0f;
    public float eval_base_texture_y = -40.0f;
    public float eval_texture_x = 70.0f;
    public float eval_texture_y = -40.0f;
    public float total_texture_x = 0.0f;
    public float total_texture_y = 0.0f;


    public Texture defeat_base_texture = null;
    public Texture eval_base_texture = null;

    public Texture result_excellent_texture = null;
    public Texture result_good_texture = null;
    public Texture result_nomal_texture = null;
    public Texture result_bad_texture = null;
    public Texture result_mini_excellent_texture = null;
    public Texture result_mini_good_texture = null;
    public Texture result_mini_nomal_texture = null;
    public Texture result_mini_bad_texture = null;


    //"返回"的文字
    public float return_texture_x = 0.0f;
    public float return_texture_y = -150.0f;
    //------------------------------------------------------------------------//


	// Use this for initialization
	void Start ()
	{
	    this.scene_control = GetComponent<SceneControl>();
	    this.score_control = GetComponent<ScoreControl>();

        

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnGUI()
    {
        
    }
}
