using UnityEngine;
using System.Collections;

public interface IFlyingObjects{

	//Rüzgardan etkilenebilecek cisimleri belirten interface

	Vector3 GetWorldPosition();

	void applyWind (Vector3 wind);

	
}
