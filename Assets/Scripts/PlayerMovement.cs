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
	public GameOfLife gameOfLife;
	public Animator levelCompleteAnim;
	public float restartTime;
	
	float timeElapsed = 0f;
	GameObject[,] _grid;
	int _lines;
	int _columns;
	
	Vector2 _position;
	Vector2 _movement;
	
	bool _isDead = false;
	bool _isComplete = false;
	float _restartTimer = 0.0f;
	
	// Use this for initialization
	void Start () {
		_grid = gridController.GetGrid();
		_lines = gridController.lines;
		_columns = gridController.columns;
		
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
		
		if (_isDead || _isComplete) {
			if (_restartTimer >= restartTime) {
				LevelManager levelManager = FindObjectOfType<LevelManager>() as LevelManager;
				if (_isDead) {
					levelManager.LoadDeath();
				} else {
					levelManager.LoadNextLevel();
				}
			} else {
				_restartTimer += Time.fixedDeltaTime;
			}
		} else {
			// Move the player around the scene.
			PrepareMove (h, v);
			h = 0;
			v = 0;
		}
	}

	// check whether the player is still alive
	// should be called after actual move		
	void CheckAlive() {
		if (_position != null && _grid != null) {
			GameObject targetObject = _grid[(int)_position.y, (int)_position.x];
			CellController cellController = targetObject.GetComponent<CellController>();
			CellController.Type targetType = cellController.GetCellType();
			if (targetType == CellController.Type.ENEMY) {
				PlayerDeath(targetObject);	
			}
		}
	}
		
	void PrepareMove (float h, float v)
	{
		if (h != 0 || v != 0) {
			_movement = Vector2.zero;
			
			if (h > 0) _movement.x = 1;
			else if (h < 0) _movement.x = -1;
			
			if (v > 0) _movement.y = 1;
			else if (v < 0) _movement.y = -1;
			if (timeElapsed == 0) {
				timeElapsed += Time.deltaTime;
			}
		}

		if (timeElapsed > playerSpeed) {
			timeElapsed = 0;
			if (_movement != Vector2.zero) {
				Vector2 newPosition = _position + _movement;
				if (CanMove(newPosition)) {
					DoMove(newPosition);
				}
				_movement = Vector2.zero;
			}
		} else if (timeElapsed > 0){
			timeElapsed += Time.deltaTime;
		}
		
		if (_movement == Vector2.zero) {
			CheckAlive();
		}
	}
	
	void DoMove (Vector2 newPosition) {
		PositionToGrid(newPosition);
		gameOfLife.Step();
	}
	
	bool CanMove(Vector2 position) {
		return (position.x < _columns && position.y < _lines) && (position.x >= 0 && position.y >= 0); 
	}
	
	void PositionToGrid(Vector2 position) {
		GameObject targetObject = _grid[(int)position.y, (int)position.x];
		CellController cellController = targetObject.GetComponent<CellController>();
		CellController.Type targetType = cellController.GetCellType();
		if (targetType == CellController.Type.ENEMY) {
			PlayerDeath(targetObject);	
		} else if (targetType == CellController.Type.EXIT) {
			LevelComplete(targetObject);
			MoveToCell(targetObject);
			this._position = position;
		} else {
			MoveToCell(targetObject);
			this._position = position;
		}
	}
	
	public void PlayerDeath(GameObject deathCell) {
		Renderer[] renderers = GetComponentsInChildren<Renderer> ();
		foreach(Renderer renderer in renderers) {
			renderer.enabled = false;
		}
		levelCompleteAnim.SetTrigger("GameOver");
		_isDead = true;
	}
	
	void LevelComplete(GameObject exitCell) {
		levelCompleteAnim.SetTrigger("LevelComplete");
		_isComplete = true;
	}
	
	void MoveToCell(GameObject newCell) {
		transform.position = newCell.transform.position;
	}
}
