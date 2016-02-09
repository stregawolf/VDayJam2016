using UnityEngine;
using System.Collections;

public class BaseActor : MonoBehaviour {
    public int mMaxHp = 10;
    public float mMaxSpeed = 5.0f;
    public bool mbFaceMovementDirection = true;

    public GameObject mModel;

    public Animator mAnimator;
    protected string mPlayedAnimationName;
    public Rigidbody mRigidbody;
    public Collider mCollider;
    public Renderer[] mRenderers;
    protected Color[] mOriginalColors;

    protected int mHp = 0;
    public int Hp { get { return mHp; } }

    protected Vector3 mMoveDir;
    protected float mSpeedFactor = 1.0f;

    protected bool mIsFlickering = false;
    public bool IsFlickering {  get { return mIsFlickering; } }

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
        for (int i = 0; i < 10; ++i)
        {
            SetColor(new Color(Random.value + 0.5f, Random.value + 0.5f, Random.value + 0.5f));
            yield return new WaitForSeconds(.05f);
        }

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

    public void KnockBack(Vector3 vec)
    {
        Vector3 dest = mRigidbody.position + vec;
        if(IsValidPosition(dest))
        {
            mRigidbody.MovePosition(dest);
        }
    }

    public virtual void OnDeath()
    {
        gameObject.SetActive(false);
    }

    protected virtual void Update()
    {
        Vector3 dest = transform.position + mMoveDir * mSpeedFactor * mMaxSpeed * Time.deltaTime;
        if(IsValidPosition(dest))
        {
            transform.position = dest;
        }
        //mRigidbody.MovePosition(mRigidbody.position + mMoveDir * mSpeedFactor * mMaxSpeed * Time.deltaTime);
    }

    public bool IsValidPosition(Vector3 pos)
    {
        return GameManager.Instance.mDungeon.GetCell(pos).mTileType != DungeonCell.TileType.Wall;
    }
}
