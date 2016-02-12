using UnityEngine;
using System.Collections;

public class CharacterSelector : MonoBehaviour {
    public SelectedCharacter mCharacter = SelectedCharacter.Rose;

    public GameObject mLight;
    public Animator mAnimator;

    public void Select()
    {
        mLight.SetActive(true);
        mAnimator.SetBool("IsWalking", true);
    }

    public void Deselect()
    {
        mLight.SetActive(false);
        mAnimator.SetBool("IsWalking", false);
    }
}
