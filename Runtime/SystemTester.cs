using UnityEngine;

public class SystemTester : MonoBehaviour
{
	[SerializeField] private GameObject projectile; 

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			GameObject obj = ObjectPoolManager.Instance.SpawnObject(projectile, transform.position, transform.rotation, ObjectPoolManager.PoolType.Projectiles);
			obj.GetComponent<Rigidbody>().linearVelocity = Vector3.forward * 5f; 
		}
	}
}
