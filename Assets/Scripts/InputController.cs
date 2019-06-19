using UnityEngine;

public class InputController : MonoBehaviour
{
	public new Transform camera;
	public LevelController segmentController;
	public CharacterController characterController;
      
    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Return))
		{
			var angles = camera.transform.eulerAngles;
			angles.z = 0f;
			camera.transform.eulerAngles = angles;
			segmentController.StartLevel();
		}

		if (characterController.IsDead || !segmentController.hasStarted)
			return;
		if (Input.GetKey(KeyCode.Q))
		{
			var angles = camera.transform.eulerAngles;
			angles.z += Time.deltaTime * 10f;
			camera.transform.eulerAngles = angles;
		}

		if (Input.GetKey(KeyCode.E))
		{
			var angles = camera.transform.eulerAngles;
			angles.z -= Time.deltaTime * 10f;
			camera.transform.eulerAngles = angles;
		}

		
	}
}
