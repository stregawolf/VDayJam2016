using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class WinScreen : MonoBehaviour {
    public Fader mFader;
    public string mNextScene = "FinalWin";

    public void Awake()
    {
        if (mFader == null)
        {
            mFader = FindObjectOfType<Fader>();
        }
        mFader.FadeIn();
        PlayMusic();
    }

    public void OnYesPressed()
    {
        mFader.FadeOut(() => SceneManager.LoadScene(mNextScene));
        SoundManager.Instance.PlayMenuConfirmSFX();
    }

    private void PlayMusic()
    {
        SoundManager.Instance.FadeIn();
        SoundManager.Instance.PlayBgm(SoundManager.Instance.bgm_title);
    }
}
