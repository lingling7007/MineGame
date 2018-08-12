using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public float m_movSpeed = 10;//移动系数  
    public float m_rotateSpeed = 1;//旋转系数  
    private void Start()
    {
        TPCamera.instance.target = transform;
        TPCamera.instance.cameraView = TPCamera.ECameraView.FreeView;

    }
    void Update()
    {
        // 定义3个值控制移动  
        float xm = 0, ym = 0, zm = 0;
        //按键盘W向上移动  
        if (Input.GetKey(KeyCode.W))
        {
            zm += m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.S))//按键盘S向下移动  
        {
            zm -= m_movSpeed * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))//按键盘A向左移动  
        {
            xm -= m_movSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.D))//按键盘D向右移动  
        {
            xm += m_movSpeed * Time.deltaTime;
        }
        transform.Translate(new Vector3(xm, ym, zm), Space.Self);
        TPCamera.instance.AudioListenerTransform.localPosition = transform.localPosition;
        if (Input.GetMouseButtonDown(0))
        {
            TPCamera.instance.IsOnJoystickGUI = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            TPCamera.instance.IsOnJoystickGUI = false;
        }
    }
}
