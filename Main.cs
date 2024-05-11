using UnityEngine;
using UnityEngine.UI;

public class Main : MonoBehaviour
{
    #region UI定義
    /// <summary>
    /// 輸入的訊息
    /// </summary>
    [SerializeField] public InputField m_InputWord;
    /// <summary>
    /// 發送訊息按鈕
    /// </summary>
    [SerializeField] private Button m_CommitInputBtn;
    #endregion
    #region 參數定義
    /// <summary>
    /// 播放聲音
    /// </summary>
    [SerializeField] private AudioSource _audioSource;
    /// <summary>
    /// 播放聲音
    /// </summary>
    [SerializeField] private TTS _TextToSpeech;

    #endregion

    private void Awake()
    {
        m_CommitInputBtn.onClick.AddListener(delegate { CheckInput(); });
    }

    private void CheckInput()
    {
        if (m_InputWord.text.Equals(""))// Need to enter some text...
            return;

        string _msg = m_InputWord.text;
        _TextToSpeech.Speak(_msg, PlayVoice);
    }

    #region 語音合成

    private void PlayVoice(AudioClip _clip, string _response)
    {
        _audioSource.clip = _clip;
        _audioSource.Play();
        Debug.Log("音頻時長：" + _clip.length);
    }

    #endregion
}
