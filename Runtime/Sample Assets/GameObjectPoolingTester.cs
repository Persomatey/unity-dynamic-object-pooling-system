using System.Collections;
using TMPro;
using UnityEngine;

namespace HunterGoodin.DynamicObjectPooler
{
	public class GameObjectPoolingTester : MonoBehaviour
	{
		[SerializeField] private GameObject projectileA;
		[SerializeField] private GameObject projectileB;
		[SerializeField] private GameObject vfx;

		[SerializeField] private TextMeshProUGUI projectileAsActive;
		[SerializeField] private TextMeshProUGUI projectileAsInactive;
		[SerializeField] private TextMeshProUGUI projectileAsTotal;

		[SerializeField] private TextMeshProUGUI projectileBsActive;
		[SerializeField] private TextMeshProUGUI projectileBsInactive;
		[SerializeField] private TextMeshProUGUI projectileBsTotal;

		[SerializeField] private TextMeshProUGUI vfxActive;
		[SerializeField] private TextMeshProUGUI vfxInactive;
		[SerializeField] private TextMeshProUGUI vfxTotal;

		private void Update()
		{
			projectileAsActive.text = $"Active ProjectileAs: {ObjectPoolManager.Instance.GetObjectPoolActiveSize(ObjectPoolManager.PoolType.Projectiles, projectileA)}";
			projectileAsInactive.text = $"Inactive ProjectileAs: {ObjectPoolManager.Instance.GetObjectPoolInactiveSize(ObjectPoolManager.PoolType.Projectiles, projectileA)}";
			projectileAsTotal.text = $"Total ProjectileAs: {ObjectPoolManager.Instance.GetObjectPoolTotalSize(ObjectPoolManager.PoolType.Projectiles, projectileA)}";

			projectileBsActive.text = $"Active ProjectileBs: {ObjectPoolManager.Instance.GetObjectPoolActiveSize(ObjectPoolManager.PoolType.Projectiles, projectileB)}";
			projectileBsInactive.text = $"Inactive ProjectileBs: {ObjectPoolManager.Instance.GetObjectPoolInactiveSize(ObjectPoolManager.PoolType.Projectiles, projectileB)}";
			projectileBsTotal.text = $"Total ProjectileBs: {ObjectPoolManager.Instance.GetObjectPoolTotalSize(ObjectPoolManager.PoolType.Projectiles, projectileB)}";

			vfxActive.text = $"Active VFX: {ObjectPoolManager.Instance.GetObjectPoolActiveSize(ObjectPoolManager.PoolType.VFX, vfx)}";
			vfxInactive.text = $"Inactive VFX: {ObjectPoolManager.Instance.GetObjectPoolInactiveSize(ObjectPoolManager.PoolType.VFX, vfx)}";
			vfxTotal.text = $"Total VFX: {ObjectPoolManager.Instance.GetObjectPoolTotalSize(ObjectPoolManager.PoolType.VFX, vfx)}";
		}

		public void SpawnProjectileA()
		{
			// Spawn projectile 
			GameObject projObj = ObjectPoolManager.Instance.SpawnObject(projectileA, transform.position, transform.rotation, ObjectPoolManager.PoolType.Projectiles); // This basically replaces Instantiate() 
			Vector3 randomDirection = Quaternion.AngleAxis(Random.Range(0f, 25), Random.onUnitSphere) * transform.forward;
			projObj.GetComponent<Rigidbody>().linearVelocity = randomDirection * 10;
			StartCoroutine(DelayedDespawnObject(projObj, ObjectPoolManager.PoolType.Projectiles, 1f));

			// Spawn VFX 
			GameObject vfxObj = ObjectPoolManager.Instance.SpawnObject(vfx, transform.position, transform.rotation, ObjectPoolManager.PoolType.VFX);
			vfxObj.GetComponent<ParticleSystem>().Play();
			StartCoroutine(DelayedDespawnObject(vfxObj, ObjectPoolManager.PoolType.VFX, 0.5f));
		}

		public void SpawnProjectileB()
		{
			// Spawn projectile 
			GameObject projObj = ObjectPoolManager.Instance.SpawnObject(projectileB, transform.position, transform.rotation, ObjectPoolManager.PoolType.Projectiles); // This basically replaces Instantiate() 
			Vector3 randomDirection = Quaternion.AngleAxis(Random.Range(0f, 25), Random.onUnitSphere) * transform.forward;
			projObj.GetComponent<Rigidbody>().linearVelocity = randomDirection * 10;
			StartCoroutine(DelayedDespawnObject(projObj, ObjectPoolManager.PoolType.Projectiles, 1f));

			// Spawn VFX 
			GameObject vfxObj = ObjectPoolManager.Instance.SpawnObject(vfx, transform.position, transform.rotation, ObjectPoolManager.PoolType.VFX);
			vfxObj.GetComponent<ParticleSystem>().Play();
			StartCoroutine(DelayedDespawnObject(vfxObj, ObjectPoolManager.PoolType.VFX, 0.5f));
		}

		private IEnumerator DelayedDespawnObject(GameObject obj, ObjectPoolManager.PoolType poolType, float time)
		{
			yield return new WaitForSeconds(time); // Slight delay to show the objects before they're re-pooled 
			ObjectPoolManager.Instance.ReturnObjectToPool(obj, poolType); // This basically replaces Destroy(GameObject) 
		}

		public void ClearProjectileAPools()
		{
			Debug.Log("ClearProjectileAPools"); 
			ObjectPoolManager.Instance.ClearPool(projectileA);
		}

		public void ClearProjectileBPools()
		{
			ObjectPoolManager.Instance.ClearPool(projectileB);
		}

		public void ClearProjectilePools()
		{
			ObjectPoolManager.Instance.ClearPoolsOfType(ObjectPoolManager.PoolType.Projectiles); 
		}

		public void ClearVFXPools()
		{
			ObjectPoolManager.Instance.ClearPoolsOfType(ObjectPoolManager.PoolType.VFX);
		}

		public void ClearAllPools()
		{
			ObjectPoolManager.Instance.ClearAllPools(); 
		}
	}
}