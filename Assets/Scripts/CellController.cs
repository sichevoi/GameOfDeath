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
	Animator cellAnimator;
	
	public enum Type{
		EMPTY,
		ENEMY,
		PLAYER,
		EXIT
	};

	Type type = Type.EMPTY;

	Color myColor;

	// Use this for initialization
	void Start () {
		cellAnimator = GetComponent<Animator> ();
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
	}

	public void SetType(Type type) {
		this.type = type;
		// set sprite
		if (Type.ENEMY == type) {
			ShowEnemy(true);
		} else if (Type.EMPTY == type){
			ShowEnemy(false);
		} else {
			ShowEnemy(false);
		}
	}
	
	public Type GetCellType() {
		return type;
	}
	
	void ShowEnemy(bool show) {
		foreach (Renderer renderer in enemyRenderers) {
			renderer.enabled = show;
		}
		cellAnimator.SetBool("IsEnemy", show);
	}
}
