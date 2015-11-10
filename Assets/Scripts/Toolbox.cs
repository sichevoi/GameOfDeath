/**
 * Specific toolbox implementation to store global variables.
 */

public class Toolbox : Singleton<Toolbox> {

	protected Toolbox () {} // guarantee this will be always a singleton only - can't use the constructor!
	
	// Currently loaded level
	public int currentLevel = 0;
	
	public static int MAX_LEVEL = 19;
	
	void Awake () {
		// Your initialization code here
	}
}
