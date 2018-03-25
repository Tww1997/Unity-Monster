using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    //----------------------------------------------------------------//

    //声音
    public AudioClip[] AttackSound;     //攻击时的声音
    public AudioClip SwordSound;        //挥剑的声音
    public AudioClip SwordHitSound;     //剑的声音（挥动声音，击中的声音
    public AudioClip MissSound;         //失败时的声音
    public AudioClip runSound;

    public AudioSource attack_voice_audio;
    public AudioSource sword_audio;
    public AudioSource miss_audio;
    public AudioSource run_audio;

    public int attack_sound_index = 0; //下一次的攻击声音

    //----------------------------------------------------------------//

    //移动速度
    private float run_speed = 5f;

    //移动速度的最大值
    public static float Run_speed_max = 20.0f;

    //移动速度的加速值
    private const float run_speed_add = 5.0f;
    
    //移动速度的减速值
    private const float run_speed_sub = 5.0f*4.0f;

    //用于攻击判定的碰撞器
    private AttackColliderControl attack_collider = null;

    public SceneControl scene_control = null;

    //判断攻击判定进行中的计时器
    //如果attack_timer > 0.0f表示中
    private float attack_timer = 0.0f;

    //攻击落空后无法攻击计时器
    //如果attack_disable_timer > 0.0f则无法攻击
    private float attack_disable_timer = 0.0f;

    //攻击判定的持续时间
    private static float ATTACK_TIME = 0.3f;

    //攻击判定的持续时间
    private static float ATTACK_DISABLE_TIME = 1.0f;

    private bool is_runnig = true;

    private bool is_contact_floor = false;

    //玩家是否能操作
    private bool is_playable = true;

    //停止的目标位置
    //(在SceneControl.cs中决定，这里是希望停下的位置
    public float stop_position = -1.0f;

    //攻击动作的种类
    public enum Attack_Motion
    {
        None = -1,

        Right = 0,
        Left,

        Num
    };

    public Attack_Motion attack_motion = Attack_Motion.Left;

    //剑的轨迹特效
    public AnimatedTextureExtendedUV kisski_left = null;
    public AnimatedTextureExtendedUV kisski_right = null;

    //击中时的特效
    public ParticleSystem fx_hit = null;

    //奔跑时的特效
    public ParticleSystem fx_run = null;

    //
    public float min_rate = 0.0f;
    public float max_rate = 1.0f;

    private Vector3 new_velocity;

    //----------------------------------------------------------------//

    public enum Step
    {
        None = -1,

        Run = 0,
        Stop,
        Miss,
        Num
    };

    public Step step = Step.None;
    public Step next_step = Step.None;

    //----------------------------------------------------------------//

	// Use this for initialization
	void Start ()
	{
	    //Vector3 new_velocity = this.GetComponent<Rigidbody>().velocity;

        //查找用于攻击判定的碰撞器
	    this.attack_collider = GameObject.FindGameObjectWithTag("AttackCollider").GetComponent<AttackColliderControl>();

        //设置用于攻击判定的碰撞器的玩家实例
	    this.attack_collider.player = this;

        //剑的轨迹特效
	    this.kisski_left = GameObject.FindGameObjectWithTag("FX_Kiseki_L").GetComponent<AnimatedTextureExtendedUV>();
	    this.kisski_left.stopPlay();

	    this.kisski_right = GameObject.FindGameObjectWithTag("FX_Kiseki_R").GetComponent<AnimatedTextureExtendedUV>();
	    this.kisski_right.stopPlay();

        //击中时的特效
	    this.fx_hit = GameObject.FindGameObjectWithTag("FX_Hit").GetComponent<ParticleSystem>();

	    this.fx_run = GameObject.FindGameObjectWithTag("FX_Run").GetComponent<ParticleSystem>();
        //

	    this.run_speed = 0.0f;

	    this.next_step = Step.Run;

	    this.attack_voice_audio = this.gameObject.AddComponent<AudioSource>();
	    this.sword_audio = this.gameObject.AddComponent<AudioSource>();
	    this.miss_audio = this.gameObject.AddComponent<AudioSource>();

	    this.run_audio = this.gameObject.AddComponent<AudioSource>();
	    this.run_audio.clip = this.runSound;
	    this.run_audio.loop = true;
	    this.run_audio.Play();


	}
	
	// Update is called once per frame
	void Update ()
	{

	    min_rate = Mathf.Clamp(min_rate, 0.0f, max_rate);
	    max_rate = Mathf.Clamp(max_rate, min_rate, 1.1f);

	    if (this.next_step == Step.None)
	    {
	        switch (this.step)
	        {
                case Step.Run:
	            {
	                if (!this.is_runnig)
	                {
	                    if (this.run_speed <= 0.0f)
	                    {
                            //当速度为零时,停止播放
	                        this.fx_run.Stop();

	                        this.next_step = Step.Stop;
	                    }
	                }
	            }
                break;

                case Step.Miss:
	            {
	                if (this.GetComponent<Rigidbody>().velocity.y < 0.0f)
	                {
	                    if (this.is_contact_floor)
	                    {
	                        this.fx_run.Play();

	                        this.GetComponent<Rigidbody>().useGravity = true;
	                        this.next_step = Step.Run;
	                    }
	                }
	            }
	                break;
	        }
	    }

        //状态迁移时的初始化
	    if (this.next_step != Step.None)
	    {
	        switch (this.next_step)
	        {
                case Step.Stop:
	            {
	                Animation animation = this.transform.GetComponentInChildren<Animation>();

	                animation.Play("P_stop");
	            }
	                break;

                case Step.Miss:
	            {
	                Vector3 velocity = this.GetComponent<Rigidbody>().velocity;

	                float jump_height = 1.0f;

	                velocity.x = -2.5f;
	                velocity.y = Mathf.Sqrt(2.0f*9.8f*jump_height);
	                velocity.z = 0.0f;

	                this.GetComponent<Rigidbody>().velocity = velocity;
	                this.GetComponent<Rigidbody>().useGravity = false;

	                this.run_speed = 0.0f;

	                Animation animation = this.transform.GetComponentInChildren<Animation>();

	                animation.Play("P_yarare");
	                animation.CrossFadeQueued("P_run");

                    //

	                this.miss_audio.PlayOneShot(this.MissSound);

                    //停止播放奔跑的特效
	                this.fx_run.Stop();
	            }
	                break;
	        }

	        this.step = this.next_step;

            this.next_step = Step.None;
	    }

	    if (is_runnig)
	    {
	        this.run_audio.volume = 1.0f;
	    }
	    else
	    {
            //声音淡出？？？
	        this.run_audio.volume = Mathf.Max(0.0f, this.run_audio.volume - 0.05f);
	    }

        //各个状态的执行
        
        //----------------------------------------------------------------------------//
        //位置

	    switch (this.step)
	    {
           //速度
            case Step.Run:
	        {
	            if (this.is_runnig)
	            {
                    //跑的时候逐渐加速
	                this.run_speed += PlayerControl.run_speed_add*Time.deltaTime;
	            }
	            else
	            {
                    //不跑的时候逐渐减速（不包括Miss状态？
	                this.run_speed -= PlayerControl.run_speed_sub*Time.deltaTime;
	            }
                //控制速度上限
	            this.run_speed = Mathf.Clamp(this.run_speed, 0.0f, PlayerControl.Run_speed_max);

	            Vector3 new_velocity = this.GetComponent<Rigidbody>().velocity;

	            new_velocity.x = run_speed;

                //控制主角y轴不变，不往y轴方向移动
	            if (new_velocity.y > 0.0f)
	            {
	                new_velocity.y = 0.0f;
	            }
                //给主角速度
                this.GetComponent<Rigidbody>().velocity = new_velocity;

	            float rate;

	            rate = this.run_speed/PlayerControl.Run_speed_max;
	            this.run_audio.pitch = Mathf.Lerp(min_rate, max_rate, rate);

                //---------------------------------------------------------------//
                //攻击

	            this.Attack_Control();

	            this.Sword_Fx_Control();
                
                //--------------------------------------------------------------//

	        }
	            break;
            case Step.Miss:
	        {
	            this.GetComponent<Rigidbody>().velocity += Vector3.down*9.8f*2.0f*Time.deltaTime;
	        }
            break;
	    }
	    
        //
	    this.is_contact_floor = false;
	}





    void OnCollisionStay(Collision other)
    {
        //和怪物接触后，减速

        if (other.gameObject.tag == "OniGroup")
        {
            if (this.step != Step.Miss)
            {
                this.next_step = Step.Miss;
                
                //玩家和怪物接触时的处理

                this.scene_control.OnPlayerMissed();

                //记录下怪物组和玩家发生了接触

                OniGroupControl oni_group = other.gameObject.GetComponent<OniGroupControl>();

                oni_group.OnPlayerHitted();

            }
        }
        //是否着陆了？
        if (other.gameObject.tag == "Floor")
        {
            is_runnig = true;
        }
    }

    //有时候CollisionStay 也可能不被调用，这里提前设定好
    void OnCollisionEnter(Collision other)
    {
        this.OnCollisionStay(other);
    }

    // -------------------------------------------------------------------------------- //

    //播放攻击命中的特效
    public void PlayHitEffect(Vector3 position)
    {
        this.fx_hit.transform.position = position;

        this.fx_hit.Play();
    }

    //播放攻击命中的声音
    public void PlayHitSound()
    {
        this.sword_audio.PlayOneShot(this.SwordHitSound);
    }

    //重置 “无法攻击”计时器（立刻可以攻击
    public void ReseAttackDisableTimer()
    {
        this.attack_disable_timer = 0.0f;
    }

    //算出从开始攻击（点击鼠标按键起）经过时间
    public float GetAttackTimer()
    {
        return (PlayerControl.ATTACK_TIME - this.attack_timer);
    }

    //获取外加的速度率（0.0f~1.0f）
    public float GetSpeedRate()
    {
        float player_speed_rate = Mathf.InverseLerp(0.0f, PlayerControl.Run_speed_max,
            this.GetComponent<Rigidbody>().velocity.magnitude);
        return (player_speed_rate);
    }

    //停止
    public void StopRequest()
    {
        this.is_runnig = false;
    }

    //允许玩家操作
    public void Playable()
    {
        this.is_playable = true;
    }

    //禁止玩家操作
    public void UnPlayable()
    {
        this.is_playable = false;
    }

    //玩家是否停止
    public bool IsStopped()
    {
        bool is_stopped = false;

        do
        {
            if (this.is_runnig)
            {
                break;
            }
            if (this.run_speed > 0.0f)
            {
                break;
            }

            is_stopped = true;
        } while (false);
        return (is_stopped);
    }

    // 持续减速时，算出预计的停止位置
    public float CalcDistanceToStop()
    {
        float distance = this.GetComponent<Rigidbody>().velocity.sqrMagnitude/(2.0f*PlayerControl.run_speed_sub);

        return (distance);
    }

    //-----------------------------------------------------------------------//

    //是否有攻击的输入
    private bool Is_attack_input()
    {
        bool is_attacking = false;

        // 如果点击鼠标左键，攻击
        //
        // OnMouseDown() 只有在对象自身被点击时才会被调用
        // 因为这里我们需要在画面的任何位置被点击时都会有反应，
        // 使用Input.GetMouseButtonDown() 
        //

        if (Input.GetMouseButtonDown(0))
        {
            is_attacking = true;
        }

        return (is_attacking);
    }


    //攻击控制
    private void Attack_Control()
    {
        if (!this.is_playable)
        {
            return;
        }

        if (this.attack_timer > 0.0f)
        {
            //正在进行攻击判定

            this.attack_timer -= Time.deltaTime;

            //攻击判定结束检测
            if (this.attack_timer <= 0f)
            {
                //使碰撞器（攻击成功否）的碰撞判断无效
                attack_collider.SetPowered(false);
            }
        }
        else
        {
            this.attack_disable_timer -= Time.deltaTime;

            if (this.attack_disable_timer >= 0.0f)
            {
                //依旧不可攻击
            }
            else
            {
                this.attack_disable_timer = 0.0f;
                if (this.Is_attack_input())
                {
                    //使碰撞器的攻击判定有效
                    attack_collider.SetPowered(true);

                    this.attack_timer = PlayerControl.ATTACK_TIME;

                    this.attack_disable_timer = PlayerControl.ATTACK_DISABLE_TIME;

                    //播放攻击动作

                    Animation animation = this.transform.GetComponentInChildren<Animation>();



                    switch (this.attack_motion)
                    {
                        default:
                        case Attack_Motion.Right:
                            animation.CrossFade("P_attack_R", 0.2f);
                            break;
                        case Attack_Motion.Left:
                            animation.CrossFade("P_attack_L", 0.2f);
                            break;
                    }

                    //攻击动作结束后，回到奔跑动作
                    animation.CrossFadeQueued("P_run");

                    this.attack_voice_audio.PlayOneShot(this.AttackSound[this.attack_sound_index]);

                    this.attack_sound_index = (this.attack_sound_index + 1)%this.AttackSound.Length;

                    this.sword_audio.PlayOneShot(this.SwordSound);
                }
            }
        }
    }


    //剑的轨迹特效
    private void Sword_Fx_Control()
    {
        do
        {
            if (this.attack_timer <= 0.0f)
            {
                break;
            }

            if (this.kisski_left.isPlaying())
            {
                break;
            }

            Animation animation = this.transform.GetComponentInChildren<Animation>();
            AnimationState state;
            //AnimatedTextureExtendedUV利用texture的scale和offset实现帧动画，用于刀光的实现。
            AnimatedTextureExtendedUV anim_player;

            switch (this.attack_motion)
            {
                default:
                case Attack_Motion.Right:
                {
                    state = animation["P_attack_R"];
                    anim_player = this.kisski_right;
                }
                break;

                case Attack_Motion.Left:
                {
                    state = animation["P_attack_L"];
                    anim_player = this.kisski_left;
                }
                break;
            }

            float start_time = 2.5f;
            float current_frame = state.time*state.clip.frameRate;

            if (current_frame < start_time)
            {
                break;
            }

            anim_player.startPlay(state.time - start_time/state.clip.frameRate);

        } while (false);
    }
}
