using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Pool; 

public class ObjectPoolManager : MonoBehaviour
{
	public enum PoolType
	{
		Projectiles,
		VFX,
	}

	// Singleton stuff 
	public static ObjectPoolManager Instance => Instance;
	private static ObjectPoolManager instance; 

	[SerializeField] private bool addToDontDestroyOnLoad = false;

	// Object pool stuff 
	private GameObject emptyHolder;
	private static GameObject projectilesEmpty;
	private static GameObject vfxEmpty;
	private static Dictionary<GameObject, ObjectPool<GameObject>> objectPools; 
	private static Dictionary<GameObject, GameObject> cloneToPrefabMap; 

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
			DontDestroyOnLoad(gameObject);
		}

		// Object pool stuff 
		objectPools = new Dictionary<GameObject, ObjectPool<GameObject>>();
		cloneToPrefabMap = new Dictionary<GameObject, GameObject>();
		SetUpEmpties(); 
	}

	private void SetUpEmpties()
	{
		emptyHolder = gameObject; 

		projectilesEmpty = new GameObject("Projectiles");
		projectilesEmpty.transform.parent = emptyHolder.transform; 

		vfxEmpty = new GameObject("VFX");
		vfxEmpty.transform.parent = emptyHolder.transform;

		if (addToDontDestroyOnLoad)
		{
			DontDestroyOnLoad(vfxEmpty.transform.root); 
		}
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
	}

	private void CreatePool(GameObject prefab, Transform parent, Quaternion rot, PoolType pPoolType)
	{
		ObjectPool<GameObject> pool = new ObjectPool<GameObject>(
			createFunc: () => CreateObject(prefab, parent, rot, pPoolType),
			actionOnGet: OnGetObject,
			actionOnRelease: OnReleaseObject,
			actionOnDestroy: OnDestroyObject
		);

		objectPools.Add(prefab, pool);
	}

	private GameObject CreateObject(GameObject prefab, Vector3 pos, Quaternion rot, PoolType pPoolType)
	{
		prefab.SetActive(false);

		GameObject obj = Instantiate(prefab, pos, rot);

		prefab.SetActive(true);

		GameObject parentObj = SetParentObject(pPoolType);
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

	private GameObject SetParentObject(PoolType pPoolType)
	{
		switch (pPoolType)
		{
			case PoolType.Projectiles:
				return projectilesEmpty; 
			case PoolType.VFX:
				return vfxEmpty; 
			default:
				Debug.LogError($"ERROR: Invalid PoolType passed ({pPoolType})");
				return null; 
		}
	}

	private T SpawnObject<T>(GameObject objectToSpawn, Transform parent, Quaternion spawnRot, PoolType pPoolType)where T : Object
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

	private T SpawnObject<T>(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType pPoolType)where T : Object
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

	public T SpawnObject<T>(T typePrefab, Vector3 spawnPos, Quaternion spawnRot, PoolType pPoolType)where T : Component
	{
		// Since our other one is so generic, we can just use that but return a Component type explicitly 
		return SpawnObject<T>(typePrefab.gameObject, spawnPos, spawnRot, pPoolType);
	}

	public GameObject SpawnObject(GameObject objectToSpawn, Vector3 spawnPos, Quaternion spawnRot, PoolType pPoolType)
	{
		// Likewise but return a GameObject type explicitly 
		return SpawnObject<GameObject>(objectToSpawn, spawnPos, spawnRot, pPoolType);
	}

	public T SpawnObject<T>(T typePrefab, Transform parent, Quaternion spawnRot, PoolType pPoolType)where T : Component
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
			GameObject parentObject = SetParentObject(pPoolType); 

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
			Debug.LogError($"ERROR: Trying to return an object that is not pooled ({obj.name})"); 
		}
	}
}
