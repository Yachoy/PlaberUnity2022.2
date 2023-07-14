using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingComponent : MonoBehaviour {

	[SerializeField] public string modelName = "name";

    public void GetPreview()
	{
		Building.LoadPreview(modelName);
	}

}
