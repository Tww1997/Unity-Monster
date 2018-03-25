using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{

    private ScoreControl scoreControl;

    //---------------------------------------------------------------//
    //预设

    public GameObject OniGroupPrefab = null;
    public GameObject OniPrefab = null;
    public GameObject OniEmitterPrefab = null;
    public GameObject OniYamaPrefab;


    //SE
    public AudioClip GameStart = null;
    public AudioClip EvalSound = null;              //评价
    public AudioClip ReturnSound = null;            //返回

    //---------------------------------------------------------------//

    //玩家
    public PlayerControl player = null;

    //摄像机
    public GameObject main_camera = null;

    //控制怪物的出现
    public LevelControl level_control = null;

    //控制得分计算
    //

    //用于在得分时从上方落下怪物的对象
    //



    //淡入淡出控制




    //---------------------------------------------------------------//

    //游戏进行的状态
    public enum Step
    {
        None = -1,

        Start,                  //"开始"的文字出现
        Game,                   //游戏进行中
        Oni_vanish_wait,        //游戏结束后，等待画面上所有怪物消失
        Last_run,               //怪物不再出现后的状态
        Player_stop_wait,       //等待玩家停止

        Goal,                   //得分
        Oni_fall_wait,          //等待“怪物从上方落下”过程结束
        Result_defeat,          //显示斩杀数量的评价
        Result_evaluation,      //显示击倒时机的评价
        Result_total,           // 综合评价

        Game_over,              //游戏结束
        Goto_title,             //迁移到标题

        Num,
    };


    public Step step = Step.Game;       //当前游戏的进行状态
    public Step next_step = Step.None;  //迁移状态？
    public float step_timer = 0.0f;     //状态迁移后的经过时间？
    public float step_timer_prev = 0.0f;

    //---------------------------------------------------------------//

    //从点击按钮后到攻击命中经历的时间（用于成绩评价）
    public float attack_time = 0.0f;

    //评价
    //斩杀怪物时靠的越近得分越高
    public enum EVALUATION
    {
        NONE = -1,

        OKEY = 0,
        GOOD,
        GREAT,

        MISS,

        NUM,
    };

    public static string[] evaluation_str =
    {
        "okay",
        "good",
        "great",
        "miss",
    };

    public EVALUATION evaluation = EVALUATION.NONE;

    //---------------------------------------------------------------//

    //游戏整体的结果
    public struct Result
    {
        public int oni_defeat_num;
        public int[] eval_count;

        public int rank;

        public float score;
        public float score_max;
    };

    public Result result;

    //---------------------------------------------------------------//

    //每次出现怪物的最大值，如果不失误则一直增加，但不会超过该数值
    public static int ONI_APPEAR_NUM_MAX = 10;

    //游戏结束时怪物的分组数量
    public int oni_group_appear_max = 50;

    //失败时减少的怪物出现数量
    public static int ONI_GROUP_PENALTY = 1;

    //隐藏得分时的出现数量
    public static float SCORE_HIDE_NUM = 40;

    //出现小组的数量
    public int oni_group_num = 0;

    //击中or发生接触的怪物分组数量
    public int oni_group_complite = 0;

    //击中的怪物分组数量
    public int oni_group_defeat_num = 0;

    //发生接触的怪物分组数量
    public int oni_group_miss_num = 0;

    //游戏开始后“开始”文字出现的时间长度
    private static float START_TIME = 0.0f;

    //显示得分时，“怪物堆积的位置”和“玩家停下的位置”之间的距离
    private static float GOAL_STOP_DISTANCE = 8.0f;

    //对操作进行评价时，按钮按下后到攻击命中所经过的时间标准
    public static float ATTACK_TIME_GREAT = 0.05f;
    public static float ATTACK_TIME_GOOD = 0.10f;



    //---------------------------------------------------------------//
    // 如果设置为true ，倒下的怪物将按照摄像机的本地坐标系移动
    // 因为和摄像机一起发生连动，即使摄像机突然停下来，也不会有不自然的感觉
    //
    public static bool IS_ONI_BLOWOUT_CAMERA_LOCAL = true;

    //讨伐数消失瞬间的讨伐数
    private int backup_oni_defeat_num = -1;


    public float eval_rate_okey = 1.0f;
    public float eval_rate_good = 2.0f;
    public float eval_rate_great = 4.0f;
    public int eval_rate = 1;

    // Use this for initialization
    void Start()
    {
        //查找玩家的实例
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        //查找摄像机的实例对象
        this.main_camera = GameObject.FindGameObjectWithTag("MainCamera");

        this.level_control = new LevelControl();
        this.level_control.scene_control = this;
        this.level_control.player = this.player;
        this.level_control.OniGroupPrefab = this.OniGroupPrefab;
        this.level_control.Create();


        scoreControl = GetComponent<ScoreControl>();


        //清空游戏的结果
        this.result.oni_defeat_num = 0;
        this.result.eval_count = new int[(int)EVALUATION.NUM];
        this.result.rank = 0;
        this.result.score = 0;
        this.result.score_max = 0;

        for (int i = 0; i < this.result.eval_count.Length; i++)
        {
            this.result.eval_count[i] = 0;
        }

        //this.GetComponent

        //直接游戏
        this.step = Step.Game;
    }

    // Update is called once per frame
    void Update()
    {

        //管理游戏的当前状态
        this.step_timer_prev = this.step_timer;
        this.step_timer += Time.deltaTime;


        //检测是否迁移到下一个状态
        switch (this.step)
        {
            case Step.Start:
                {
                    if (this.step_timer > SceneControl.START_TIME)
                    {
                        next_step = Step.Game;
                    }
                }
                break;
            case Step.Game:
                {
                    if (this.oni_group_complite >= this.oni_group_appear_max)
                    {
                        next_step = Step.Oni_vanish_wait;
                    }
                    if (this.oni_group_complite >= SCORE_HIDE_NUM && this.backup_oni_defeat_num == -1)
                    {
                        this.backup_oni_defeat_num = this.result.oni_defeat_num;
                    }
                }
                break;
            case Step.Oni_vanish_wait:
                {
                    do
                    {
                        if (GameObject.FindGameObjectsWithTag("OniGroup").Length > 0)
                        {
                            break;
                        }
                        if (this.player.GetSpeedRate() < 0.5f)
                        {
                            break;
                        }
                        next_step = Step.Last_run;
                    } while (false);
                }
                break;

            case Step.Last_run:
                {
                    if (this.step_timer > 2.0f)
                    {
                        next_step = Step.Player_stop_wait;
                    }
                }
                break;
            case Step.Player_stop_wait:
                {
                    //if (this.player.IsStopped())
                    //{
                        UIManager.Instance.m_ExitBtn.SetActive(true);
                        if (Input.GetMouseButton(0))
                        {
                            UIManager.Instance.m_ExitBtn.GetComponent<BoxCollider>().enabled =false;
                            this.GetComponent<AudioSource>().PlayOneShot(this.ReturnSound);
                            next_step = Step.Game_over;
                        }
                    //}
                }
                break;
            #region
            //case Step.Goal:
            //    {
            //        this.next_step = Step.Oni_fall_wait;
            //    }
            //    break;

            //case Step.Oni_fall_wait:
            //    {
            //        this.next_step = Step.Result_defeat;
            //    }
            //    break;

            //case Step.Result_defeat:
            //    {
            //        if (this.step_timer >= 0.4f && this.step_timer_prev < 0.4f)
            //        {
            //            this.GetComponent<AudioSource>().PlayOneShot(this.EvalSound);
            //        }
            //        if (this.step_timer > 0.5f)
            //        {
            //            this.next_step = Step.Result_evaluation;
            //        }
            //    }
            //    break;

            //case Step.Result_evaluation:
            //    {
            //        if (this.step_timer >= 0.4f && this.step_timer_prev < 0.4f)
            //        {
            //            this.GetComponent<AudioSource>().PlayOneShot(this.EvalSound);
            //        }

            //        if (this.step_timer > 2.0f)
            //        {
            //            this.next_step = Step.Result_total;
            //        }
            //    }
            //    break;
            //case Step.Result_total:
            //    {
            //        if (this.step_timer >= 0.4f && this.step_timer_prev < 0.4f)
            //        {
            //            this.GetComponent<AudioSource>().PlayOneShot(this.EvalSound);
            //        }
            //        if (this.step_timer > 2.0f)
            //        {
            //            this.next_step = Step.Game_over;
            //        }
            //    }
            //    break;
            #endregion
            case Step.Game_over:
                {
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
                        //淡出效果
                        UIManager.Instance.m_FadePanel.FadeOut();
                        this.next_step = Step.Goto_title;
                    }
                }
                break;

            case Step.Goto_title:
                {
                    if (!UIManager.Instance.m_FadePanel.isActive)
                    {
                    SceneManager.LoadScene(0);
                    }
                }
                break;
        }

        //状态变化时的初始化处理
        if (this.next_step != Step.None)
        {
            switch (this.next_step)
            {
                case Step.Player_stop_wait:
                    {
                        //Debug.Log("!!!!");
                        //使玩家停下来
                        this.player.StopRequest();
                    }
                    break;

            }
            this.step = this.next_step;
            this.next_step = Step.None;

            this.step_timer = 0.0f;
            this.step_timer_prev = -1.0f;

        }

        //各个状态的执行处理

        switch (this.step)
        {
            case Step.Game:
                {
                    this.level_control.oniAppearControl();
                }
                break;
        }
    }

    //玩家失败时的处理
    public void OnPlayerMissed()
    {
        this.oni_group_miss_num++;
        this.oni_group_complite++;
        this.oni_group_appear_max -= ONI_GROUP_PENALTY;

        this.level_control.OnPlayerMissed();

        this.evaluation = EVALUATION.MISS;

        this.result.eval_count[(int)this.evaluation]++;

        GameObject[] oni_groups = GameObject.FindGameObjectsWithTag("OniGroup");

        foreach (var oni_group in oni_groups)
        {
            this.oni_group_num--;
            oni_group.GetComponent<OniGroupControl>().BeginLeave();
        }

        this.next_step = Step.Last_run;
    }

    //追加被击倒的怪物数量
    public void AddDefeatNum(int num)
    {
        this.oni_group_defeat_num++;
        this.oni_group_complite++;
        this.result.oni_defeat_num += num;

        this.attack_time = this.player.GetComponent<PlayerControl>().GetAttackTimer();

        if (this.evaluation == EVALUATION.MISS)
        {
            this.evaluation = EVALUATION.OKEY;
        }
        else
        {
            if (this.attack_time < ATTACK_TIME_GREAT)
            {
                this.evaluation = EVALUATION.GREAT;
            }
            else if (this.attack_time < ATTACK_TIME_GOOD)
            {
                this.evaluation = EVALUATION.GOOD;
            }
            else
            {
                this.evaluation = EVALUATION.OKEY;
            }
        }

        //计算得分
        //togo
        //float[] score_list = { this.eval_rate_okey, this.eval_rate_good, this.eval_rate_great, 0 };
        //this.result.score_max += num * this.eval_rate_great;
        //this.result.score += num * score_list[(int)this.evaluation];

        scoreControl.AddScore(num);
    }
}
