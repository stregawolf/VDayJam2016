using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TitleScreen : MonoBehaviour {
    public void OnPlayPressed()
    {
        Debug.Log("Play Pressed");
        SceneManager.LoadScene("CharacterSelect");
    }
}
