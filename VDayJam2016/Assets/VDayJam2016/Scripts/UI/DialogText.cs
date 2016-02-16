using UnityEngine;
using System.Collections;

public class DialogText : MonoBehaviour {
    public const float kShowTweenTime = 0.33f;

    public TextMesh mText;

    public void Awake()
    {
        if (mText == null)
        {
            mText = GetComponent<TextMesh>();
        }
        transform.localScale = Vector3.zero;
        gameObject.SetActive(false);
    }

    protected void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }

    public void Show(string text, float duration = 3.0f)
    {
        gameObject.SetActive(true);
        StopCoroutine("HandleTextDisplay");
        mText.text = text;
        StartCoroutine(HandleTextDisplay(duration));
    }

    public IEnumerator HandleTextDisplay(float duration)
    {

        float showTimer = 0.0f;
        while(showTimer < kShowTweenTime)
        {
            showTimer += Time.deltaTime;
            transform.localScale = Vector3.one * SinLerp(showTimer / kShowTweenTime);
            yield return new WaitForEndOfFrame();
        }

        transform.localScale = Vector3.one;
        yield return new WaitForSeconds(duration);
        
        while (showTimer > 0)
        {
            showTimer -= Time.deltaTime;
            transform.localScale = Vector3.one * SinLerp(showTimer / kShowTweenTime);
            yield return new WaitForEndOfFrame();
        }

        gameObject.SetActive(false);
    }

    public float SinLerp(float t)
    {
        return -0.5f * (Mathf.Cos(Mathf.PI * Mathf.Clamp01(t)) - 1);
    }
}
