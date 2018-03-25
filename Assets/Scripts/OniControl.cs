using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OniControl : MonoBehaviour
{
    //玩家
    public PlayerControl player = null;

    //摄像机
    public GameObject main_camera = null;

    //碰撞盒的大小
    public static float collision_size = 0.5f;

    //依然活着？
    private bool is_alive = true;

    //生成时的位置
    private Vector3 initial_position;

    //左右波动时的波动周期
    private float wave_angle_offset = 0.0f;

    //左右波动时的幅度
    public float wave_amplitude = 0.0f;


    enum Step
    {
        None = -1,

        Run = 0,
        Defated,

        Num,
    };

    //当前的状态
    private Step step = Step.None;

    //下次迁移的状态
    private Step next_step = Step.None;

    //状态迁移后的时间
    private float step_time = 0.0f;

    //defeated,fly_to_stack 开始时的速度向量
    private Vector3 blowout_vector = Vector3.zero;
    private Vector3 blowout_angular_velocity = Vector3.zero;

    //----------------------------------------------------------------//

	// Use this for initialization
	void Start ()
	{
        //生成时的位置
	    this.initial_position = this.transform.position;

	    this.transform.rotation = Quaternion.AngleAxis(180.0f, Vector3.up);

	    this.GetComponent<Collider>().enabled = false;

        //不限制旋转的速度
	    this.GetComponent<Rigidbody>().maxAngularVelocity = float.PositiveInfinity;

        //模型的中心略靠下，重心作适当偏移
        this.GetComponent<Rigidbody>().centerOfMass = new Vector3(0.0f,0.5f,0.0f);
	}
	
	// Update is called once per frame
	void Update ()
	{
	    this.step_time += Time.deltaTime;

	    switch (this.step)
	    {
            case Step.None:
	        {
	            this.next_step = Step.Run;
	        }
	            break;
	    }

        //初始化
        //状态发生迁移时的初始化处理

	    if (this.next_step != Step.None)
	    {
	        switch (this.next_step)
	        {
                case Step.Defated:
	            {
	                this.GetComponent<Rigidbody>().velocity = this.blowout_vector;

                    //旋转的角速度
	                this.GetComponent<Rigidbody>().angularVelocity = this.blowout_angular_velocity;

                    //接触父子关系
                    //因为父对象（OniGroup)被删除时子对象也会被删除
	                this.transform.parent = null;

                    //在摄像机的坐标系内运动
                    //（和摄像机一起联动）
	                if (SceneControl.IS_ONI_BLOWOUT_CAMERA_LOCAL)
	                {
	                    this.transform.parent = this.main_camera.transform;
	                }


                    //播放被攻击的动作
	                this.transform.GetChild(0).GetComponent<Animation>().Play("oni_yarare");

	                this.is_alive = false;
	            }
	                break;
	        }
	        this.step = this.next_step;

	        this.next_step = Step.None;

	        this.step_time = 0.0f;
	    }

        //各个状态的执行处理

	    Vector3 new_position = this.transform.position;

        float low_limit = this.initial_position.y;

	    switch (this.step)
	    {
            case Step.Run:
	        {
	            //活着的时候使它不会陷入地面中

	            if (new_position.y < low_limit)
	            {
	                new_position.y = low_limit;
	            }

                //左右波动

	            float wave_angle = 2.0f*Mathf.PI*Mathf.Repeat(this.step_time, 1.0f) + this.wave_angle_offset;

	            float wave_offset = this.wave_amplitude*Mathf.Sin(wave_angle);

	            new_position.z = this.initial_position.z + wave_offset;

                //方向（Y轴旋转）也随之变化
	            if (this.wave_amplitude > 0.0f)
	            {
	                this.transform.rotation = Quaternion.AngleAxis(180.0f - 30.0f*Mathf.Sin(wave_angle + 90.0f), Vector3.up);
	            }
	        }
	            break;
            case Step.Defated:
	        {
	            if (new_position.y < low_limit)
	            {
                    //死后的短时间可能会陷入地面中，速度朝上（死后的瞬间）时，让其不落入地面中
	                if (this.GetComponent<Rigidbody>().velocity.y > 0.0f)
	                {
	                    new_position.y = low_limit;
	                }
	            }

                //稍微向后移动
	            if (this.transform.parent != null)
	            {
	                this.GetComponent<Rigidbody>().velocity += -3.0f*Vector3.right*Time.deltaTime;
	            }
	        }
	            break;

	    }
	    this.transform.position = new_position;

	    do
	    {
	        if (this.GetComponent<Renderer>().isVisible)
	        {
	            break;
	        }
	        if (this.is_alive)
	        {
	            break;
	        }

	        if (this.GetComponent<AudioSource>().isPlaying)
	        {
	            if (this.GetComponent<AudioSource>().time<this.GetComponent<AudioSource>().clip.length)
	            {
	                break;
	            }
	        }

	        Destroy(this.gameObject);
	    } while (false);
	}

    //设置动作的播放速度
    public void setMotionSpeed(float speed)
    {
        this.transform.GetChild(0).GetComponent<Animation>()["oni_run1"].speed = speed;
        this.transform.GetChild(0).GetComponent<Animation>()["oni_run2"].speed = speed;
    }

    //开始执行被攻击时的处理
    public void AttackedFromPlayer(Vector3 blowout, Vector3 angular_velocity)
    {
        this.blowout_vector = blowout;
        this.blowout_angular_velocity = angular_velocity;

        //解除父子关系
        this.transform.parent = null;

        //父对象（onigroup)被删除后将被一并删除
        this.next_step = Step.Defated;
    }
}
