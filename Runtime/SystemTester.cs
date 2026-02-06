using System.Collections;
using TMPro;
using UnityEngine;

public class SystemTester : MonoBehaviour
{
	[SerializeField] private GameObject projectile;
	[SerializeField] private GameObject vfx;

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

		vfxActive.text = $"Active VFX: {ObjectPoolManager.Instance.ObjectPoolActiveSize(ObjectPoolManager.PoolType.VFX, vfx)}";
		vfxInactive.text = $"Inactive VFX: {ObjectPoolManager.Instance.ObjectPoolInactiveSize(ObjectPoolManager.PoolType.VFX, vfx)}";
		vfxTotal.text = $"Total VFX: {ObjectPoolManager.Instance.ObjectPoolTotalSize(ObjectPoolManager.PoolType.VFX, vfx)}";
	}

	public void SpawnProjectile()
	{
		// Spawn projectile 
		GameObject projObj = ObjectPoolManager.Instance.SpawnObject(projectile, transform.position, transform.rotation, ObjectPoolManager.PoolType.Projectiles); // This basically replaces GameObject.Instantiate() 
		Vector3 randomDirection = Quaternion.AngleAxis(Random.Range(0f, 25), Random.onUnitSphere) * transform.forward;
		projObj.GetComponent<Rigidbody>().linearVelocity = randomDirection * 10; 
		StartCoroutine(DelayedDespawnObject(projObj, ObjectPoolManager.PoolType.Projectiles, 1f));

		// Spawn VFX 
		GameObject vfxObj = ObjectPoolManager.Instance.SpawnObject(vfx, transform.position, transform.rotation, ObjectPoolManager.PoolType.Projectiles);
		vfxObj.GetComponent<ParticleSystem>().Play();  
		StartCoroutine(DelayedDespawnObject(vfxObj, ObjectPoolManager.PoolType.VFX, 0.5f));
	}

	private IEnumerator DelayedDespawnObject(GameObject obj, ObjectPoolManager.PoolType poolType, float time)
	{
		yield return new WaitForSeconds(time); // Slight delay to show the objects before they're re-pooled 
		ObjectPoolManager.Instance.ReturnObjectToPool(obj, poolType); // This basically replaces Destroy(GameObject) 
	}
}
