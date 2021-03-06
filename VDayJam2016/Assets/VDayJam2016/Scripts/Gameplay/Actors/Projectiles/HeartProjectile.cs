﻿using UnityEngine;
using System.Collections;

public class HeartProjectile : BaseProjectile {
    public int mHeartValue = 1;

    public Renderer[] mRenderers;
    public GameObject mCollectionVFX;

    protected override void Awake()
    {
        base.Awake();
        mRenderers = GetComponentsInChildren<Renderer>();
    }

    protected override void StartAutoDestruction()
    {
        StartCoroutine(Flicker());
    }

    public IEnumerator Flicker()
    {
        bool visible = true;
        while(mLifetime > 0)
        {
            visible = !visible;
            SetRendererEnabled(visible);
            //SetColor(new Color(Random.value + 0.5f, Random.value + 0.5f, Random.value + 0.5f));
            if(!visible)
            {
                yield return new WaitForSeconds(.05f);
                mLifetime -= 0.05f;
            }
            else
            {
                float visibleTime = mLifetime * 0.1f;
                yield return new WaitForSeconds(visibleTime);
                mLifetime -= visibleTime;
            }
        }

        Destroy(gameObject);
    }

    public void SetRendererEnabled(bool enabled)
    {
        for (int i = 0, n = mRenderers.Length; i < n; ++i)
        {
            mRenderers[i].enabled = enabled;
        }
    }

    public override void OnCollisionEnter(Collision c)
    {
        BasePlayer hitPlayer = c.gameObject.GetComponentInParent<BasePlayer>();
        if (hitPlayer != null)
        {
            SoundManager.Instance.PlaySfx(SoundManager.Instance.sfx_pickup1);
            hitPlayer.mDialogText.Show(string.Format("+{0} Heart", mHeartValue), 2.0f);
            GlobalData.NumHearts += mHeartValue;
            Instantiate(mCollectionVFX, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
