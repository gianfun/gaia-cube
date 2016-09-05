using UnityEngine;
using System.Collections;

[System.Serializable]
public class WorldDimension{
	public int rowLen;
	public int height;
	public int numRowsPerHeight;
}

[System.Serializable]
public class WorldJson {
	public WorldDimension dimensions;
	public int[] start;
	public int[] goal;
}

public class WorldLoader {
	private WorldDimension dimens;
	private int[,,] clayState, goalState;

	public WorldLoader(int level){
		LoadLevel (level);
	}

	public void LoadLevel(int level){
		string json = System.IO.File.ReadAllText("Assets/Levels/level" + level + ".json");
		WorldJson worldjson = JsonUtility.FromJson<WorldJson> (json);
		dimens = worldjson.dimensions;

		clayState = new int[dimens.rowLen, dimens.height, dimens.numRowsPerHeight];
		goalState = new int[dimens.rowLen, dimens.height, dimens.numRowsPerHeight];

		for (int j = 0; j < dimens.height; j++) {
			for (int k = 0; k < dimens.numRowsPerHeight; k++) {
				for (int i = 0; i < dimens.rowLen; i++) {
					int heightOffset = j * dimens.rowLen * dimens.numRowsPerHeight;
					int rowOffset = k * dimens.rowLen;
					clayState [k, j, i] = worldjson.start [heightOffset + rowOffset + i];
					goalState [k, j, i] = worldjson.goal [heightOffset + rowOffset + i];
				}
			}
		}
	}


	public int[,,] getClayState (){
		return clayState;
	}

	public int[,,] getGoalState (){
		return goalState;
	}

	public Vector3 getDimensions (){
		return new Vector3(dimens.numRowsPerHeight, dimens.height, dimens.rowLen);
	}

}
