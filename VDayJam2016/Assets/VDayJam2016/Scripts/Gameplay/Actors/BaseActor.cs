using UnityEngine;
using System.Collections;

public class BaseActor : MonoBehaviour {
    public const float kRadius = 0.3f;

    public int mMaxHp = 10;
    public float mMaxSpeed = 5.0f;
    public bool mbFaceMovementDirection = true;
    public float mFlickerTime = 0.5f;

    public GameObject mModel;

    public Animator mAnimator;
    protected string mPlayedAnimationName;
    public Rigidbody mRigidbody;
    public Collider mCollider;
    public Renderer[] mRenderers;
    protected Color[] mOriginalColors;

    protected int mHp = 0;
    public int Hp { get { return mHp; } }

    public GameObject mHeartProjectilePrefab;

    protected Vector3 mMoveDir;
    protected float mSpeedFactor = 1.0f;

    protected bool mIsFlickering = false;
    public bool IsFlickering {  get { return mIsFlickering; } }

    protected Vector3 mLastPos;

    protected virtual void Awake()
    {
        if (mRigidbody == null)
        {
            mRigidbody = GetComponentInChildren<Rigidbody>();
        }

        if(mCollider == null)
        {
            mCollider = GetComponentInChildren<Collider>();
        }

        if(mAnimator == null)
        {
            mAnimator = GetComponentInChildren<Animator>();
        }

        if(mModel == null)
        {
            mModel = transform.FindChild("Model").gameObject;
        }

        mRenderers = GetComponentsInChildren<Renderer>();
        mOriginalColors = new Color[mRenderers.Length];
        for(int i = 0, n = mRenderers.Length; i < n; ++i)
        {
            mOriginalColors[i] = mRenderers[i].material.color;
        }

        mHp = mMaxHp;
    }

    public void ResetColor()
    {
        for (int i = 0, n = mRenderers.Length; i < n; ++i)
        {
            mRenderers[i].material.color = mOriginalColors[i];
        }
    }

    public void SetRendererEnabled(bool enabled)
    {
        for (int i = 0, n = mRenderers.Length; i < n; ++i)
        {
            mRenderers[i].enabled = enabled;
        }
    }

    public void SetColor(Color c)
    {
        for (int i = 0, n = mRenderers.Length; i < n; ++i)
        {
            mRenderers[i].material.color = c;
        }
    }

    public IEnumerator Flicker(System.Action onComplete = null)
    {
        mIsFlickering = true;
        bool visible = true;
        float flickerTimer = mFlickerTime;
        while(flickerTimer > 0)
        {
            visible = !visible;
            SetRendererEnabled(visible);
            //SetColor(new Color(Random.value + 0.5f, Random.value + 0.5f, Random.value + 0.5f));
            yield return new WaitForSeconds(0.05f);
            flickerTimer -= 0.05f;
        }

        SetRendererEnabled(true);
        ResetColor();

        mIsFlickering = false;
        if (onComplete != null)
        {
            onComplete();
        }
    }

    public void TriggerAnimation(string name)
    {
        mPlayedAnimationName = name;
        mAnimator.SetTrigger(name);
    }

    public bool IsPlayingAnimation()
    {
        return mAnimator.IsInTransition(0) || mAnimator.GetCurrentAnimatorStateInfo(0).IsName(mPlayedAnimationName);
    }

    public virtual void TeleportTo(Vector3 pos)
    {
        transform.position = pos;
    }

    public virtual void Stop()
    {
        mMoveDir = Vector3.zero;
        mAnimator.SetBool("IsWalking", false);
    }

    public virtual void MoveDir(Vector3 dir, float speedFactor = 1.0f)
    {
        mMoveDir = dir.normalized;
        mSpeedFactor = speedFactor;
        if (mbFaceMovementDirection)
        {
            LookDir(mMoveDir);
        }
        mAnimator.SetBool("IsWalking", dir.sqrMagnitude > 0);
    }

    public virtual void LookAtPoint(Vector3 pos)
    {
        LookDir(pos - transform.position);
    }

    public virtual void LookDir(Vector3 dir)
    {
        dir.y = transform.position.y;
        dir.Normalize();
        transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
    }

    public virtual void TakeDamage(int amount)
    {
        if (mHp < 0 || IsFlickering)
        {
            return;
        }

        TriggerAnimation("Hurt");
        mHp -= amount;
        if(mHp <= 0)
        {
            mHp = 0;
            StartCoroutine(Flicker(OnDeath));
        }
        else
        {
            StartCoroutine(Flicker());
        }
    }

    public virtual void Revive()
    {
        mHp = mMaxHp;
        gameObject.SetActive(true);
        SetRendererEnabled(true);
        ResetColor();
    }

    public void KnockBack(Vector3 vec)
    {
        /*
        Vector3 dest = mRigidbody.position + vec;
        if(GameManager.Instance.mDungeon.GetCell(dest + vec.normalized * 0.3f).mTileType != DungeonCell.TileType.Wall)
        {
            mRigidbody.MovePosition(dest);
        }
        */
        MoveBy(Vector3.Scale(vec, Vector3.right));
        MoveBy(Vector3.Scale(vec, Vector3.forward));
    }

    public virtual void OnDeath()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        /*
        Vector3 dest = transform.position + mMoveDir * mSpeedFactor * mMaxSpeed * Time.deltaTime;
        if(GameManager.Instance.mDungeon.GetCell(dest + mMoveDir * 0.3f).mTileType != DungeonCell.TileType.Wall)
        {
            transform.position = dest;
        }
        */
        Vector3 moveDelta = mMoveDir * mSpeedFactor * mMaxSpeed * Time.deltaTime;
        MoveBy(Vector3.Scale(moveDelta, Vector3.right));
        MoveBy(Vector3.Scale(moveDelta, Vector3.forward));
    }

    protected virtual void MoveBy(Vector3 vec)
    {
        Vector3 dest = transform.position + vec;
        if (GameManager.Instance.mDungeon.GetCell(dest + vec.normalized * kRadius).mTileType != DungeonCell.TileType.Wall)
        {
            transform.position = dest;
        }
    }

    protected virtual void FixedUpdate()
    {
        if (GameManager.Instance.mDungeon.GetCell(mRigidbody.position).mTileType == DungeonCell.TileType.Wall)
        {
            mRigidbody.position = mLastPos;
        }

        mLastPos = mRigidbody.position;
    }

    public void LoseHearts(int numHearts)
    {
        Vector3 startPos = transform.position + transform.up * 0.5f;
        while (numHearts > 0)
        {
            int value = 1;
            float scale = 1.0f;
            if (numHearts > 375)
            {
                value = 100;
                scale = 2.0f;
            }
            else if (numHearts > 125)
            {
                value = 50;
                scale = 1.75f;
            }
            else if (numHearts > 100)
            {
                value = 25;
                scale = 1.5f;
            }
            else if (numHearts > 10)
            {
                value = 10;
                scale = 1.25f;
            }

            Vector3 dir = Random.onUnitSphere;
            dir.y = 0;
            dir.Normalize();
            Vector3 spawnPos = startPos + dir * 0.5f;
            GameObject projectileObj = GameManager.Instance.SpawnPrefab(mHeartProjectilePrefab, spawnPos, Quaternion.identity);
            HeartProjectile projectile = projectileObj.GetComponent<HeartProjectile>();
            projectile.mHeartValue = value;
            projectile.transform.localScale *= scale;
            projectile.Throw(null, spawnPos, dir, Random.Range(3.0f, 10.0f), false);

            numHearts -= value;
        }
    }
}
