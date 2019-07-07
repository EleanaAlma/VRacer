using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObstacleController : EntityController
{
	/// <summary>
	/// Damage player when called
	/// </summary>
	public override void Hit()
	{
		//Play sound effect of crash on player
		characterController.crashSound.Stop();
		characterController.crashSound.time = 0;
		characterController.crashSound.Play();
		//Damage player
		characterController.ModifyCarLife(-10);
		//Halt player's movement
		characterController.carSpeed = 0f;
		//Remove from scene
		Destroy(gameObject);
	}
}
