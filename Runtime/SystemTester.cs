using System.Collections;
using TMPro;
using UnityEngine;

public class SystemTester : MonoBehaviour
{
	[SerializeField] private GameObject projectile;

	[SerializeField] private TextMeshProUGUI projectilesActive; 
	[SerializeField] private TextMeshProUGUI projectilesInactive;
	[SerializeField] private TextMeshProUGUI projectilesTotal;

	[SerializeField] private TextMeshProUGUI vfxActive;
	[SerializeField] private TextMeshProUGUI vfxInactive;
	[SerializeField] private TextMeshProUGUI vfxTotal;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			SpawnProjectile(); 
		}

		projectilesActive.text = $"Active Projectiles: {ObjectPoolManager.Instance.ObjectPoolActiveSize(ObjectPoolManager.PoolType.Projectiles, projectile)}";
		projectilesInactive.text = $"Inactive Projectiles: {ObjectPoolManager.Instance.ObjectPoolInactiveSize(ObjectPoolManager.PoolType.Projectiles, projectile)}";
		projectilesTotal.text = $"Total Projectiles: {ObjectPoolManager.Instance.ObjectPoolTotalSize(ObjectPoolManager.PoolType.Projectiles, projectile)}";
	}

	public void SpawnProjectile()
	{
		GameObject obj = ObjectPoolManager.Instance.SpawnObject(projectile, transform.position, transform.rotation, ObjectPoolManager.PoolType.Projectiles);

		Vector3 randomDirection =
			Quaternion.AngleAxis(
				Random.Range(0f, 25),
				Random.onUnitSphere
			) * transform.forward;

		obj.GetComponent<Rigidbody>().linearVelocity = randomDirection * 10; 

		StartCoroutine(DelayedDespawnBall(obj)); 
	}

	private IEnumerator DelayedDespawnBall(GameObject obj)
	{
		yield return new WaitForSeconds(1f);

		ObjectPoolManager.Instance.ReturnObjectToPool(obj, ObjectPoolManager.PoolType.Projectiles); 
	}
}
