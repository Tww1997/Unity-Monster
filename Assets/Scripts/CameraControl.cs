using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//控制摄像机移动
public class CameraControl : MonoBehaviour
{
    
    private GameObject player = null;
    //摄像机与player的距离差值
    private Vector3 offset;

	// Use this for initialization
	void Start ()
	{
	    player = GameObject.FindGameObjectWithTag("Player");

	    this.offset = this.transform.position - this.player.transform.position;

	}
	
	// Update is called once per frame
	void Update ()
	{
        //本项目player面向X移动，所以只计算X轴偏移
	    this.transform.position = new Vector3(this.player.transform.position.x + offset.x, this.transform.position.y,
	        this.transform.position.z);
	}
}
