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
		
		levelLabel.text = "Level " + levelNum;
	
		IDictionary<int, int[]> shiftsMap = new Dictionary<int, int[]>();
		int linesShift;
		int columnsShift;
		
		linesShift = 47;
		columnsShift = 47;
		
		exit = Position.world(53, 50);
		
		if (levelNum == 0) {
			linesShift = 46;
			columnsShift = 43;
			
			exit = Position.world(linesShift + 7, columnsShift + 6);
			
		} else if (levelNum == 1) {
			linesShift = 44;
			columnsShift = 43;

			exit = Position.world(linesShift + 5, columnsShift + 12);
		} else if (levelNum == 2){
			linesShift = 48;
			columnsShift = 35;
			
			exit = Position.world(linesShift + 5, columnsShift + 13);
		} else if (levelNum == 3) {
			linesShift = 42;
			columnsShift = 41;
									
			exit = Position.world(53, 50);
		} else if (levelNum == 4) {

			linesShift = 47;
			columnsShift = 47;
			
			exit = Position.world(53, 50);
		} else if (levelNum == 9) {
			exit = Position.world(53, 50);
		} else if (levelNum == 12) {
			linesShift = 43;
			columnsShift = 42;
		
			exit = Position.world(53, 46);
		} else if (levelNum == 13) {
			linesShift = 43;
			columnsShift = 48;
			
			exit = Position.world(53, 50);
		} else if (levelNum == 14) {
			linesShift = 41;
			columnsShift = 46;
			
			exit = Position.world(50, 55);
		} else if (levelNum == 16) {
			linesShift = 44;
			columnsShift = 46;
			
			exit = Position.world(53, 55);
		} else if (levelNum == 17) {
			linesShift = 43;
			columnsShift = 40;
			
			exit = Position.world(53, 55);
		} else if (levelNum == 19) {
			linesShift = 43;
			columnsShift = 40;
			
			exit = Position.world(53, 55);
		} else {
			linesShift = 46;
			columnsShift = 44;
			
			exit = Position.world(53, 48);
		}
		
		TextAsset textFile = Resources.Load("Level" + levelNum) as TextAsset;
		string text = textFile.text;
		
		string[] lines = text.Split('\n');
		for(int l = 0; l < lines.Length; ++l) {
			string line = lines[l];
			List<int> items = new List<int>();
			for (int i = 0; i < line.Length; ++i) {
				if (line[i].Equals('O') || line[i].Equals('*')) {
					items.Add(i);
				}
			}
			shiftsMap.Add(new KeyValuePair<int, int[]>(l, items.ToArray()));
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
				
				bool isExit = false;
				
				if (_exit.getLine() == (i + _horizontalShift) && _exit.getColumn() == (j + _verticalShift)) {
					_gridController.DrawExit(i, j);
					isExit = true;
				}
				
				if (_activeGame [_horizontalShift + i, _verticalShift + j]) {
					cellC.SetType (CellController.Type.ENEMY);
				} else if (isExit) {
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
