# Unity Package CI/CD System Template 
A simple object pooling package for Unity. A singleton manager that efficiently spawns and reuses GameObjects using Unity’s built-in ObjectPool system. Includes a sample scene demonstrating projectiles and VFX pooling.

This is to effectively replace `Instantiate()` and `Destroy()` functions for objects you create multiple of in your project with `ObjectPoolManager.Instance.SpawnObject()` and `ObjectPoolManager.Instance.ReturnObjectToPool()`. 

Check out [Releases](https://github.com/Persomatey/unity-package-ci-cd-system-template/releases) tab to see a history of all versions of this package. 

## Installation 
### Install via Package Manager

<details>
<summary>Select `Install package from git URL...` in the Package Manager</summary>
	<img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/git-url-installation-example.png">
</details>

For latest release:</br>
`https://github.com/Persomatey/unity-dynamic-object-pooling-system.git#upm
`</br>
Or install specific releases:</br>
`https://github.com/Persomatey/unity-dynamic-object-pooling-system.git#v#.#.#`

### Download the tarball directly from release
<details>
<summary>Go to Release and download directly from Assets</summary>
	  <img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/release-tab-tarball-circled.png">
</details>

`com.huntergoodin.objectpool-v#.#.#.tgz`

`com.huntergoodin.objectpool-v#.#.#.zip`

<details>
<summary>Select `Install package from tarball...` in the Package Manager</summary>
	  <img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/tarball-installation-example.png">
</details>

## To add pool types (optional) 
<i>This is really just to keep everything nice and tidy in your scene heirarchy, making debugging easier. You can ignore this feature entirely and sort everything in one big pool if you want, a single pool is an `Object` dictionary that's agnostic as to what types of prefabs/components are pooled. So you can have ProjectileA and ProjectileB in the same pool, but if you request a ProjectileA, the system will return a pooled ProjectileA only (and vice versa).</i>
1. Add your pool type to the `PoolType` enum 
2. Create a new Empty GameObject
   - Under `// Object pool stuff`, create a new static `GameObject` for your pool type
3. Create the Empty `GameObject` in `SetUpEmpties()`
	- Ex: 
	```
	projectilesEmpty = new GameObject("Projectiles");
	projectilesEmpty.transform.parent = emptyHolder.transform; 
	```
 4. Ensure that pooled objects of a certain type are parented correctly in `SetParentObject()` 
    - Ex: 
	```
	case PoolType.Projectiles:
		return projectilesEmpty; 
	```
 5. Set pool type in count getters' `targetPrefab` (`GetObjectPoolTotalSize()`, `GetObjectPoolActiveSize()`, `GetObjectPoolInactiveSize()`)
	- Ex: 
	```
	PoolType.Projectiles => prefab,
	```

## Features 
- A singleton-based Object Pool Manager that dynamically creates and manages pools per prefab at runtime
    - Pools are automatically created the first time a prefab is spawned
    - Uses Unity’s built-in ObjectPool<T> for efficient allocation and reuse
- Supports multiple categorized pool types:
    - Automatically organizes pooled objects under labeled parent GameObjects in the hierarchy
    - Included (as examples):
    	- Projectiles
     	- VFX
		- AudioSource
    - You can create as many as you want though (or ignore this feature entirely and sort everything in one big category if you want). 
- Generic SpawnObject<T>() system:
    - Spawn via world position or parent Transform
    - Supports both GameObject and Component return types
    - Automatically resets Rigidbody velocity and clears TrailRenderer when reused
- ReturnObjectToPool() method:
    - Safely returns objects to their originating pool
    - Prevents returning non-pooled objects with error handling
- Runtime pool tracking utilities:
    - Get total pool size
    - Get active object count
    - Get inactive object count
- Includes ready-to-use/drag-and-drop ObjectPoolManager prefab for quick setup
- (Optional) DontDestroyOnLoad support for persistence across scenes
- (Optional) `OnGetObject()` in you need additional logic when you get an object (like resetting it or something) 

## Future Plans 
<i>No plans on when I'd release these features, would likely depend on my needs for a specific project/boredom/random interest in moving this project along.</i>
- Make some of the functions DRYer 
	- Not that it really matters since the system itself shouldn't need to be touched much, but it'd be nice for my own sanity
- Contemplate if `OnGetObject()` is actually necessary or useful in any way
	- Every project I've worked on has very different repooling needs, and even sometimes object-to-object within the same project, and I usually just use a reset function in the pooled object itself anyways. 
