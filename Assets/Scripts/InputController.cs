using UnityEngine;

/// <summary>
/// Class repsonsible for reading input from keyboard/mous
/// </summary>
public class InputController : MonoBehaviour
{
	public new Transform camera;
	public LevelController levelController;
	public CharacterController characterController;
      
	/// <summary>
	/// Executed every frame
	/// </summary>
    void Update()
    {
		//Check every frame if Return key was pressed
		if (Input.GetKeyDown(KeyCode.Return))
		{
			//Reset camera's rotation 
			var angles = camera.transform.eulerAngles;
			angles.z = 0f;
			camera.transform.eulerAngles = angles;
			//Start level
			levelController.StartLevel();
		}

		if (characterController.IsDead || !levelController.hasStarted)
			return;
		//Check every frame if Q was pressed
		if (Input.GetKey(KeyCode.Q))
		{
			//Add 10 degrees / second to the camera's z rotation axis
			var angles = camera.transform.eulerAngles;
			angles.z += Time.deltaTime * 10f;
			camera.transform.eulerAngles = angles;
		}
		//Check every frame if E was pressed
		if (Input.GetKey(KeyCode.E))
		{
			//Subtract 10 degrees / second to the camera's z rotation axis
			var angles = camera.transform.eulerAngles;
			angles.z -= Time.deltaTime * 10f;
			camera.transform.eulerAngles = angles;
		}		
	}
}
