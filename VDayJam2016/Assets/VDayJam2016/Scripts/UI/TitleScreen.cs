using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreen : MonoBehaviour {
    public Fader mFader;

    public GameObject mSaveExistsGroup;
    public GameObject mFirstTimeGroup;

    public GameObject mAreYouSurePopUp;

    public void Awake()
    {
        if(mFader == null)
        {
            mFader = FindObjectOfType<Fader>();
        }

        if(GlobalData.SaveExists())
        {
            mSaveExistsGroup.SetActive(true);
            mFirstTimeGroup.SetActive(false);
        }
        else
        {
            mSaveExistsGroup.SetActive(false);
            mFirstTimeGroup.SetActive(true);
        }

        mFader.FadeIn();
        playTitleTheme();
    }

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            mFader.FadeOut(() => SceneManager.LoadScene("RoseWin"));
        }

        if (Input.GetKeyDown(KeyCode.F2))
        {
            mFader.FadeOut(() => SceneManager.LoadScene("VuWin"));
        }
    }

    public void OnContninuePressed()
    {
        GlobalData.Load();
        mFader.FadeOut(() => SceneManager.LoadScene("Game"));
        SoundManager.Instance.PlayMenuConfirmSFX();
    }

    public void OnNewGamePressed()
    {
        mAreYouSurePopUp.SetActive(true);
        SoundManager.Instance.PlayMenuConfirmSFX();
    }

    public void OnCancelNewGamePressed()
    {
        mAreYouSurePopUp.SetActive(false);
    }

    public void OnNewGameConfirmedPressed()
    {
        mAreYouSurePopUp.SetActive(false);
        OnPlayPressed();
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
