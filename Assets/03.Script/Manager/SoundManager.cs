using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{

    public AudioSource bgmSource;
    public AudioSource effectSource;

    public AudioClip HitAudio;
    public AudioClip[] boss_Audioclip; // 0 : BGM, 1 : LAUGH , 2 : BULLET, 3 : LASER
    public AudioClip[] Player_Audio; // 0.걷는소리 / 1.플레이어가 맞았을때
    public AudioClip[] Player_Weapon; // 0.무기 휘두르는 소리
    public AudioClip[] UI_Audio; // 0.타운BGM / 1.던전BGM / 2. 죽는소리 / 3.던전입장 / 4.로비 BGM
    public AudioClip[] monsterAudio; // 0.스컬독 / 1. 박쥐 / 2.스컬대장

    private AudioClip currentBGM;

    public float bgmSave;
    public float effectSave;

    [SerializeField]
    private Slider bgmSlider;

    [SerializeField]
    private Slider effectSlider;

    public static SoundManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    void Start()
    {
        if (PlayerPrefs.HasKey("bgmVolume"))
        {
            bgmSave = PlayerPrefs.GetFloat("bgmVolume");

            bgmSource.volume = bgmSave;
        }
        if (PlayerPrefs.HasKey("effectVolume"))
        {
            effectSave = PlayerPrefs.GetFloat("effectVolume");

            effectSource.volume = effectSave;
        }
    }
    void Update()
    {
        Volume_Update();
    }

    // BGM 변경
    public void PlayBGM(AudioClip newBGM)
    {
        if (currentBGM != null && currentBGM == newBGM)
        {
            return;
        }
        StopBGM();
        currentBGM = newBGM;
        bgmSource.clip = newBGM;
        bgmSource.Play();
    }
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    // 설정창의 볼륨 업데이트
    void Volume_Update()
    {
        if (bgmSlider != null)
        {
            bgmSave = bgmSlider.value;
            bgmSource.volume = bgmSave;
        }
        if (effectSlider != null)
        {
            effectSave = effectSlider.value;
            effectSource.volume = effectSave;
        }
    }
    public void Volume_Save()
    {
        PlayerPrefs.SetFloat("bgmVolume", bgmSave);
        PlayerPrefs.SetFloat("effectVolume", effectSave);
    }
}
