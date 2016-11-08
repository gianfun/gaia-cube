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

	public WorldLoader(){
		//LoadLevel (level);
	}
	public IEnumerator LoadLevel(int level){
		string json;
		
		Debug.Log ("Load Level " + level);
		string path = Application.streamingAssetsPath + "/Levels/level" + level + ".json";
		Debug.Log ("Loading " + path);

		#if UNITY_EDITOR
			json = System.IO.File.ReadAllText(path);
			yield return null;
		#else
			WWW www = new WWW ( path);
			yield return www;
			Debug.Log ("Loaded " + path);
			Debug.Log ("Size: " + www.size);

			json = www.text;
		#endif
		WorldJson worldjson = JsonUtility.FromJson<WorldJson> (json);
		dimens = worldjson.dimensions;

		clayState = new int[dimens.rowLen, dimens.height, dimens.numRowsPerHeight];
		goalState = new int[dimens.rowLen, dimens.height, dimens.numRowsPerHeight];

		int x, z;
		for (int j = 0; j < dimens.height; j++) {
			for (int k = 0; k < dimens.numRowsPerHeight; k++) {
				for (int i = 0; i < dimens.rowLen; i++) {
					x = (dimens.rowLen - 1 - i); // = i; //To rotate/mirror json array.
					z = (dimens.numRowsPerHeight - 1 - k);// = k; //To rotate/mirror json array.
					int heightOffset = j * dimens.rowLen * dimens.numRowsPerHeight;
					int rowOffset = k * dimens.rowLen;

					clayState [x, j, z] = worldjson.start [heightOffset + rowOffset + x];
					goalState [x, j, z] = worldjson.goal  [heightOffset + rowOffset + x];
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
		return new Vector3(dimens.rowLen, dimens.height, dimens.numRowsPerHeight);
	}

}
