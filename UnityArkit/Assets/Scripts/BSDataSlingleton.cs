using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSDataSlingleton
{
    private static BSDataSlingleton _instance;
    private static readonly object _instanceLock = new object();

    public static BSDataSlingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                        _instance = new BSDataSlingleton();
                }
            }
            return _instance;
        }
    }

    // ------------------------------
    // Double Buffer
    // ------------------------------
    private readonly byte[] _bufferA = new byte[232];
    private readonly byte[] _bufferB = new byte[232];

    private volatile byte[] _activeWriteBuffer; // 主线程写
    private volatile byte[] _activeSendBuffer;  // 发送线程读

    private readonly float[] _bs = new float[52];
    private readonly float[] _pos = new float[3];
    private readonly float[] _rot = new float[3];
    private readonly float[] _zero = new float[3];
    private readonly float[] _rotAdj = new float[3];

    private BSDataSlingleton()
    {
        _activeWriteBuffer = _bufferA;
        _activeSendBuffer = _bufferB;
    }

    // ------------------------------
    // 主线程写数据
    // ------------------------------
    public void SetBsData(int idx, float v)
    {
        _bs[idx] = v;
    }

    public void SetPosData(Vector3 p)
    {
        _pos[0] = p.x;
        _pos[1] = p.y;
        _pos[2] = p.z;
    }

    public void SetRotData(Vector3 r)
    {
        _rot[0] = r.x;
        _rot[1] = r.y;
        _rot[2] = r.z;
    }

    public void Return2Zero(float[] v)
    {
        _zero[0] = v[0];
        _zero[1] = v[1];
        _zero[2] = v[2];
    }
    public void ResetZero()
    {
        _zero[0] = 0f;
        _zero[1] = 0f;
        _zero[2] = 0f;
    }

    // ------------------------------
    // 将最新数据写入 activeWriteBuffer
    // 在 ARKit 更新完毕后，每帧调用一次
    // ------------------------------
    public void CommitFrame()
    {
        var buf = _activeWriteBuffer;

        // BS
        Buffer.BlockCopy(_bs, 0, buf, 0, 52 * 4);

        // pos
        Buffer.BlockCopy(_pos, 0, buf, 52 * 4, 3 * 4);

        // rot-zero
        _rotAdj[0] = _rot[0] - _zero[0];
        _rotAdj[1] = _rot[1] - _zero[1];
        _rotAdj[2] = _rot[2] - _zero[2];

        Buffer.BlockCopy(_rotAdj, 0, buf, (52 + 3) * 4, 3 * 4);

        // ---- swap buffers (no lock needed!) ----
        var tmp = _activeSendBuffer;
        _activeSendBuffer = _activeWriteBuffer;
        _activeWriteBuffer = tmp;
    }

    // ------------------------------
    // 后台线程用这个发送（零复制）
    // ------------------------------
    public byte[] GetSendBuffer()
    {
        return _activeSendBuffer;
    }

    // ------------------------------
    // 可选：单项数据读取（无锁，因为只有主线程改）
    // ------------------------------
    public float[] GetPosData()
    {
        float[] r = new float[3];
        Array.Copy(_pos, r, 3);
        return r;
    }

    public float[] GetRotData()
    {
        float[] r = new float[3];
        r[0] = _rot[0] - _zero[0];
        r[1] = _rot[1] - _zero[1];
        r[2] = _rot[2] - _zero[2];
        return r;
    }

    public float[] GetBSData()
    {
        float[] r = new float[52];
        Array.Copy(_bs, r, 52);
        return r;
    }
}

