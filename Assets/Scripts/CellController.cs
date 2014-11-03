/**
 * Copyright 2014 Sichevoid <sichevoid@gmail.com>.
 *
 * Script handing play grid cell.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CellController : MonoBehaviour {

	List<Renderer> enemyRenderers; 

	public enum Type{
		EMPTY,
		ENEMY,
		PLAYER,
		EXIT
	};

	Type type = Type.EMPTY;

	Color myColor;

	bool change = false;

	// Use this for initialization
	void Start () {
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		enemyRenderers = new List<Renderer> ();
		foreach (Renderer renderer in renderers) {
			if (renderer.CompareTag("Enemy")) {
				enemyRenderers.Add(renderer);
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Type.ENEMY == type) {
			animateEnemy();
		}
		if (change) {
			change = false;
		}
	}

	public void SetType(Type type) {
		this.type = type;
		// set sprite
		if (Type.ENEMY == type) {
			showEnemy(true);
		} else if (Type.EMPTY == type){
			showEnemy(false);
		} else {
			showEnemy(false);
		}
		change = true;
	}
	
	public Type GetCellType() {
		return type;
	}

	void animateEnemy() {

	}
	
	void showEnemy(bool show) {
		foreach (Renderer renderer in enemyRenderers) {
			renderer.enabled = show;
		}
	}
}
