using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TTS : MonoBehaviour
{
    /// <summary>
    /// 語音合成的api位址
    /// </summary>
    [SerializeField] protected string m_PostURL = string.Empty;
    /// <summary>
    /// 計算方法呼叫的時間
    /// </summary>
    [SerializeField] protected Stopwatch stopwatch = new Stopwatch();
    /// <summary>
    /// 語音合成，回音訊
    /// </summary>
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public virtual void Speak(string _msg, Action<AudioClip> _callback) { }
    /// <summary>
    /// 合成語音返回音頻，同時返回合成的文本
    /// </summary>
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public virtual void Speak(string _msg, Action<AudioClip, string> _callback) { }


}