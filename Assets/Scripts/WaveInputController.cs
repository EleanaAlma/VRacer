using UnityEngine;
using System.Collections.Generic;
using wvr;

public class WaveInputController : MonoBehaviour
{
	public LevelController levelController;
	public Transform waveRoot;

	/// <summary>
	/// Executes every frame
	/// </summary>
	void Update ()
	{
		//Check if button on right controller is pressed
		var isTriggered = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Menu);		
		if (isTriggered)
		{
			//Get controller's transform from plugin
			var trans = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).transform;
			//Calculate controller's forward vector in world coordinates
			var dir = (trans.TransformPoint(new Vector3(0f, 0f, 1f)) - trans.pos).normalized;
			//Create a ray starting from the controller's position, pointing to its forward vector
			var ray = new Ray(trans.pos, dir);
			RaycastHit hit;
			//Check if anything is hit by ray
			if(Physics.Raycast(ray, out hit, 100f))
			{
				//Check if what is being hit has 'START' tag
				if(hit.transform.tag == "START")
				{
					//Reset HMD's roaming position
					ResetWorldPivot();
					//Start game
					levelController.StartLevel();
				}
			}			
		}		
	}

	/// <summary>
	/// Resets displacement of user from VR coordinage system
	/// due to 6 degrees of freedom
	/// </summary>
	private void ResetWorldPivot()
	{		
		var children = new List<Transform>();
		//Find VR camera
		var head = waveRoot.Find("head");
		//Get all children of VR coordinate system
		foreach(Transform child in waveRoot)
		{
			//Store child to array buffer
			children.Add(child);
			//Unparent child from coordinate system
			child.parent = null;
		}
		//Snap coordinate system to head's position
		waveRoot.transform.position = head.transform.position;
		//Reattach all children to VR coordinate system
		foreach(var child in children)
		{
			child.transform.parent = waveRoot;
		}
	}
}
