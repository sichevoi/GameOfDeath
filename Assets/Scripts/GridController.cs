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
	public float cellSize = 2.0f;
	public float cellSpacing = 5.0f;
	[Range(9, 100)] public int lines = 9;
	[Range(9, 100)] public int columns = 16;

	GameObject[,] mygrid;

	// Use this for initialization
	void Awake ()
	{
		mygrid = new GameObject[lines, columns];
		for (int i = 0; i < lines; ++i) {
			for (int j = 0; j < columns; ++j) {
				GameObject obj = (GameObject)Instantiate (cell, new Vector3 (j * (cellSize + cellSpacing), i * (cellSize + cellSpacing), 0), Quaternion.identity);
				mygrid [i, j] = obj;
			}
		}
	}
	
	public GameObject[,] GetGrid() {
		return mygrid;
	}
}
