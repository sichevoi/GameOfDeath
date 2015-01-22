﻿/**
 * Copyright 2014 Sichevoid <sichevoid@gmail.com>.
 *
 * Script handing player movement.
 */
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float playerSpeed = 0.5f;
	public GridController gridController;
	public Animator levelCompleteAnim;
	
	float timeElapsed = 0f;
	GameObject[,] grid;
	int lines;
	int columns;
	
	Vector2 position;
	Vector2 movement;
	
	// Use this for initialization
	void Start () {
		grid = gridController.GetGrid();
		lines = gridController.lines;
		columns = gridController.columns;
		
		PositionToGrid(Vector2.zero);
	}
	
	float h = 0;
	float v = 0;
	
	public void OnClickHorizontal(int direction) {
		h = direction;
	}

	public void OnClickVertical(int direction) {
		v = direction;
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		CheckAlive();
		
		// Move the player around the scene.
		Move (h, v);
		
		h = 0;
		v = 0;
	}
		
	void CheckAlive() {
		GameObject targetObject = grid[(int)position.y, (int)position.x];
		CellController cellController = targetObject.GetComponent<CellController>();
		CellController.Type targetType = cellController.GetCellType();
		if (targetType == CellController.Type.ENEMY) {
			PlayerDeath(targetObject);	
		}
	}
		
	void Move (float h, float v)
	{
		if (h != 0 || v != 0) {
			movement = Vector2.zero;
			
			if (h > 0) movement.x = 1;
			else if (h < 0) movement.x = -1;
			
			if (v > 0) movement.y = 1;
			else if (v < 0) movement.y = -1;
			if (timeElapsed == 0) {
				timeElapsed += Time.deltaTime;
			}
		}

		if (timeElapsed > playerSpeed) {
			timeElapsed = 0;
			if (movement != Vector2.zero) {
				Vector2 newPosition = position + movement;
				if (CanMove(newPosition)) {
					PositionToGrid(newPosition);
				}
				movement = Vector2.zero;
			}
		} else if (timeElapsed > 0){
			timeElapsed += Time.deltaTime;
		}
	}
	
	bool CanMove(Vector2 position) {
		return (position.x < columns && position.y < lines) && (position.x >= 0 && position.y >= 0); 
	}
	
	void PositionToGrid(Vector2 position) {
		GameObject targetObject = grid[(int)position.y, (int)position.x];
		CellController cellController = targetObject.GetComponent<CellController>();
		CellController.Type targetType = cellController.GetCellType();
		if (targetType == CellController.Type.ENEMY) {
			PlayerDeath(targetObject);	
		} else if (targetType == CellController.Type.EXIT) {
			LevelComplete(targetObject);
			MoveToCell(targetObject);
			this.position = position;
		} else {
			MoveToCell(targetObject);
			this.position = position;
		}
	}
	
	public void PlayerDeath(GameObject deathCell) {
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach(Renderer renderer in renderers) {
			renderer.enabled = false;
		}
		levelCompleteAnim.SetTrigger("GameOver");
	}
	
	void LevelComplete(GameObject exitCell) {
		levelCompleteAnim.SetTrigger("LevelComplete");
		gridController.Restart(Application.loadedLevel + 1);
	}
	
	void MoveToCell(GameObject newCell) {
		transform.position = newCell.transform.position;
	}
}
