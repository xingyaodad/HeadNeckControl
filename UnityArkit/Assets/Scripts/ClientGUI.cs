#if UNITY_IOS && !UNITY_EDITOR && INCLUDE_ARKIT_FACE_PLUGIN
#define SUPPORTED
#endif

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


    class ClientGUI : MonoBehaviour
    {
#pragma warning disable 649

        [SerializeField]
        int m_Port = 9003;

        [SerializeField]
        string m_ServerIP = "192.168.3.11";

        [SerializeField]
        Client m_Client;

        //[SerializeField]
        //Canvas m_MainGUI;

        [SerializeField]
        Button m_StopButton;

        [SerializeField]
        Button m_ConnectButton;

        [SerializeField]
        TMP_InputField m_PortTextField;

        [SerializeField]
        TMP_InputField m_IPTextField;



        UdpClient m_Socket;
        
        void Awake()
        {

        }

        void Start()
        {
            m_PortTextField.onValueChanged.AddListener(OnPortValueChanged);
            m_IPTextField.onValueChanged.AddListener(OnIPValueChanged);

            m_ConnectButton.onClick.AddListener(OnConnectClick);

            m_StopButton.onClick.AddListener(OnStopClick);
            // Make sure text fields match serialized values
            m_PortTextField.text = m_Port.ToString();
            m_IPTextField.text = m_ServerIP;
        }

        void Update()
        {
            var connected = m_Socket != null;
           
           
            if (m_ConnectButton.gameObject.activeSelf && connected)
            {
                IPAddress ip;
                if (!IPAddress.TryParse(m_ServerIP, out ip))
                    return;
                m_Client.endPoint = new IPEndPoint(ip, m_Port);
                m_Client.StartCapture(m_Socket);

            }
            m_ConnectButton.gameObject.SetActive(!connected);
            m_IPTextField.gameObject.SetActive(!connected);
            m_PortTextField.gameObject.SetActive(!connected);
            m_StopButton.gameObject.SetActive(connected);

        }

        void OnPortValueChanged(string value)
        {
            int port;
            if (int.TryParse(value, out port))
                m_Port = port;
            else
                m_PortTextField.text = m_Port.ToString();
        }

        void OnIPValueChanged(string value)
        {

            IPAddress ip;
            m_ConnectButton.gameObject.SetActive(IPAddress.TryParse(value, out ip));
            m_ServerIP = value;
        }
        void OnStopClick()
        {
            m_Client.stop();
            m_Socket = null;
        }
        void OnConnectClick()
        {
            new Thread(() =>
            {
                try
                {
                    var socket = new UdpClient();

                    m_Socket = socket;
                }
                catch (Exception e)
                {
                    Debug.Log("Exception trying to connect: " + e.Message);
                }
            }).Start();
        }

    }

