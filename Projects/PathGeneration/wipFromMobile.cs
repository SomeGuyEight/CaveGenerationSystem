/* just some functions and thoughts when I did not have access to my computer */
internal void HandleDirectionChoice() {
	
	Vector3Int startPoint = Vector3Int.zero;
	Vector3Int endPoint = Vector3Int.zero;

	// (Key) int == (int)CubeDir & (Value)float == change in magnitude
	var distanceChanges = new List<KeyValuePair<<int,float>>();
	distanceChanges = GetDistanceChangeByDirection(startPoint, endpoint);

	// can square the value to exponentially increas an options choice, b/c the larger will get even larger etc
	/* choose the amount of directions to choose from
	 * > 3 guarantees step will not move backwards	 
	 * >> but if inline with two axis it will favor dir by enum index
	 *
	 * > 5 covers the caveat above, but may choose dir away from end point
	 * >> small chance choice will move away from end point
	 * 
	 * > solution 1: when choosing directions to weigh in
	 * >> when at the last index
	 * >> check if next index is the same #
	 * >>> if true add the direction to the pool, but halve both weights
	 * >>>> halve b/c if all same are added equally, it will dilute the pool
	 * >>> repeat until all = values included in pool
	 * >>> may need to check previous values to make sure they are not equal to the last... only happens if in line with 2 axis
	 * 
	 * > solution 2: as path gets closer to endPoint 
	 * >> force no moves that increase distance from end
	 * >> it will always weight the ~shortest route highest
	 * 
	 * > con: when inline with an axis
	 * >> it will stay within a 5 cell cross until it hits the end
	 *
	 * > 'fix': only have - moves fully eliminated within a threshhold
	 */
}

internal static int GetDiv(int dividend, int devisor) { 

}



/// <summary>
/// For fast exponential results when weighting pathing parameters
/// </summary>
/// <param name="base"></param>
/// <param name="exponent"></param>
/// <returns>if exponent > 0 => returns true exponential results; OR if exponent <= 0 => returns 0 to remove from weighted test;</returns>
internal static int GetExpWeight(this int base, int exponent) {
	int result = base;
	if (exponent <= 0) {
		return 0;
	} else if (int > 0) {
		for (int i = 0; i < exponent; i++) {
			result = result * base;
		}
	}
	return result;
}



internal static List<KeyValuePair<int,float>> GetDistanceChangeByDirection(this Vector3Int startPoint, Vector3Int endPoint) {
	
	var distanceChanges = new Dictionary<int, float>();

	float currentDistance = (endPoint- startPoint).magnitude;
	
	distanceChanges[(int)CubeDir.Right] = currentMagnitude - (endPoint- new Vector3Int(startPoint.x +1, startPoint.y, startPoint.z).magnitude;  
	distanceChanges[(int)CubeDir.Right] = currentMagnitude - (endPoint- new Vector3Int(startPoint.x -1, startPoint.y, startPoint.z).magnitude;  
	distanceChanges[(int)CubeDir.Up] = currentMagnitude - (endPoint- new Vector3Int(startPoint.x, startPoint.y+1, startPoint.z).magnitude;  
	distanceChanges[(int)CubeDir.Down] = currentMagnitude - (endPoint- new Vector3Int(startPoint.x, startPoint.y-1, startPoint.z).magnitude;  
	distanceChanges[(int)CubeDir.Forward] = currentMagnitude - (endPoint- new Vector3Int(startPoint.x, startPoint.y, startPoint.z+1).magnitude;  
	distanceChanges[(int)CubeDir.Back] = currentMagnitude - (endPoint- new Vector3Int(startPoint.x, startPoint.y, startPoint.z-1).magnitude;
	
	// shortest distances first, so those that move towards the target the least...
	// distanceChanges.OrderBy(x => x.Value)

	// descending will give the one that progresses towards the endpoint the most at the front
	List<KeyValuePair<int,float>> sortedKeyValuePairs = (
	from keyValuePair in dictionary
	orderby keyValuePair.Value descending
	select keyValuePair
	).ToList();

	return distanceChanges;
}



internal static int? ChooseDirection(List<KeyValuePair<int,float>> distanceChanges, int dirTestQuantity) {

	dirTestQuantity = Mathf.Max(distanceChanges.Count, dirTestQuantity);
	int totalTestCount = 0;
	for (int index = 0; index < dirTestQuantity; index++) {
		totalTestCount = totalTestCount + CeilToInt(distanceChanges.ElementAt(index).value);
	}
	
	int randomResult = UnityEngine.RandomRange(0, totalTestCount);
	
	for (int index = 1; index < dirTestQuantity; index++) {
		totalTestCount = CeilToInt(distanceChanges.ElementAt(index).value);
		if (totalTestCount < distanceChanges.ElementAt(index).value) {
			return index;
		}
	}
	return null;
}
