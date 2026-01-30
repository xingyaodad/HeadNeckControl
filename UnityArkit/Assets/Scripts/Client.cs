using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARKit;
using UnityEngine.XR.ARSubsystems;
#if UNITY_IOS && !UNITY_EDITOR
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.ARKit;
#endif



    /// <summary>
    /// Streams blend shape data from a device to a connected server.
    /// </summary>
    class Client : MonoBehaviour
    {
        byte[] m_Buffer;

        float m_CurrentTime = -1;

        bool m_Running;

        float m_StartTime;
        const float k_Timeout = 5;
        const int k_SleepTime = 40;
        public IPEndPoint endPoint;

        public TextMeshProUGUI text;
        readonly string[] All_BsName = new string[] {
            "BrowDownLeft",
            "BrowDownRight",
            "BrowInnerUp",
            "BrowOuterUpLeft",
            "BrowOuterUpRight",
            "CheekPuff",
            "CheekSquintLeft",
            "CheekSquintRight",
            "EyeBlinkLeft",
            "EyeBlinkRight",
            "EyeLookDownLeft",
            "EyeLookDownRight",
            "EyeLookInLeft",
            "EyeLookInRight",
            "EyeLookOutLeft",
            "EyeLookOutRight",
            "EyeLookUpLeft",
            "EyeLookUpRight",
            "EyeSquintLeft",
            "EyeSquintRight",
            "EyeWideLeft",
            "EyeWideRight",
            "JawForward",
            "JawLeft",
            "JawOpen",
            "JawRight",
            "MouthClose",
            "MouthDimpleLeft",
            "MouthDimpleRight",
            "MouthFrownLeft",
            "MouthFrownRight",
            "MouthFunnel",
            "MouthLeft",
            "MouthLowerDownLeft",
            "MouthLowerDownRight",
            "MouthPressLeft",
            "MouthPressRight",
            "MouthPucker",
            "MouthRight",
            "MouthRollLower",
            "MouthRollUpper",
            "MouthShrugLower",
            "MouthShrugUpper",
            "MouthSmileLeft",
            "MouthSmileRight",
            "MouthStretchLeft",
            "MouthStretchRight",
            "MouthUpperUpLeft",
            "MouthUpperUpRight",
            "NoseSneerLeft",
            "NoseSneerRight",
            "TongueOut"
         };
    
    void Awake()
    {
        m_Buffer = new byte[274];
            
    }


    private void Update()
    {
        //主线程每帧调用一次 CommitFrame
        BSDataSlingleton.Instance.CommitFrame();

        StringBuilder sb = new StringBuilder();
        float[] rot = BSDataSlingleton.Instance.GetRotData();
        for (int i = 0; i < rot.Length; i++)
        { 
            sb.Append(rot[i].ToString("F3"));
            sb.Append('\n');
        }

        text.text = sb.ToString();
    
    }
    public void Return2ZeroRot()
    {
        float[] rot = BSDataSlingleton.Instance.GetRotData();
        BSDataSlingleton.Instance.Return2Zero(rot);
    }
    public void Reset2ZeroRot()
    {
        BSDataSlingleton.Instance.ResetZero();
    }
    public void StartCapture(UdpClient socket)
    {

        Application.targetFrameRate = 60;
        m_Running = true;

        new Thread(() =>
        {

            while (m_Running)
            {
                try
                {
                        
                    m_Buffer[0] = (byte)42;

                    var packet = BSDataSlingleton.Instance.GetSendBuffer();

                    Buffer.BlockCopy(packet, 0, m_Buffer, 1, 232);

                    socket.Send(m_Buffer, m_Buffer.Length, endPoint);
                        
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    TryTimeout();
                }

                Thread.Sleep(k_SleepTime);
            }
        }).Start();
    }

    public void stop()
    {
        m_Running = false;
    }
    void TryTimeout()
    {
        if (m_CurrentTime - m_StartTime > k_Timeout)
        {
            m_Running = false;
        }
    }
       
      
}

