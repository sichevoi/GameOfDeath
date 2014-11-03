/**
 * Copyright 2014 Sichevoid <sichevoid@gmail.com>.
 *
 * Script handing player movement.
 */
using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour {
	public float playerSpeed = 0.5f;
	public GridController gridController;
	
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
	
	// Update is called once per frame
	void FixedUpdate () {
		// Store the input axes.
		float h = Input.GetAxisRaw ("Horizontal");
		float v = Input.GetAxisRaw ("Vertical");
		
		CheckAlive();
		
		// Move the player around the scene.
		Move (h, v);
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
		return position.x < columns && position.y < lines; 
	}
	
	void PositionToGrid(Vector2 position) {
		GameObject targetObject = grid[(int)position.y, (int)position.x];
		CellController cellController = targetObject.GetComponent<CellController>();
		CellController.Type targetType = cellController.GetCellType();
		if (targetType == CellController.Type.ENEMY) {
			PlayerDeath(targetObject);	
		} else if (targetType == CellController.Type.EXIT) {
			LevelComplete(targetObject);
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
	}
	
	void LevelComplete(GameObject exitCell) {

	}
	
	void MoveToCell(GameObject newCell) {
		transform.position = newCell.transform.position;
	}
}
