using UnityEngine;
using System.Collections;

public class OneShotVFX : MonoBehaviour {
    protected ParticleSystem mParticleSystem;
	
    protected void Awake()
    {
        mParticleSystem = GetComponent<ParticleSystem>();
    }

    protected void Update()
    {
        if(!mParticleSystem.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
