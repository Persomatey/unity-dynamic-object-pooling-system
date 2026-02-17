using System.Collections;
using TMPro;
using UnityEngine;

namespace HunterGoodin.DynamicObjectPooler
{
	public class ComponentPoolingTester : MonoBehaviour
	{
		[SerializeField] private GameObject audioSourcePrefab;

		[SerializeField] private TextMeshProUGUI audioSourcesActive;
		[SerializeField] private TextMeshProUGUI audioSourcesInactive;
		[SerializeField] private TextMeshProUGUI audioSourcesTotal;

		private void Update()
		{
			audioSourcesActive.text = $"Active VFX: {ObjectPoolManager.Instance.GetObjectPoolActiveSize(ObjectPoolManager.PoolType.AudioSource, audioSourcePrefab)}";
			audioSourcesInactive.text = $"Inactive VFX: {ObjectPoolManager.Instance.GetObjectPoolInactiveSize(ObjectPoolManager.PoolType.AudioSource, audioSourcePrefab)}";
			audioSourcesTotal.text = $"Total VFX: {ObjectPoolManager.Instance.GetObjectPoolTotalSize(ObjectPoolManager.PoolType.AudioSource, audioSourcePrefab)}";
		}

		public void SpawnComponent()
		{
			AudioSource source = ObjectPoolManager.Instance.SpawnObject<AudioSource>(audioSourcePrefab.GetComponent<AudioSource>(), Vector3.zero, Quaternion.identity, ObjectPoolManager.PoolType.AudioSource);
			StartCoroutine(DelayedDespawnComponent(source)); 
		}

		private IEnumerator DelayedDespawnComponent(Component comp)
		{
			yield return new WaitForSeconds(1f);
			ObjectPoolManager.Instance.ReturnObjectToPool(comp.gameObject, ObjectPoolManager.PoolType.AudioSource);
		}
	}
}
