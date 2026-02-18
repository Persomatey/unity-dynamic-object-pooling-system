using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace HunterGoodin.DynamicObjectPooler
{
	public class ObjectPoolManager : MonoBehaviour
	{
		public enum PoolType
		{
			Projectiles,
			VFX,
			AudioSource
		}

		// Singleton stuff 
		public static ObjectPoolManager Instance => instance;
		private static ObjectPoolManager instance;

		[SerializeField] private bool addToDontDestroyOnLoad = false;

		// Object pool stuff 
		private GameObject emptyHolder;
		private static GameObject projectilesEmpty;
		private static GameObject vfxEmpty;
		private static GameObject audiosourceEmpty;
		private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools;
		private static Dictionary<GameObject, GameObject> cloneToPrefabMap;
		private static Dictionary<GameObject, PoolType> prefabToPoolType;

		private void Awake()
		{
			// Singleton Stuff 
			if (instance != null)
			{
				Destroy(gameObject);
			}
			else
			{
				instance = this;
			}

			if (addToDontDestroyOnLoad)
			{
				DontDestroyOnLoad(gameObject);
			}

			// Object pool stuff 
			objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
			cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
			prefabToPoolType = new Dictionary<GameObject, PoolType>();

			SetUpEmpties();
		}

		private void SetUpEmpties()
		{
			emptyHolder = gameObject;

			projectilesEmpty = new GameObject("Projectiles");
			projectilesEmpty.transform.parent = emptyHolder.transform;

			vfxEmpty = new GameObject("VFX");
			vfxEmpty.transform.parent = emptyHolder.transform;

			audiosourceEmpty = new GameObject("AudioSources");
			audiosourceEmpty.transform.parent = emptyHolder.transform;
		}

		private void CreatePool(GameObject prefab, Vector3 pos, Quaternion rot, PoolType pPoolType)
		{
			Debug.Log($"Creating pool of {pPoolType}");

			ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
				createFunc: () => CreateObject(prefab, pos, rot, pPoolType),
				actionOnGet: OnGetObject,
				actionOnRelease: OnReleaseObject,
				actionOnDestroy: OnDestroyObject
			);

			objectPools.Add(prefab, pool);
			prefabToPoolType[prefab] = pPoolType;
		}

		private void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType pPoolType)
		{
			Debug.Log($"Creating pool of {pPoolType}");

			ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
				createFunc: () => CreateObject(prefab, parent, rot, pPoolType),
				actionOnGet: OnGetObject,
				actionOnRelease: OnReleaseObject,
				actionOnDestroy: OnDestroyObject
			);

			objectPools.Add(prefab, pool);
			prefabToPoolType[prefab] = pPoolType;
		}

		private GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType pPoolType)
		{
			prefab.SetActive(false);

			GameObject obj = Instantiate(prefab, pos, rot);

			prefab.SetActive(true);

			GameObject parentObj = GetParentObject(pPoolType);
			obj.transform.SetParent(parentObj.transform);

			return obj;
		}

		private GameObject CreateObject(GameObject prefab, Transform parent, Quaternion rot, PoolType pPoolType)
		{
			prefab.SetActive(false);

			GameObject obj = Instantiate(prefab, parent);
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = rot;
			obj.transform.localScale = Vector3.one;

			prefab.SetActive(true);

			return obj;
		}

		private void OnGetObject(GameObject obj)
		{
			// Optional Logic for when we just need to get the object itself for some reasion 
		}

		private void OnReleaseObject(GameObject obj)
		{
			obj.SetActive(false);
		}

		private void OnDestroyObject(GameObject obj)
		{
			if (cloneToPrefabMap.ContainsKey(obj))
			{
				cloneToPrefabMap.Remove(obj);
			}
		}

		private GameObject GetParentObject(PoolType pPoolType)
		{
			switch (pPoolType)
			{
				case PoolType.Projectiles:
					return projectilesEmpty;
				case PoolType.VFX:
					return vfxEmpty;
				case PoolType.AudioSource:
					return audiosourceEmpty;
				default:
					Debug.LogError($"ERROR: Invalid PoolType passed ({pPoolType})");
					return null;
			}
		}

		private T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion spawnRot, PoolType pPoolType) where T : Object
		{
			// Check if pool already exists, if it doesn't, create it now 
			if (!objectPools.ContainsKey(objectToSpawn))
			{
				CreatePool(objectToSpawn, parent, spawnRot, pPoolType);
			}

			// Gets an object from the pool, otherwise instantiate one for us 
			GameObject obj = objectPools[objectToSpawn].Get();

			// Add it to the cloning dictionary and return it 
			if (obj != null)
			{
				if (!cloneToPrefabMap.ContainsKey(obj))
				{
					cloneToPrefabMap.Add(obj, objectToSpawn);
				}

				obj.transform.SetParent(parent);
				obj.transform.localPosition = Vector3.zero;
				obj.transform.localRotation = spawnRot;

				if (obj.GetComponent<Rigidbody>())
				{
					obj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
					obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				}

				if (obj.GetComponent<TrailRenderer>())
				{
					obj.GetComponent<TrailRenderer>().Clear();
				}

				obj.SetActive(true);

				// In case T isn't a GameObject, if we ever want that for some reason (maybe for pooling AudioSources like I was thinking for the constant SFX system I wanted to make)
				if (typeof(T) == typeof(GameObject))
				{
					return obj as T;
				}

				T component = obj.GetComponent<T>();

				if (component == null)
				{
					Debug.LogError($"ERROR: Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
					return null;
				}

				return component;
			}
			else
			{
				Debug.LogError($"ERROR: No valid obj was found or created somehow ({obj})");
				return null;
			}
		}

		private T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType pPoolType) where T : Object
		{
			// Check if pool already exists, if it doesn't, create it now 
			if (!objectPools.ContainsKey(objectToSpawn))
			{
				CreatePool(objectToSpawn, spawnPos, spawnRot, pPoolType);
			}

			// Gets an object from the pool, otherwise instantiate one for us 
			GameObject obj = objectPools[objectToSpawn].Get();

			// Add it to the cloning dictionary and return it 
			if (obj != null)
			{
				if (!cloneToPrefabMap.ContainsKey(obj))
				{
					cloneToPrefabMap.Add(obj, objectToSpawn);
				}

				obj.transform.position = spawnPos;
				obj.transform.rotation = spawnRot;

				if (obj.GetComponent<Rigidbody>())
				{
					obj.GetComponent<Rigidbody>().linearVelocity = Vector3.zero;
					obj.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
				}

				if (obj.GetComponent<TrailRenderer>())
				{
					obj.GetComponent<TrailRenderer>().Clear();
				}

				obj.SetActive(true);

				// In case T isn't a GameObject, if we ever want that for some reason (maybe for pooling AudioSources like I was thinking for the constant SFX system I wanted to make)
				if (typeof(T) == typeof(GameObject))
				{
					return obj as T;
				}

				T component = obj.GetComponent<T>();

				if (component == null)
				{
					Debug.LogError($"ERROR: Object {objectToSpawn.name} doesn't have component of type {typeof(T)}");
					return null;
				}

				return component;
			}
			else
			{
				Debug.LogError($"ERROR: No valid obj was found or created somehow ({obj})");
				return null;
			}
		}

		// Public accessor functions 

		public T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRot, PoolType pPoolType) where T : Component
		{
			// Since our other one is so generic, we can just use that but return a Component type explicitly 
			return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot, pPoolType);
		}

		public GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType pPoolType)
		{
			// Likewise but return a GameObject type explicitly 
			return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRot, pPoolType);
		}

		public T SpawnObject<T>(T typePrefab, Transform parent, Quaternion spawnRot, PoolType pPoolType) where T : Component
		{
			// Since our other one is so generic, we can just use that but return a Component type explicitly 
			return SpawnObject<T>(typePrefab.gameObject, parent, spawnRot, pPoolType);
		}

		public GameObject SpawnObject(GameObject objectToSpawn, Transform parent, Quaternion spawnRot, PoolType pPoolType)
		{
			// Likewise but return a GameObject type explicitly 
			return SpawnObject<GameObject>(objectToSpawn, parent, spawnRot, pPoolType);
		}

		public void ReturnObjectToPool(GameObject obj, PoolType pPoolType)
		{
			if (cloneToPrefabMap.TryGetValue(obj, out GameObject prefab))
			{
				GameObject parentObject = GetParentObject(pPoolType);

				// If it's not parented for whatever reason, reparent it so it goes back where it's supposed to 
				if (obj.transform.parent != parentObject.transform)
				{
					obj.transform.parent = parentObject.transform;
				}

				// Find the object from the pool's Dictionary and release it 
				if (objectPools.TryGetValue(prefab, out ObjectPool<GameObject> pool))
				{
					pool.Release(obj);
				}
			}
			else
			{
				if (obj)
				{
					Debug.LogError($"ERROR: Trying to return an object that is not pooled ({obj.name})");
				}
			}
		}

		// Public count getters 

		public int GetObjectPoolTotalSize(PoolType poolType, GameObject prefab)
		{
			if (objectPools == null || objectPools.Count == 0)
			{
				return 0;
			}

			GameObject targetPrefab = poolType switch
			{
				PoolType.Projectiles => prefab,
				PoolType.VFX => prefab,
				PoolType.AudioSource => prefab,
				_ => null
			};

			if (targetPrefab == null || !objectPools.TryGetValue(targetPrefab, out var pool))
			{
				return 0;
			}

			return pool.CountAll;
		}

		public int GetObjectPoolActiveSize(PoolType poolType, GameObject prefab)
		{
			if (objectPools == null || objectPools.Count == 0)
			{
				return 0;
			}

			GameObject targetPrefab = poolType switch
			{
				PoolType.Projectiles => prefab,
				PoolType.VFX => prefab,
				PoolType.AudioSource => prefab,
				_ => null
			};

			if (targetPrefab == null || !objectPools.TryGetValue(targetPrefab, out var pool))
			{
				return 0;
			}

			return pool.CountActive;
		}

		public int GetObjectPoolInactiveSize(PoolType poolType, GameObject prefab)
		{
			if (objectPools == null || objectPools.Count == 0)
			{
				return 0;
			}

			GameObject targetPrefab = poolType switch
			{
				PoolType.Projectiles => prefab,
				PoolType.VFX => prefab,
				PoolType.AudioSource => prefab,
				_ => null
			};

			if (targetPrefab == null || !objectPools.TryGetValue(targetPrefab, out var pool))
			{
				return 0;
			}

			return pool.CountInactive;
		}

		// Clear functions 

		public void ClearPool(GameObject prefab)
		{
			if (objectPools == null || !objectPools.ContainsKey(prefab))
			{
				Debug.LogWarning($"No pool exists for prefab {prefab.name}");
				return;
			}
			ObjectPool<GameObject> pool = objectPools[prefab];

			// Destroy the GameObjects 
			List<GameObject> toDestroy = new List<GameObject>();

			foreach (var kvp in cloneToPrefabMap)
			{
				if (kvp.Value == prefab)
				{
					toDestroy.Add(kvp.Key);
				}
			}

			foreach (var obj in toDestroy)
			{
				cloneToPrefabMap.Remove(obj);

				if (obj != null)
				{ 
					Destroy(obj);
				}
			}

			// Clear the pool itself
			pool.Clear();
			objectPools.Remove(prefab);

			Debug.Log($"Cleared pool for prefab {prefab.name}");
		}

		public void ClearPoolsOfType(PoolType poolType)
		{
			if (objectPools == null || objectPools.Count == 0)
			{
				Debug.LogWarning("No pools to clear.");
				return;
			}

			List<GameObject> prefabsToClear = new List<GameObject>();

			foreach (var kvp in prefabToPoolType)
			{
				if (kvp.Value == poolType)
				{
					prefabsToClear.Add(kvp.Key);
				}
			}

			foreach (GameObject prefab in prefabsToClear)
			{
				ClearPool(prefab);
			}

			Debug.Log($"Cleared all pools of type {poolType}");
		}


		public void ClearAllPools()
		{
			if (objectPools == null || objectPools.Count == 0)
			{
				Debug.LogWarning("No pools to clear.");
				return;
			}

			// Make a list of all prefabs so we can iterate safely while modifying the dictionary
			List<GameObject> allPrefabs = new List<GameObject>(objectPools.Keys);

			foreach (GameObject prefab in allPrefabs)
			{
				ClearPool(prefab);
			}

			Debug.Log("Cleared all object pools.");
		}
	}
}