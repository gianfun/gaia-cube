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

public class WorldLoader : MonoBehaviour {

	public static int[,,] LoadLevel(int level){
		int[,,] world;
		string json = System.IO.File.ReadAllText("Assets/Levels/level" + level + ".json");
		WorldJson worldjson = JsonUtility.FromJson<WorldJson> (json);

		WorldDimension dimens = worldjson.dimensions;

		world = new int[dimens.rowLen, dimens.height, dimens.numRowsPerHeight];

		for (int j = 0; j < dimens.height; j++) {
			for (int k = 0; k < dimens.numRowsPerHeight; k++) {
				for (int i = 0; i < dimens.rowLen; i++) {
					int heightOffset = j * dimens.rowLen * dimens.numRowsPerHeight;
					int rowOffset = k * dimens.rowLen;
					world [k, j, i] = worldjson.start [heightOffset + rowOffset + i];
				}
			}
		}

		return world;
	}
}
