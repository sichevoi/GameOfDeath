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

	int _lines;
	int _columns;
	
	int _horizontalShift;
	int _verticalShift;

	bool[,] _activeGame;
	bool[,] _comingGame;
	Position _exit;

	GameObject[,] _objectsMatrix;

	float _timeExpired = 0f;
	
	GridController _gridController;
	
	bool _doStep = false;

	// Initialize the game
	void Start ()
	{
		_gridController = GetComponent<GridController> ();
		
		_lines = SIZE;
		_columns = SIZE;
		_activeGame = new bool[SIZE, SIZE];
		_comingGame = new bool[SIZE, SIZE];
		
		int levelNum = Toolbox.Instance.currentLevel;
		init (levelNum, _gridController.GetGrid());
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
			_timeExpired += Time.deltaTime;
		}
	}

	// Initialize the GoL
	public void init (int levelNum, GameObject[,] objects)
	{
		Position[] level;
		Position exit;
		
		
		IDictionary<int, int[]> shiftsMap = new Dictionary<int, int[]>();
		int linesShift;
		int columnsShift;
		
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
			linesShift = 43;
			columnsShift = 43;
			
			shiftsMap.Add(new KeyValuePair<int, int[]> (3, new int[] { 1, 2, 5, 6, 7 }));
			shiftsMap.Add(new KeyValuePair<int, int[]> (4, new int[] { 4 }));
			shiftsMap.Add(new KeyValuePair<int, int[]> (5, new int[] { 2 }));
			exit = Position.world(linesShift + 10, columnsShift + 6);
			
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
			
			linesShift = 46;
			columnsShift = 43;
		
			shiftsMap.Add(new KeyValuePair<int, int[]>(0, new int[] { 13, 14 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(1, new int[] { 12, 16 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(2, new int[] { 11, 17, 25 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(3, new int[] { 1, 2, 11, 15, 17, 18, 23, 25}));
			shiftsMap.Add(new KeyValuePair<int, int[]>(4, new int[] { 1, 2, 11, 17, 21, 22 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(5, new int[] { 12, 16, 21, 22, 35, 36 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(6, new int[] { 13, 14, 21, 22, 35, 36 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(7, new int[] { 23, 25 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(8, new int[] { 25 }));
			
			exit = Position.world(linesShift + 3, columnsShift + 12);
		} else if (levelNum == 2){
//			             5         10        15        20        25
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
			// 0 0 1 1 1 0 0 0 0 0 0 0 1 0 0 0 0 0 1 0 0 0 0 0 0 0 0 1 1 1 0 0
			// 0 1 0 0 1 0 0 0 0 0 0 1 1 1 0 0 0 1 1 1 0 0 0 0 0 0 0 1 0 0 1 0
			// 0 0 0 0 1 0 0 0 0 0 1 1 0 1 0 0 0 1 0 1 1 0 0 0 0 0 0 1 0 0 0 0
			// 0 0 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 0
			// 0 0 0 0 1 0 0 1 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 1 0 0 0 1 0 0 0 0
			// 0 0 0 0 1 0 0 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 1 0 0 0 0
			// 0 0 0 1 0 0 0 1 1 0 0 0 0 0 0 0 0 0 0 0 0 0 1 1 0 0 0 0 1 0 0 0
			// 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0
			
			shiftsMap.Add(new KeyValuePair<int, int[]>(1, new int[] { 2, 3, 4, 12, 18, 27, 28, 29 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(2, new int[] { 1, 5, 11, 12, 13, 17, 18, 19, 27, 30 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(3, new int[] { 4, 10, 11, 13, 17, 19, 20, 27 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(4, new int[] { 4, 27 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(5, new int[] { 4, 7, 23, 27 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(6, new int[] { 4, 7, 8, 22, 23, 27 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(7, new int[] { 3, 7, 8, 22, 23, 28 }));
			
			linesShift = 48;
			columnsShift = 35;
			exit = Position.world(linesShift + 5, columnsShift + 13);
		} else {
//			        ............................**
//				    ............................**
//					............................*
//					.........................*..*
//					......................*....*
//					.......**.............**.***.*
//					.......*..***.*...***......**
//					......*........*..**
//					.....****.*.*....*
//					.....**........*.**
//					....*.........*****
//					.***.*...........***
//			13		*..*.............***
//					.***.*...........***
//					....*.........*****
//					.....**........*.**
//					.....****.*.*....*
//					......*........*..**
//					.......*..***.*...***......**
//					.......**.............**.***.*
//					......................*....*
//					.........................*..*
//					............................*
//					............................**
//					............................**

			linesShift = 50;
			columnsShift = 36;
						
			shiftsMap.Add(new KeyValuePair<int, int[]>(1, new int[] { 13 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(2, new int[] { 12, 14 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(3, new int[] { 12, 14 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(4, new int[] { 12, 14 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(5, new int[] { 12, 13, 14 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(6, new int[] { 11, 15 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(7, new int[] { 9, 10, 12, 14, 16, 17 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(8, new int[] { 6, 7, 9, 17, 19, 20 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(9, new int[] { 6, 9, 17, 20 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(10, new int[] { }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(11, new int[] { 7, 9, 17, 19 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(12, new int[] { 7, 19 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(13, new int[] { 7, 9, 17, 19 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(14, new int[] { }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(15, new int[] { 7, 11, 15, 19 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(16, new int[] { 8, 10, 11, 15, 16, 18 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(17, new int[] { 11, 15 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(18, new int[] { 9, 10, 11, 12, 13, 14, 15, 16, 17 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(19, new int[] { 7, 8, 10, 11, 12, 13, 14, 15, 16, 18, 19 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(20, new int[] { 7, 8, 12, 13, 14, 18, 19 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(21, new int[] { 7, 19 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(22, new int[] { }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(23, new int[] { 5, 6, 20, 21 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(24, new int[] { 6, 20 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(25, new int[] { }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(26, new int[] { 4, 6, 20, 22 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(27, new int[] { 6, 20 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(28, new int[] { 5, 6, 7, 19, 20, 21 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(29, new int[] { 1, 2, 4, 5, 7, 19, 22, 23, 24, 25 }));
			shiftsMap.Add(new KeyValuePair<int, int[]>(30, new int[] { 1, 2, 6, 20, 24, 25 }));
			
			exit = Position.world(48 + 5, 35 + 13);
		}

		int pointsCount = 0;
		foreach(int[] points in shiftsMap.Values) {
			pointsCount += points.Length;
		}

		level = new Position[pointsCount];
		int index = 0;
		int[] columns;
		foreach(int line in shiftsMap.Keys) {
			shiftsMap.TryGetValue(line, out columns);
			foreach (int column in columns) {
				level[index++] = Position.world(linesShift + line, columnsShift + column);
			}
		}
		
		
		_objectsMatrix = objects;
		
		_horizontalShift = (SIZE - objects.GetLength(0)) / 2 - 1;
		_verticalShift = (SIZE - objects.GetLength(1)) / 2 - 1;

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
			_activeGame [line, column] = true;
		}
		this._exit = Position.world(exit.getLine(), exit.getColumn());
	}

	// Translate the full grid the visible game objects
	void gameToGrid ()
	{
		int gridLines = _objectsMatrix.GetLength (0);
		int gridColumns = _objectsMatrix.GetLength (1);
				
		for (int i = 0; i < gridLines; ++i) {
			for (int j = 0; j < gridColumns; ++j) {
				GameObject o = _objectsMatrix [i, j];
				CellController cellC = o.GetComponent<CellController> ();
				if (_activeGame [_horizontalShift + i, _verticalShift + j]) {
					cellC.SetType (CellController.Type.ENEMY);
				} else if (_exit.getLine() == (i + _horizontalShift) && _exit.getColumn() == (j + _verticalShift)) {
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
		for (int i = 0; i < _lines; ++i) {
			for (int j = 0; j < _columns; ++j) {
				int an = aliveNeighb (i, j);
				_comingGame [i, j] = isAlive (_activeGame [i, j], an);
			}
		}
		swapGames ();
	}

	void swapGames ()
	{
		bool[,] tmp = _activeGame;
		_activeGame = _comingGame;
		_comingGame = tmp;
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
			return getInt(_activeGame [i - 1, j - 1]);
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
			return getInt(_activeGame [i - 1, j]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) (i + 1, j)  (i + 1, j + 1)
	// (i, j - 1)     (i,j)       (i, j +1 )
	// (i - 1, j - 1) (i - 1, j) *(i - 1, j + 1)
	int bottomRight (int i, int j)
	{
		if (i > 0 && j < _columns - 1) {
			return getInt(_activeGame [i - 1, j + 1]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) (i + 1, j)  (i + 1, j + 1)
	// (i, j - 1)     (i,j)      *(i, j +1 )
	// (i - 1, j - 1) (i - 1, j)  (i - 1, j + 1)
	int right (int i, int j)
	{
		if (j < _columns - 1) {
			return getInt(_activeGame [i, j + 1]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) (i + 1, j) *(i + 1, j + 1)
	// (i, j - 1)     (i,j)       (i, j +1 )
	// (i - 1, j - 1) (i - 1, j)  (i - 1, j + 1)
	int topRight (int i, int j)
	{
		if (i < _lines - 1 && j < _columns - 1) {
			return getInt(_activeGame [i + 1, j + 1]);
		} else {
			return 0;
		}
	}

	// (i + 1, j - 1) *(i + 1, j) (i + 1, j + 1)
	// (i, j - 1)      (i,j)      (i, j +1 )
	// (i - 1, j - 1)  (i - 1, j) (i - 1, j + 1)
	int top (int i, int j)
	{
		if (i < _lines - 1) {
			return getInt(_activeGame [i + 1, j]);
		} else {
			return 0;
		}
	}

	// *(i + 1, j - 1) (i + 1, j) (i + 1, j + 1)
	//  (i, j - 1)     (i,j)      (i, j +1 )
	//  (i - 1, j - 1) (i - 1, j) (i - 1, j + 1)
	int topLeft (int i, int j)
	{
		if (i < _lines - 1 && j > 0) {
			return getInt(_activeGame [i + 1, j - 1]);
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
			return getInt(_activeGame [i, j - 1]);
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
