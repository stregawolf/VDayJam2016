using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreen : MonoBehaviour {
    public Fader mFader;

    public void Awake()
    {
        if(mFader == null)
        {
            mFader = FindObjectOfType<Fader>();
        }
        mFader.FadeIn();
        playTitleTheme();
    }

    public void OnPlayPressed()
    {
        mFader.FadeOut(() => SceneManager.LoadScene("CharacterSelect"));
        SoundManager.Instance.PlayMenuConfirmSFX();
    }
    
    private void playTitleTheme()
    {
        SoundManager.Instance.FadeIn();
        SoundManager.Instance.PlayBgm(SoundManager.Instance.bgm_title);    
    }
}
