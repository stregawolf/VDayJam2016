using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Fader : MonoBehaviour {
    public Image mImage;

    protected bool mIsFading = false;

    public void FadeIn(System.Action onComplete = null, float fadeTime = 1.0f)
    {
        if(mIsFading)
        {
            return;
        }

        StartCoroutine(HandleFadeIn(fadeTime, onComplete));
    }

    public void FadeOut(System.Action onComplete = null, float fadeTime = 0.5f)
    {
        if (mIsFading)
        {
            return;
        }
        StartCoroutine(HandleFadeOut(fadeTime, onComplete));
    }

    protected IEnumerator HandleFadeIn(float fadeTime, System.Action onComplete)
    {
        mIsFading = true;

        float fadeTimer = fadeTime;
        Color c;
        while (fadeTimer > 0)
        {
            fadeTimer -= Time.unscaledDeltaTime;

            c = mImage.color;
            c.a = Mathf.Clamp01(fadeTimer / fadeTime);
            mImage.color = c;

            yield return new WaitForEndOfFrame();
        }

        c = mImage.color;
        c.a = 0.0f;
        mImage.color = c;

        mIsFading = false;

        if (onComplete != null)
        {
            onComplete();
        }
    }

    protected IEnumerator HandleFadeOut(float fadeTime, System.Action onComplete)
    {
        mIsFading = true;
        float fadeTimer = fadeTime;
        Color c;
        while (fadeTimer > 0)
        {
            fadeTimer -= Time.unscaledDeltaTime;

            c = mImage.color;
            c.a = 1.0f-Mathf.Clamp01(fadeTimer / fadeTime);
            mImage.color = c;

            yield return new WaitForEndOfFrame();
        }

        c = mImage.color;
        c.a = 1.0f;
        mImage.color = c;

        mIsFading = false;

        if (onComplete != null)
        {
            onComplete();
        }
    }
}
