using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public void LoadLevel(string name) {
		Debug.Log("Button clicked " + name);
		Application.LoadLevel(name);
	}
	
	public void QuitRequest(string name) {
		Debug.Log("Button clicked " + name);
		Application.Quit();
	}
}
