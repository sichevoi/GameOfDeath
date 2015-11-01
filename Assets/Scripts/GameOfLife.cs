/**
 * Copyright 2014 Sichevoid <sichevoid@gmail.com>.
 *
 * Script handing Conwey's Game Of Life rules.
 */
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameOfLife : MonoBehaviour
{
	public Text levelLabel;
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
	
	GridController _gridController;

	// Initialize the game
	void Start ()
	{
		_gridController = GetComponent<GridController> ();
		
		lines = SIZE;
		columns = SIZE;
		activeGame = new bool[SIZE, SIZE];
		comingGame = new bool[SIZE, SIZE];
		
		init (Application.loadedLevel, _gridController.GetGrid());
	}
	
	bool _doStep = false;
	public void DoStep() {
		//_doStep = true;
	}
	
	public void Step() {
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
		levelLabel.text = "Level " + Application.loadedLevel;
//			levelNum = 2;
		if (levelNum == 0) {
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
			level = new Position[] { Position.world(shift + 3, shift + 1), Position.world (shift + 5, shift + 2), Position.world (shift + 3, shift + 2), 
				Position.world (shift + 4, shift + 4), Position.world (shift + 3, shift + 5), 
				Position.world (shift + 3, shift + 6), Position.world (shift + 3, shift + 7) };
			exit = Position.world(shift + 10, shift + 6);
		} else if (levelNum == 1) {
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 X
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 X 0 X
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 X X 0 0 0 0 0 0 X X 0 0 0 0 0 0 0 0 0 0 0 0 X X
			// 0 0 0 0 0 0 0 0 0 0 0 0 X 0 0 0 X 0 0 0 0 X X 0 0 0 0 0 0 0 0 0 0 0 0 X X
			// 0 X X 0 0 0 0 0 0 0 0 X 0 0 0 0 0 X 0 0 0 X X 0 0 0
			// 0 X X 0 0 0 0 0 0 0 0 X 0 0 0 X 0 X X 0 0 0 0 X 0 X
			// 0 0 0 0 0 0 0 0 0 0 0 X 0 0 0 0 0 X 0 0 0 0 0 0 0 X
			// 0 0 0 0 0 0 0 0 0 0 0 0 X 0 0 0 X 0 0 0 0 0 0 0 0 0
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 X X 0 0 0 0 0 0 0 0 0 0 0
			
			int linesShift = 46;
			int columnsShift = 43;
		
			level = new Position[] { Position.world (linesShift + 3, columnsShift + 1), Position.world (linesShift + 4, columnsShift + 1), 
				Position.world (linesShift + 3, columnsShift + 2), Position.world (linesShift + 4, columnsShift + 2), 
				Position.world (linesShift + 2, columnsShift + 11), Position.world (linesShift + 3, columnsShift + 11), Position.world (linesShift + 4, columnsShift + 11),
				Position.world (linesShift + 1, columnsShift + 12), Position.world (linesShift + 5, columnsShift + 12),
				Position.world (linesShift + 0, columnsShift + 13), Position.world (linesShift + 6, columnsShift + 13), 
				Position.world (linesShift + 0, columnsShift + 14), Position.world (linesShift + 6, columnsShift + 14),
				Position.world (linesShift + 3, columnsShift + 15), 
				Position.world (linesShift + 1, columnsShift + 16), Position.world (linesShift + 5, columnsShift + 16), 
				Position.world (linesShift + 2, columnsShift + 17), Position.world (linesShift + 3, columnsShift + 17), Position.world (linesShift + 4, columnsShift + 17),
				Position.world (linesShift + 3, columnsShift + 18), 
				Position.world (linesShift + 4, columnsShift + 21), Position.world (linesShift + 5, columnsShift + 21), Position.world (linesShift + 6, columnsShift + 21),
				Position.world (linesShift + 4, columnsShift + 22), Position.world (linesShift + 5, columnsShift + 22), Position.world (linesShift + 6, columnsShift + 22),
				Position.world (linesShift + 3, columnsShift + 23), Position.world (linesShift + 7, columnsShift + 23), 
				Position.world (linesShift + 2, columnsShift + 25), Position.world (linesShift + 3, columnsShift + 25), Position.world (linesShift + 7, columnsShift + 25), Position.world (linesShift + 8, columnsShift + 25),
				Position.world (linesShift + 5, columnsShift + 35), Position.world (linesShift + 6, columnsShift + 35),
				Position.world (linesShift + 5, columnsShift + 36), Position.world (linesShift + 6, columnsShift + 36)};
			exit = Position.world(linesShift + 3, columnsShift + 12);
		} else {
//			                 5         10        15        20        25
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
			// 0 0 1 1 1 0 0 0 0 0 0 0 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 1 1 1 0 0
			// 0 1 0 0 1 0 0 0 0 0 0 1 1 1 0 0 0 1 1 1 0 0 0 0 0 0 0 1 0 0 1 0
			// 0 0 0 0 1 0 0 0 0 0 1 1 0 1 0 0 0 1 0 1 1 0 0 0 0 0 0 1 0 0 0 0
			// 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0
			// 0 0 0 0 1 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 0 0 0 0
			// 0 0 0 0 1 0 0 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 1 0 0 0 0
			// 0 0 0 1 0 0 0 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 1 0 0 0
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
			
			Vector2[] shiftsArray = new Vector2[] {
				new Vector2(1, 2), new Vector2(1, 3), new Vector2(1, 4), new Vector2(1, 12), new Vector2(1, 18), new Vector2(1, 27), new Vector2(1, 28), new Vector2(1, 29),
				new Vector2(2, 1), new Vector2(2, 5), new Vector2(2, 11), new Vector2(2, 12), new Vector2(2, 13), new Vector2(2, 17), new Vector2(2, 18), new Vector2(2, 19), new Vector2(2, 27), new Vector2(2, 30),
				new Vector2(3, 4), new Vector2(3, 10), new Vector2(3, 11), new Vector2(3, 13), new Vector2(3, 17), new Vector2(3, 19), new Vector2(3, 20), new Vector2(3, 27),
				new Vector2(4, 4), new Vector2(4, 27),
				new Vector2(5, 4), new Vector2(5, 7), new Vector2(5, 23), new Vector2(5, 27), 
				new Vector2(6, 4), new Vector2(6, 7), new Vector2(6, 8), new Vector2(6, 22), new Vector2(6, 23), new Vector2(6, 27),
				new Vector2(7, 3), new Vector2(7, 7), new Vector2(7, 8), new Vector2(7, 22), new Vector2(7, 23), new Vector2(7, 28),
			};
			
			int linesShift = 48;
			int columnsShift = 35;
			
			level = new Position[shiftsArray.Length];
			for (int i = 0; i < shiftsArray.Length; ++i) {
				Vector2 shift = shiftsArray[i];
				level [i] = Position.world(linesShift + (int)shift.x, columnsShift + (int)shift.y);
			}
			exit = Position.world(linesShift + 5, columnsShift + 13);
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
		this.exit = Position.world(exit.getLine(), exit.getColumn());
	}

	// Translate the full grid the visible game objects
	void gameToGrid ()
	{
		int gridLines = objectsMatrix.GetLength (0);
		int gridColumns = objectsMatrix.GetLength (1);
		
		Debug.Log("Exit line = " + exit.getLine() + " column = " + exit.getColumn());
		Debug.Log("Shifts: horizontal " + horizontalShift + " vertical " + verticalShift);
		
		for (int i = 0; i < gridLines; ++i) {
			for (int j = 0; j < gridColumns; ++j) {
				GameObject o = objectsMatrix [i, j];
				CellController cellC = o.GetComponent<CellController> ();
				if (activeGame [horizontalShift + i, verticalShift + j]) {
					cellC.SetType (CellController.Type.ENEMY);
				} else if (exit.getLine() == (i + horizontalShift) && exit.getColumn() == (j + verticalShift)) {
					_gridController.DrawExit(i, j);
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
	
		const int zeroLine = 45;
		const int zeroColumn = 44;
		
		int line;
		int column;
		
		private Position() {
		}
		
		public static Position world(int line, int column)
		{			
			if (column < 0) {
				Debug.Log("Column is negative " + column);			
			}

			Position position = new Position();
			
			position.line = line;
			position.column = column;
			
			return position;
		}
		
		public static Position game(int line, int column) {
			if (column < 0) {
				Debug.Log("Column is negative " + column);			
			}
			
			Position position = new Position();
			
			position.line = zeroLine + line;
			position.column = zeroColumn + column;
			
			return position;
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
		
		public int getGameLine() {
			return column - zeroColumn;
		}
		
		public int getGameColumn() {
			return line - zeroLine;
		}
	}
}
