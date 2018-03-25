using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackColliderControl : MonoBehaviour
{

    public PlayerControl player = null;

    //正在执行攻击判定
    public bool is_powered = false;

	// Use this for initialization
	void Start ()
	{
	    this.SetPowered(false);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //OnTriggerEnter只会在和碰撞对象发生接触的瞬间被调用
    //如果在产生攻击判断球时完全嵌入怪物的内侧，则会不容易检测
    //所以此处使用OnTriggerStay
    void OnTriggerStay(Collider other)
    {
        do
        {
            if (!this.is_powered)
            {
                break;
            }
            if (other.tag != "OniGroup")
            {
                break;
            }

            OniGroupControl oni = other.GetComponent<OniGroupControl>();

            if (oni == null)
            {
                break;
            }

            //怪物被击飞
            oni.OnAttackedFromPlayer();

            //重置不能攻击计时器
            this.player.ReseAttackDisableTimer();

            //播放攻击命中特效
            this.player.PlayHitEffect(oni.transform.position);

            //发出攻击命中音效
            this.player.PlayHitSound();

        } while (false);
    }


    public void SetPowered(bool sw)
    {
        this.is_powered = sw;
        if (SceneControl.IS_ONI_BLOWOUT_CAMERA_LOCAL)
        {
            //this.GetComponent<Renderer>().enabled = sw;
        }

    }
}
