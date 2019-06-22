using UnityEngine;
using System.Collections.Generic;
using wvr;

public class WaveInputController : MonoBehaviour
{
	public LevelController levelController;
	public Transform waveRoot;

	void Update ()
	{
		var isTriggered = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Menu);		
		if (isTriggered)
		{
			var trans = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).transform;
			var dir = (trans.TransformPoint(new Vector3(0f, 0f, 1f)) - trans.pos).normalized;
			var ray = new Ray(trans.pos, dir);
			RaycastHit hit;
			if(Physics.Raycast(ray, out hit, 100f))
			{
				if(hit.transform.tag == "START")
				{
					ResetWorldPivot();
					levelController.StartLevel();
				}
			}			
		}		
	}

	private void ResetWorldPivot()
	{
		var children = new List<Transform>();
		var head = waveRoot.Find("head");
		foreach(Transform child in waveRoot)
		{
			children.Add(child);
			child.parent = null;
		}
		waveRoot.transform.position = head.transform.position;
		foreach(var child in children)
		{
			child.transform.parent = waveRoot;
		}
	}
}
