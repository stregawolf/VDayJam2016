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
    }

    public void OnPlayPressed()
    {
        mFader.FadeOut(() => SceneManager.LoadScene("CharacterSelect"));
    }
}
