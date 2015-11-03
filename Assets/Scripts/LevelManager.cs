using UnityEngine;
using System.Collections;

public class LevelManager : MonoBehaviour {

	public static string MENU_SCENE_NAME = "StartMenu";
	public static string GAME_SCENE_NAME = "Game";
	public static string WIN_SCENE_NAME = "TheWin";
	public static string DEATH_SCENE_NAME = "TheDeath";
	
	public void LoadStartMenu() {
		Application.LoadLevel(MENU_SCENE_NAME);
	}
	
	public void LoadStartLevel() {
		Toolbox.Instance.currentLevel = 0;
		Application.LoadLevel(GAME_SCENE_NAME);
	}
	
	public void LoadNextLevel() {
		Toolbox toolbox = Toolbox.Instance;
		if (toolbox.currentLevel < Toolbox.MAX_LEVEL) {
			++toolbox.currentLevel;
			Application.LoadLevel(GAME_SCENE_NAME);
		} else {
			Application.LoadLevel(WIN_SCENE_NAME);
		}
	}
	
	public void LoadDeath() {
		Application.LoadLevel(DEATH_SCENE_NAME);
	}
	
	public void RestartLevel() {
		Application.LoadLevel(GAME_SCENE_NAME);
	}
	
	public void QuitRequest(string name) {
		Debug.Log("Button clicked " + name);
		Application.Quit();
	}
}
