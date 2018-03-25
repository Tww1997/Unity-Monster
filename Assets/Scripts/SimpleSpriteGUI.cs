using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleSpriteGUI : MonoBehaviour
{

    public Texture texture;
    public Vector3 position;
    public Vector3 scale;   //大小比例
    public float angle;     //角度
    public Color color;     //颜色

    public Vector3 pivot;
    public Matrix4x4 matrix;
    public Matrix4x4 matrix_trans_rect;

    public Rect rect;

    public bool is_visible;

    private bool is_update_matrix;

    //-------------------------------------------------//

    public void create()
    {
        this.position = Vector3.zero;
        this.scale = Vector3.one;
        this.angle = 0.0f;
        this.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        this.pivot = Vector3.zero;
        this.matrix = Matrix4x4.identity;

        this.rect = new Rect(0.0f,0.0f,1.0f,1.0f);

        this.is_update_matrix = true;

        this.is_visible = true;
    }

    public void draw()
    {
        if (this.is_visible)
        {
            if (this.is_update_matrix)
            {
               
            }
        }
    }

}
