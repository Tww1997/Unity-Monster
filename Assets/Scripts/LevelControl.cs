using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelControl
{

    //预设
    public GameObject OniGroupPrefab = null;

    //-----------------------------------------------------------//

    public PlayerControl player = null;
    public SceneControl scene_control = null;

    //-----------------------------------------------------------//

    //生成怪物的位置
    //玩家的X坐标如果超过这条线，则在玩家前方产生怪物
    private float oni_generate_line;

    //距离玩家前方多少米处产生怪物
    private float appear_margin = 15.0f;

    //一个分组的怪物数量
    private int oni_appear_num = 1;

    //连续成功的次数
    private int no_miss_count = 0;

    //怪物的类型
    public enum Group_Type
    {
        None = -1,

        Slow = 0,       //缓慢型
        Decelerate,     //中途加速再减速型
        Passing,        //后面超越前面型
        Rapid,          //间隔极短型

        Normal,         //普通型

        Num,
    };

    public Group_Type group_type = Group_Type.Normal;
    public Group_Type group_type_next = Group_Type.Normal;

    //判断是否产生怪物
    private bool can_dispatch = false;

    //剩下的一般产生次数
    private int normal_count = 5;

    //剩下的时间产生次数
    private int event_count = 1;

    //正在产生的事件
    private Group_Type event_type = Group_Type.None;

    //产生的位置
    private Vector3 appear_position;

    //下一个分组的速度（一般情况下）
    private float next_speed = OniGroupControl.Speed_Min * 5.0f;

    //下一个小组的生成位置（一般情况下，距离玩家的偏移值）
    private float next_line = 50.0f;

    //随机控制（游戏一般情况）
    public bool is_random = true;

    //-----------------------------------------------------------//

    public static float INTERVAL_MIN = 20.0f; //怪物出现间隔的最小值
    public static float INTERVAL_MAX = 50.0f; //怪物出现间隔的最大值

    //-----------------------------------------------------------//

    public void Create()
    {
        //为了在游戏开始后产生怪物，将生成线（主角越过就产怪）初始化设置为玩家后方
        this.oni_generate_line = this.player.transform.position.x - 1.0f;
    }

    //当玩家失误时
    public void OnPlayerMissed()
    {
        //重置怪物一次性出现的数量
        this.oni_appear_num = 1;
        //重新记录连击数
        this.no_miss_count = 0;
    }

    //控制怪物出现
    public void oniAppearControl()
    {
        //玩家每前进一定距离后，生成的怪物分组

        if (this.can_dispatch)
        {
        }
        else
        {
            if (this.Is_one_group_only())//检测是否是特别模式
            {
                //特别模式下，等待怪物从画面中消失
                if (GameObject.FindGameObjectsWithTag("OniGroup").Length == 0)
                {
                    this.can_dispatch = true;
                }
            }
            else
            {
                //普通模式下，马上生成
                this.can_dispatch = true;
            }

            if (this.can_dispatch)
            {
                //准备好出现后，通过玩家的位置计算怪物应该出现位置
                if (this.group_type_next == Group_Type.Normal)
                {
                    this.oni_generate_line = this.player.transform.position.x + this.next_line;
                }
                else if (this.group_type_next == Group_Type.Slow)
                {
                    this.oni_generate_line = this.player.transform.position.x + 50.0f;
                }
                else
                {
                    this.oni_generate_line = this.player.transform.position.x + 10.0f;
                }
            }
        }

        //玩家前进一定距离后，生成下一个分组
        do
        {
            if (this.scene_control.oni_group_num >= this.scene_control.oni_group_appear_max)
            {
                break;
            }
            if (!this.can_dispatch)
            {
                break;
            }
            if (this.player.transform.position.x <= this.oni_generate_line)
            {
                break;
            }

            this.group_type = this.group_type_next;

            switch (this.group_type)
            {
                case Group_Type.Slow:
                    {
                        this.Dispatch_slow();
                    }
                    break;
                case Group_Type.Decelerate:
                    {
                        this.Dispatch_decelerate();
                    }
                    break;
                case Group_Type.Passing:
                    {
                        this.dispatch_passing();
                    }
                    break;
                case Group_Type.Rapid:
                    {
                        this.dispatch_rapid();
                    }
                    break;
                case Group_Type.Normal:
                    {
                        this.Dispatch_normal(this.next_speed);
                    }
                    break;
            }

            
            //更新下次出现分组的怪物数量
            this.oni_appear_num++;

            this.oni_appear_num = Mathf.Min(this.oni_appear_num, SceneControl.ONI_APPEAR_NUM_MAX);

            this.can_dispatch = false;

            this.no_miss_count++;

            this.scene_control.oni_group_num++;

            if (this.is_random)
            {
                
                //选择下次出现的分组
                this.Select_next_group_type();
            }


        } while (false);

    }

    //画面上只有一个分组?
    public bool Is_one_group_only()
    {
        bool ret;
        do
        {
            ret = true;

            if (this.group_type == Group_Type.Passing || this.group_type_next == Group_Type.Passing)
            {
                break;
            }
            if (this.group_type == Group_Type.Decelerate || this.group_type_next == Group_Type.Decelerate)
            {
                break;
            }
            if (this.group_type == Group_Type.Slow || this.group_type_next == Group_Type.Slow)
            {
                break;
            }
            ret = false;
        } while (false);
        return ret;
    }

    //随机下次生成的类型
    public void Select_next_group_type()
    {
        if (this.event_type != Group_Type.None)
        {

            this.event_count--;
            if (this.event_count <= 0)
            {
                this.event_type = Group_Type.None;
                this.normal_count = Random.Range(3, 6);
            }
        }
        else
        {
            this.normal_count--;

            if (this.normal_count <= 0)
            {
                //产生随机事件
                this.event_type = (Group_Type) Random.Range(0, 4);

                switch (this.event_type)
                {
                    default:
                    case Group_Type.Decelerate:
                    case Group_Type.Passing:
                    case Group_Type.Slow:
                    {
                        this.event_count = 1;
                    }
                        break;
                    case Group_Type.Rapid:
                    {
                        this.event_count = Random.Range(2, 4);
                    }
                        break;
                }
            }
        }
    

    //生成普通分组和事件分组

            if (this.event_type == Group_Type.None)
            {
                //普通类型分组
                float rate;

                rate = (float)this.no_miss_count / 10.0f;

                rate = Mathf.Clamp01(rate);

                this.next_speed = Mathf.Lerp(OniGroupControl.Speed_Max, OniGroupControl.Speed_Min, rate);

                this.next_line = Mathf.Lerp(LevelControl.INTERVAL_MAX, LevelControl.INTERVAL_MIN, rate);

                this.group_type_next = Group_Type.Normal;
            }
            else
            {
                //事件类型的分组
                this.group_type_next = this.event_type;
            }
        }
    

    //普通模式
    public void Dispatch_normal(float speed)
    {
        appear_position = this.player.transform.position;

        //在玩家前方，稍微在画面外的位置生成
        appear_position.x += appear_margin;

        this.Create_oni_group(appear_position, speed, OniGroupControl.Type.Normal);
    }

    //缓慢型
    public void Dispatch_slow()
    {
        appear_position = this.player.transform.position;

        appear_position.x += appear_margin;

        float rate;
        rate = (float)this.no_miss_count / 10.0f;

        rate = Mathf.Clamp01(rate);

        this.Create_oni_group(appear_position, OniGroupControl.Speed_Min * 5.0f, OniGroupControl.Type.Normal);
    }

    //极短型
    public void dispatch_rapid()
    {
        appear_position = this.player.transform.position;

        appear_position.x += appear_margin;

        this.Create_oni_group(appear_position, this.next_speed, OniGroupControl.Type.Normal);

    }

    //中途减速型
    public void Dispatch_decelerate()
    {
        Vector3 appear_position = this.player.transform.position;

        appear_position.x += appear_margin;

        this.Create_oni_group(appear_position, 9.0f, OniGroupControl.Type.Decelerate);
    }

    //中途追赶超越型
    public void dispatch_passing()
    {
        float speed_low = 2.0f;
        float speed_rate = 2.0f;
        float speed_high = (speed_low - this.player.GetComponent<Rigidbody>().velocity.x)/speed_rate +
                           this.player.GetComponent<Rigidbody>().velocity.x;

        //更慢的怪物被超越时的位置
        float passing_point = 0.5f;

        Vector3 appear_position = this.player.transform.position;

        //为了让两个分组在途中重叠,调整生成的位置

        appear_position.x = this.player.transform.position.x + appear_margin;

        this.Create_oni_group(appear_position, speed_high, OniGroupControl.Type.Normal);

        appear_position.x = this.player.transform.position.x + appear_margin*Mathf.Lerp(speed_rate, 1.0f, passing_point);

        this.Create_oni_group(appear_position, speed_low, OniGroupControl.Type.Normal);
    }


    //生成怪物分组
    private void Create_oni_group(Vector3 appear_position, float speed, OniGroupControl.Type type)
    {
        //生成分组整体的碰撞器（用于碰撞检测）
        Vector3 position = appear_position;

        //生成OniGroupPrefab实例
        GameObject go = GameObject.Instantiate(this.OniGroupPrefab) as GameObject;

        OniGroupControl new_group = go.GetComponent<OniGroupControl>();

        //和地面接触的高度
        position.y = OniGroupControl.collision_size / 2.0f;

        position.z = 0.0f;

        new_group.transform.position = position;

        new_group.scene_control = this.scene_control;
        new_group.main_camera = this.scene_control.main_camera;
        new_group.player = this.player;
        new_group.run_speed = speed;
        new_group.type = type;


        //---------------------------------------------//
        //生成分组的怪物集合

        Vector3 base_position = position;

        int oni_num = this.oni_appear_num;

        //靠近碰撞盒的左端
        base_position.x -= (OniGroupControl.collision_size / 2.0f - OniControl.collision_size / 2.0f);

        //与地面接触的高度
        base_position.y = OniControl.collision_size / 2.0f;

        //生成怪物
        new_group.CreateOnis(oni_num, base_position);
    }
}
