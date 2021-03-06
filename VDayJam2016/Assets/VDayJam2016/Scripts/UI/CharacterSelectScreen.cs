﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelectScreen : MonoBehaviour {

    public GameObject mStartButton;

    public GameObject mVuButton;
    public GameObject mRoseButton;

    public GameObject mUnselectedImage;
    public GameObject mVuSelectedImage;
    public GameObject mRoseSelectedImage;

    public Fader mFader;

    private CharacterSelector mSelectedCharacter = null;


    private void Awake()
    {
        GlobalData.sSelectedCharacter = SelectedCharacter.None;
        UpdateDisplay();

        if (mFader == null)
        {
            mFader = FindObjectOfType<Fader>();
        }
        mFader.FadeIn();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(inputRay, out hit, Mathf.Infinity))
            {
                CharacterSelector character = hit.collider.GetComponentInParent<CharacterSelector>();
                if(character != null)
                {
                    if(mSelectedCharacter != null)
                    {
                        mSelectedCharacter.Deselect();
                    }
                    mSelectedCharacter = character;
                    if(mSelectedCharacter != null)
                    {
                        SoundManager.Instance.PlayMenuConfirmSFX();
                        GlobalData.sSelectedCharacter = mSelectedCharacter.mCharacter;
                        mSelectedCharacter.Select();
                        UpdateDisplay();
                        mStartButton.SetActive(true);
                    }
                }
            }
        }
    }

    public void UpdateDisplay()
    {
        mUnselectedImage.SetActive(false);
        mVuSelectedImage.SetActive(false);
        mRoseSelectedImage.SetActive(false);
        switch(GlobalData.sSelectedCharacter)
        {
            case SelectedCharacter.None:
                mUnselectedImage.SetActive(true);
                mVuButton.transform.SetParent(mUnselectedImage.transform);
                mRoseButton.transform.SetParent(mUnselectedImage.transform);
                break;
            case SelectedCharacter.Rose:
                mRoseSelectedImage.SetActive(true);
                mVuButton.transform.SetParent(mRoseSelectedImage.transform);
                mRoseButton.transform.SetParent(mRoseSelectedImage.transform);
                break;
            case SelectedCharacter.Vu:
                mVuSelectedImage.SetActive(true);
                mVuButton.transform.SetParent(mVuSelectedImage.transform);
                mRoseButton.transform.SetParent(mVuSelectedImage.transform);
                break;
        }
    }

    public void OnBackPressed()
    {
        SoundManager.Instance.PlayMenuDeclineSFX();
        mFader.FadeOut(() => SceneManager.LoadScene("Title"));
    }

    public void OnStartPressed()
    {
        SoundManager.Instance.FadeOut(.4f);
        SoundManager.Instance.PlayMenuConfirmSFX();
        GlobalData.ResetData();
        mFader.FadeOut(() => SceneManager.LoadScene("Game"));
    }

    public void OnVuPressed()
    {
        SoundManager.Instance.PlayMenuConfirmSFX();
        GlobalData.sSelectedCharacter = SelectedCharacter.Vu;
        UpdateDisplay();
        mStartButton.SetActive(true);
    }

    public void OnRosePressed()
    {
        SoundManager.Instance.PlayMenuConfirmSFX();
        GlobalData.sSelectedCharacter = SelectedCharacter.Rose;
        UpdateDisplay();
        mStartButton.SetActive(true);
    }
}
