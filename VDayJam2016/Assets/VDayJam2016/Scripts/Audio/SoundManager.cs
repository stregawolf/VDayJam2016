using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
	private static SoundManager _instance;
	public static SoundManager Instance {
		get{
			return _instance;
		}
	}

	//Snapshots
	public AudioMixerSnapshot DefaultSnapshot;
	public AudioMixerSnapshot NoBgmSnapshot;

	//SoundPlayers
	public AudioSource sfxSource;
	public AudioSource bgmSource;

	//Clips
    //BGM
	public AudioClip bgm_title;
	public AudioClip bgm_main;
	public AudioClip bgm_shop;
	public AudioClip bgm_boss_flower;
	public AudioClip bgm_boss_chocolate;
	public AudioClip bgm_boss_imposter;
    public AudioClip bgm_final_room;
   
    //SFX
    public AudioClip sfx_confirm;
    public AudioClip sfx_decline;
    public AudioClip sfx_pickup1;
    public AudioClip sfx_pickup2;
    public AudioClip sfx_player_atk_melee;
    public AudioClip sfx_player_atk_ranged;
    public AudioClip sfx_playerhurt1;
    public AudioClip sfx_playerhurt2;
    public AudioClip sfx_win_ditty;
    
    public AudioClip sfx_monster_pop;
    public AudioClip sfx_ouch_bones;
    public AudioClip sfx_ouch_dragon;
    public AudioClip sfx_ouch_ghost;
    public AudioClip sfx_ouch_paper;
    public AudioClip sfx_ouch_poop;
    public AudioClip sfx_ouch_spider;

    public string curBgm;
	public void Awake()
	{
		if(_instance != null)
		{
			Destroy(this);
			return;
		}

		_instance = this;
		DontDestroyOnLoad (this.gameObject);
	}

	public void Update(){

	}

	public SoundManager(){
        
	}

	private float beatsToSeconds(float beats, float bpm = 120){
		return beats * 60/bpm;
	}

	//such repetition, wow.
	public void FadeOut(float s = 0){
		NoBgmSnapshot.TransitionTo (s);
	}
    
    public void FadeIn(float s = 0){
        DefaultSnapshot.TransitionTo(s);
    }

	public void PlaySfx(AudioClip clip, float volume = .75f){
		if (clip != null) {
			sfxSource.PlayOneShot(clip, volume);
		}
	}

	public void PlayBgm(AudioClip clip, bool overwrite = false){
        if (clip != null && (overwrite || clip.name != curBgm)) {
            curBgm = clip.name;
			bgmSource.clip = clip;
			bgmSource.Play ();
		} else if (clip == null) {
            FadeOut();
        }
	}
    
    public void StopBgm(){
        bgmSource.Stop();
    }
    
    public void RestartBgm(){
        if(bgmSource.clip != null){
            bgmSource.Play();
        }
    }
    
    //Common SFX
    public void PlayMenuConfirmSFX(){
        PlaySfx(sfx_confirm);
    }
    
    public void PlayMenuDeclineSFX(){
        PlaySfx(sfx_decline);
    }
}
