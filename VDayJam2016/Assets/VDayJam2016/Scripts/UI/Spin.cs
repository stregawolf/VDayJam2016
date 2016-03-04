using UnityEngine;
using System.Collections;

public class Spin : MonoBehaviour {
	public float mSpinSpeed = 10.0f;
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.forward, mSpinSpeed * Time.deltaTime);
	}
}
