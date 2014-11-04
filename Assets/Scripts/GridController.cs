/**
 * Copyright 2014 Sichevoid <sichevoid@gmail.com>.
 *
 * Script handing play grid.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridController : MonoBehaviour
{
	public GameObject cell;
	public GameObject exit;
	public float cellSpacing = 5.0f;
	public Renderer cellBgRenderer;
		
	[Range(9, 100)] public int lines = 9;
	[Range(9, 100)] public int columns = 16;

	GameObject[,] mygrid;
	float cellSize;

	// Use this for initialization
	void Awake ()
	{
		mygrid = new GameObject[lines, columns];
		cellSize = cellBgRenderer.bounds.size.x;
		for (int i = 0; i < lines; ++i) {
			for (int j = 0; j < columns; ++j) {
				GameObject obj = (GameObject)Instantiate (cell, new Vector3 (j * (cellSize + cellSpacing), i * (cellSize + cellSpacing), 0), Quaternion.identity);
				mygrid [i, j] = obj;
			}
		}
	}
	
	bool restart = false;
	int nextLevel = 0;
	float restartTimer;
	public float restartDelay = 10f;
	
	void Update()
	{
		if (restart)
		{
			restartTimer += Time.deltaTime;
			
			if (restartTimer >= restartDelay)
			{
				Application.LoadLevel(nextLevel);
			}
		}
	}
	
	public void Restart(int level) {
		restart = true;
		nextLevel = level;
	}
	
	public GameObject[,] GetGrid() {
		return mygrid;
	}
	
	public void DrawExit(int line, int column) {
		if (line == lines -1) {
			//draw exit on top
			GameObject bottom = mygrid[line, column];
			float x = bottom.transform.position.x;
			float y = bottom.transform.position.y + cellSize/2 + cellSpacing * 2;
			GameObject realExit = (GameObject) Instantiate(exit, new Vector3(x, y, 0), Quaternion.FromToRotation(new Vector3(1, 0, 0), new Vector3(0, 1, 0)));
		} else {
			GameObject bottom = mygrid[line, column];
			float x = bottom.transform.position.x + cellSize/2 + cellSpacing * 2;
			float y = bottom.transform.position.y;
			GameObject realExit = (GameObject) Instantiate(exit, new Vector3(x, y, 0), Quaternion.identity);
		}
	}
}
