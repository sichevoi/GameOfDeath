/**
 * Copyright 2014 Sichevoid <sichevoid@gmail.com>.
 *
 * Script handing Conwey's Game Of Life rules.
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameOfLife : MonoBehaviour
{
	public float updatePeriod = 0.5f;

	static public int MAX_LEVEL = 1;
	
	int SIZE = 102;

	int lines;
	int columns;
	
	int horizontalShift;
	int verticalShift;

	bool[,] activeGame;
	bool[,] comingGame;
	Position exit;

	GameObject[,] objectsMatrix;

	float timeExpired = 0f;

	// Initialize the game
	void Start ()
	{
		GridController gridController = GetComponent<GridController> ();
		
		lines = SIZE;
		columns = SIZE;
		activeGame = new bool[SIZE, SIZE];
		comingGame = new bool[SIZE, SIZE];
		
		init (Application.loadedLevel, gridController.GetGrid());
		gridController.DrawExit(exit.getLine(), exit.getColumn());
		//gridController.DrawExit(8, 11);
	}
	
	bool _doStep = false;
	public void DoStep() {
		_doStep = true;
	}

	// Called once per fixed frame
	// We make a step in the GoL each updatePeriod seconds.
	void FixedUpdate ()
	{
		if (_doStep) {
			_doStep = false;
			iterate ();
			gameToGrid ();
		} else {
			timeExpired += Time.deltaTime;
		}
	}

	// Initialize the GoL
	public void init (int levelNum, GameObject[,] objects)
	{
			Position[] level;
			Position exit;
			
			Debug.Log("Application loadedLevel is " + Application.loadedLevel);

			if (Application.loadedLevel == 0) {
				// O O O O O O O O O O
				// O O O O O O O O O O 
				// O O O O O O O O O O 
				// O O X O O O O O O O 
				// O O O O X O O O O O 
				// O X X O O X X X O O 
				// O O O O O O O O O O 
				// O O O O O O O O O O 
				// O O O O O O O O O O
				int shift = 43;
				level = new Position[] { new Position (shift + 3, shift + 1), new Position (shift + 5, shift + 2), new Position (shift + 3, shift + 2), 
														new Position (shift + 4, shift + 4), new Position (shift + 3, shift + 5), 
														new Position (shift + 3, shift + 6), new Position (shift + 3, shift + 7) };
				exit = new Position(shift + 10, shift + 6);
			} else {
				// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 X
				// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 X 0 X
				// 0 0 0 0 0 0 0 0 0 0 0 0 0 X X 0 0 0 0 0 0 X X 0 0 0 0 0 0 0 0 0 0 0 0 X X
				// 0 0 0 0 0 0 0 0 0 0 0 0 X 0 0 0 X 0 0 0 0 X X 0 0 0 0 0 0 0 0 0 0 0 0 X X
				// 0 X X 0 0 0 0 0 0 0 0 X 0 0 0 0 0 X 0 0 0 X X 0 0 0
				// 0 X X 0 0 0 0 0 0 0 0 X 0 0 0 X 0 X X 0 0 0 0 X 0 X
				// 0 0 0 0 0 0 0 0 0 0 0 X 0 0 0 0 0 X 0 0 0 0 0 0 0 X
				// 0 0 0 0 0 0 0 0 0 0 0 0 X 0 0 0 X 0 0 0 0 0 0 0 0 0
				// 0 0 0 0 0 0 0 0 0 0 0 0 0 X X 0 0 0 0 0 0 0 0 0 0 0
				
				int linesShift = 43;
				int columnsShift = 30;
			
				level = new Position[] { new Position (linesShift + 3, columnsShift + 1), new Position (linesShift + 4, columnsShift + 1), 
										new Position (linesShift + 3, columnsShift + 2), new Position (linesShift + 4, columnsShift + 2), 
										new Position (linesShift + 2, columnsShift + 11), new Position (linesShift + 3, columnsShift + 11), new Position (linesShift + 4, columnsShift + 11),
				new Position (linesShift + 1, columnsShift + 12), new Position (linesShift + 5, columnsShift + 12),
				new Position (linesShift + 0, columnsShift + 13), new Position (linesShift + 6, columnsShift + 13), 
				new Position (linesShift + 0, columnsShift + 14), new Position (linesShift + 6, columnsShift + 14),
				new Position (linesShift + 3, columnsShift + 15), 
				new Position (linesShift + 1, columnsShift + 16), new Position (linesShift + 5, columnsShift + 16), 
				new Position (linesShift + 2, columnsShift + 17), new Position (linesShift + 3, columnsShift + 17), new Position (linesShift + 4, columnsShift + 17),
				new Position (linesShift + 3, columnsShift + 18), 
				new Position (linesShift + 4, columnsShift + 21), new Position (linesShift + 5, columnsShift + 21), new Position (linesShift + 6, columnsShift + 21),
				new Position (linesShift + 4, columnsShift + 22), new Position (linesShift + 5, columnsShift + 22), new Position (linesShift + 6, columnsShift + 22),
				new Position (linesShift + 3, columnsShift + 23), new Position (linesShift + 7, columnsShift + 23), 
				new Position (linesShift + 2, columnsShift + 25), new Position (linesShift + 3, columnsShift + 25),new Position (linesShift + 7, columnsShift + 25), new Position (linesShift + 8, columnsShift + 25),
				new Position (linesShift + 5, columnsShift + 35), new Position (linesShift + 6, columnsShift + 35),
				new Position (linesShift + 5, columnsShift + 36), new Position (linesShift + 6, columnsShift + 36)};
				exit = new Position(linesShift + 10, columnsShift + 8);
			}

			objectsMatrix = objects;
			
			horizontalShift = (SIZE - objects.GetLength(0)) / 2 - 1;
			verticalShift = (SIZE - objects.GetLength(1)) / 2 - 1;

			initGame (level, exit);
			gameToGrid ();
	}

	// Put the initial inhabited points to the game grid
	void initGame (Position[] alivePositions, Position exit)
	{
		for (int i = 0; i < alivePositions.Length; ++i) {
			Position position = alivePositions [i];
			int line = position.getLine ();
			int column = position.getColumn ();
			activeGame [line, column] = true;
		}
		this.exit = new Position(exit.getLine() - horizontalShift, exit.getColumn() - verticalShift);
	}

	// Translate the full grid the visible game objects
	void gameToGrid ()
	{
		int gridLines = objectsMatrix.GetLength (0);
		int gridColumns = objectsMatrix.GetLength (1);
		
		for (int i = 0; i < gridLines; ++i) {
			for (int j = 0; j < gridColumns; ++j) {
				GameObject o = objectsMatrix [i, j];
				CellController cellC = o.GetComponent<CellController> ();
				if (activeGame [horizontalShift + i, verticalShift + j]) {
					cellC.SetType (CellController.Type.ENEMY);
				} else if (exit.getLine() == i && exit.getColumn() == j) {
					cellC.SetType(CellController.Type.EXIT);
				} else {
					cellC.SetType (CellController.Type.EMPTY);
				}
			}
		}
		
	}

	// Run an interation of Game of Life
	void iterate ()
	{
		for (int i = 0; i < lines; ++i) {
			for (int j = 0; j < columns; ++j) {
				int an = aliveNeighb (i, j);
				comingGame [i, j] = isAlive (activeGame [i, j], an);
			}
		}
		swapGames ();
	}

	void swapGames ()
	{
		bool[,] tmp = activeGame;
		activeGame = comingGame;
		comingGame = tmp;
	}

	//Any live cell with fewer than two live neighbours dies, as if caused by under-population.
	//Any live cell with two or three live neighbours lives on to the next generation.
	//Any live cell with more than three live neighbours dies, as if by overcrowding.
	//Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.
	bool isAlive (bool current, int numNeighb)
	{
		if ((current && numNeighb == 2) || numNeighb == 3) {
			return true;
		}
		return false;
	}

	// (i + 1, j - 1) (i + 1, j) (i + 1, j + 1)
	// (i, j - 1)     (i,j)      (i, j +1 )
	// (i - 1, j - 1) (i - 1, j) (i - 1, j + 1)
	int aliveNeighb (int i, int j)
	{
		return bottomLeft (i, j) + bottom (i, j) + bottomRight (i, j) + right (i, j) + topRight (i, j) + top (i, j) + topLeft (i, j) + left (i, j);
	}

	//  (i + 1, j - 1) (i + 1, j) (i + 1, j + 1)
	//  (i, j - 1)     (i,j)      (i, j +1 )
	// *(i - 1, j - 1) (i - 1, j) (i - 1, j + 1)
	int bottomLeft (int i, int j)
	{
		if (i > 0 && j > 0) {
			return getInt(activeGame [i - 1, j - 1]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1)  (i + 1, j) (i + 1, j + 1)
	// (i, j - 1)      (i,j)      (i, j +1 )
	// (i - 1, j - 1) *(i - 1, j) (i - 1, j + 1)
	int bottom (int i, int j)
	{
		if (i > 0) {
			return getInt(activeGame [i - 1, j]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) (i + 1, j)  (i + 1, j + 1)
	// (i, j - 1)     (i,j)       (i, j +1 )
	// (i - 1, j - 1) (i - 1, j) *(i - 1, j + 1)
	int bottomRight (int i, int j)
	{
		if (i > 0 && j < columns - 1) {
			return getInt(activeGame [i - 1, j + 1]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) (i + 1, j)  (i + 1, j + 1)
	// (i, j - 1)     (i,j)      *(i, j +1 )
	// (i - 1, j - 1) (i - 1, j)  (i - 1, j + 1)
	int right (int i, int j)
	{
		if (j < columns - 1) {
			return getInt(activeGame [i, j + 1]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) (i + 1, j) *(i + 1, j + 1)
	// (i, j - 1)     (i,j)       (i, j +1 )
	// (i - 1, j - 1) (i - 1, j)  (i - 1, j + 1)
	int topRight (int i, int j)
	{
		if (i < lines - 1 && j < columns - 1) {
			return getInt(activeGame [i + 1, j + 1]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) *(i + 1, j) (i + 1, j + 1)
	// (i, j - 1)      (i,j)      (i, j +1 )
	// (i - 1, j - 1)  (i - 1, j) (i - 1, j + 1)
	int top (int i, int j)
	{
		if (i < lines - 1) {
			return getInt(activeGame [i + 1, j]);
		} else {
			return 0;
		}
	}

	// *(i + 1, j - 1) (i + 1, j) (i + 1, j + 1)
	//  (i, j - 1)     (i,j)      (i, j +1 )
	//  (i - 1, j - 1) (i - 1, j) (i - 1, j + 1)
	int topLeft (int i, int j)
	{
		if (i < lines - 1 && j > 0) {
			return getInt(activeGame [i + 1, j - 1]);
		} else {
			return 0;
		}
	}

	//  (i + 1, j - 1) (i + 1, j) (i + 1, j + 1)
	// *(i, j - 1)     (i,j)      (i, j +1 )
	//  (i - 1, j - 1) (i - 1, j) (i - 1, j + 1)
	int left (int i, int j)
	{
		if (j > 0) {
			return getInt(activeGame [i, j - 1]);
		} else {
			return 0;
		}
	}
	
	int getInt(bool val) {
		return val ? 1 : 0;
	}

	public class Position
	{
		int line;
		int column;
		public Position (int line, int column)
		{
			if (column < 0) {
				Debug.Log("Column is negative " + column);			
			}
			this.line = line;
			this.column = column;
		}

		public Position (int[] position)
		{
			this.line = position [0];
			this.column = position [1];
		}

		public int getLine ()
		{
			return line;
		}

		public int getColumn ()
		{
			return column;
		}
	}
}
