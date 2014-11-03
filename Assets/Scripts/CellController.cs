/**
 * Copyright 2014 Sichevoid <sichevoid@gmail.com>.
 *
 * Script handing play grid cell.
 */
using UnityEngine;
using System.Collections;

public class CellController : MonoBehaviour {

	SpriteRenderer spriteRenderer; 

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
		spriteRenderer = GetComponent<SpriteRenderer> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Type.ENEMY == type) {
			animateEnemy();
		}
		if (change) {
			spriteRenderer.color = myColor;
			change = false;
		}
	}

	public void SetType(Type type) {
		this.type = type;
		// set sprite
		if (Type.ENEMY == type) {
			myColor = Color.black;
		} else if (Type.EMPTY == type){
			myColor = Color.red;
		} else {
			myColor = Color.blue;
		}
		change = true;
	}
	
	public Type GetCellType() {
		return type;
	}

	void animateEnemy() {

	}
}
