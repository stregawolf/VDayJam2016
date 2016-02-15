using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelectScreen : MonoBehaviour {

    public GameObject mStartButton;
    public GameObject mUnselectedImage;
    public GameObject mVuSelectedImage;
    public GameObject mRoseSelectedImage;

    private CharacterSelector mSelectedCharacter = null;

    private void Awake()
    {
        GlobalData.sSelectedCharacter = SelectedCharacter.None;
        UpdateDisplay();
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
                break;
            case SelectedCharacter.Rose:
                mRoseSelectedImage.SetActive(true);
                break;
            case SelectedCharacter.Vu:
                mVuSelectedImage.SetActive(true);
                break;
        }
    }

    public void OnBackPressed()
    {
        SceneManager.LoadScene("Title");
    }

    public void OnStartPressed()
    {
        GlobalData.ResetData();
        SceneManager.LoadScene("Game");
    }

    public void OnVuPressed()
    {
        GlobalData.sSelectedCharacter = SelectedCharacter.Vu;
        UpdateDisplay();
        mStartButton.SetActive(true);
    }

    public void OnRosePressed()
    {
        GlobalData.sSelectedCharacter = SelectedCharacter.Rose;
        UpdateDisplay();
        mStartButton.SetActive(true);
    }
}
