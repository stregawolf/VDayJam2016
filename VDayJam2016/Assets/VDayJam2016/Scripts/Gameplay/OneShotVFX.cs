using UnityEngine;
using System.Collections;

public class OneShotVFX : MonoBehaviour {
    protected void Awake()
    {
        ParticleSystem particleSystem = GetComponent<ParticleSystem>();
        Destroy(gameObject, particleSystem.duration);
    }
}
