using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ObstacleController : EntityController
{
	public override void Hit()
	{
		characterController.crashSound.Stop();
		characterController.crashSound.time = 0;
		characterController.crashSound.Play();
		characterController.ModifyCarLife(-10);
		characterController.carSpeed = 0f;
		Destroy(gameObject);
	}
}
