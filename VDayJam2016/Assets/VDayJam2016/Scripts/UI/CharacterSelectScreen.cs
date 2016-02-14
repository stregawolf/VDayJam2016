using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class CharacterSelectScreen : MonoBehaviour {

    public GameObject mStartButton;
    private CharacterSelector mSelectedCharacter = null;

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
                        mStartButton.SetActive(true);
                    }
                }
            }
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
}
