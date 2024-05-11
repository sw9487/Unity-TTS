using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

public class YatingTextToSpeech : TTS
{
    #region 參數定義

    [SerializeField] private string api_key = string.Empty;//apikey
    [SerializeField] private ModelType m_ModelType = ModelType.zh_en_female_2;//模型
    [SerializeField, Range(0.5f, 1.5f)] private float speed = 1.0f;
    [SerializeField, Range(0.5f, 1.5f)] private float pitch = 1.0f;
    [SerializeField, Range(0.5f, 1.5f)] private float energy = 1.0f;
    [SerializeField] private AudioEncoding audioEncoding = AudioEncoding.LINEAR16;
    [SerializeField] private SampleRate sampleRate = SampleRate._22K;

    #endregion

    private void Awake()
    {
        m_PostURL = "https://tts.api.yating.tw/v2/speeches/short";
    }

    /// <summary>
    /// 語音合成，回合成文本
    /// </summary>
    /// <param name="_msg"></param>
    /// <param name="_callback"></param>
    public override void Speak(string _msg, Action<AudioClip, string> _callback)
    {
        StartCoroutine(GetVoice(_msg, _callback));
    }

    private IEnumerator GetVoice(string _msg, Action<AudioClip, string> _callback)
    {
        var _sampleRate = "";
        switch (sampleRate)
        {
            case SampleRate._16K:
                _sampleRate = "16K";
                break;
            case SampleRate._22K:
                _sampleRate = "22K";
                break;
        }

        RequestData _requestData = new RequestData
        {
            input = new InputData
            {
                text = _msg,
                type = "text"
            },
            voice = new VoiceData
            {
                model = m_ModelType.ToString(),
                speed = speed,
                pitch = pitch,
                energy = energy
            },
            audioConfig = new AudioConfigData
            {
                encoding = audioEncoding.ToString(),
                sampleRate = _sampleRate
            }
        };

        string _jsonText = JsonUtility.ToJson(_requestData).Trim();
        Debug.Log(_jsonText);

        stopwatch.Restart();
        using (UnityWebRequest request = new UnityWebRequest(m_PostURL, "POST"))
        {
            byte[] data = System.Text.Encoding.UTF8.GetBytes(_jsonText);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(data);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            request.SetRequestHeader("key", api_key);
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("雅婷語音合成 傳送：" + _jsonText);
            yield return request.SendWebRequest();

            if (request.responseCode == 201)
            {
                Debug.Log("雅婷語音合成 接收到：" + request.downloadHandler.data);
                string _text = request.downloadHandler.text;


                Response _response = JsonUtility.FromJson<Response>(_text);

                // 將base64字符串解碼為二進制數據
                byte[] audioBytes = Convert.FromBase64String(_response.audioContent);
                // 將解碼後的二進制數據寫入文件中
                string filePath = Application.persistentDataPath + "/decodedAudio.wav"; // 可以指定任意文件名和文件格式
                File.WriteAllBytes(filePath, audioBytes);

                // 將解碼後的二進制數據轉換為Unity的AudioClip
                AudioClip audioClip = WavUtility.ToAudioClip(filePath);

                _callback(audioClip, _msg);
            }
            else
            {
                Debug.LogError("語音合成失敗：" + request.responseCode);
                Debug.LogError("語音合成失敗：" + request.error);
            }

            stopwatch.Stop();
            Debug.Log("雅婷語音合成：" + stopwatch.Elapsed.TotalSeconds);
        }
    }

    #region 數據定義

    /*
    發送的資料格式

    {
   "input":{
      "text":"這是測試",
      "type":"text"
   },
   "voice":{
      "model":"zh_en_female_2",
      "speed":0.8,
      "pitch":1.3,
      "energy":1.0
   },
   "audioConfig":{
      "encoding":"LINEAR16",
      "sampleRate":"22K"
   }
}
    */
    /*

    傳回的資料格式

    {
   "audioContent":"//NExAARqoIIAAhEuWAAAGNmBGMY4EBcxvABAXBPmPIAF3/o...",
   "audioConfig":{
      "encoding":"LINEAR16",
      "sampleRate":"22K"
   }
}
    */

    [Serializable]
    public class Response
    {
        public string audioContent = string.Empty;
        public AudioConfigData audioConfig = new AudioConfigData();
    }

    /// <summary>
    /// 發送的報文
    /// </summary>
    [Serializable]
    public class RequestData
    {
        public InputData input;
        public VoiceData voice;
        public AudioConfigData audioConfig;
    }

    [Serializable]
    public class InputData
    {
        public string text;
        public string type;
    }

    [Serializable]
    public class VoiceData
    {
        public string model;
        public float speed;
        public float pitch;
        public float energy;
    }

    [Serializable]
    public class AudioConfigData
    {
        public string encoding;
        public string sampleRate;
    }

    /// <summary>
    /// 模型類型
    /// </summary>
    public enum ModelType
    {
        zh_en_female_1,// 雅婷AI人聲
        zh_en_male_1,// 家豪AI人聲
        zh_en_female_2,// 意晴AI人聲
        zh_en_male_2,// 志明AI人聲
    }

    public enum AudioEncoding
    {
        LINEAR16,
        MP3
    }

    public enum SampleRate
    {
        _16K,
        _22K
    }
    #endregion
}
