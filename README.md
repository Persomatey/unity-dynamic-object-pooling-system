# Unity Package CI/CD System Template 
A simple free template CI/CD system for Unity packages using GitHub Actions. 

This system is designed to stay close to Unity's reccommended UPM workflow, avoid docker and editor automation complexity, be understandable and debuggable for your package in particular. 

Check out [Releases](https://github.com/Persomatey/unity-package-ci-cd-system-template/releases) tab to see a history of all versions of this package. 

Happy packaging! 

## Installation 
### Install via Package Manager

<details>
<summary>Select `Install package from git URL...` in the Package Manager</summary>
	<img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/git-url-installation-example.png">
</details>

For latest release:</br>
`https://github.com/Persomatey/unity-package-ci-cd-system-template.git#upm`</br>
Or install specific releases:</br>
`https://github.com/Persomatey/unity-package-ci-cd-system-template.git#v#.#.#`

### Download the tarball directly from release
<details>
<summary>Go to Release and download directly from Assets</summary>
	  <img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/release-tab-tarball-circled.png">
</details>

`com.yourcompany.packagename-v#.#.#.tgz`

`com.yourcompany.packagename-v#.#.#.zip`

<details>
<summary>Select `Install package from tarball...` in the Package Manager</summary>
	  <img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/tarball-installation-example.png">
</details>

## Features 
- CI-safe Git operations
	- Git identity configured for GitHub Actions
	- Orphan branch creation handled cleanly
	- Fully non-interactive, repeatable pipeline
- Clean UPM-compliant package builds
	- Produces a sanitized package layout suitable for Unity Package Manager
	- Excludes project-only files (Assets, ProjectSettings, .github, etc.)
	- Ensures only package-safe contents are published
- GitHub Releases integration
	- Automatically publishes versioned releases
	- Release artifacts include ready-to-install package archives and git versioned UPM installation 
	- <details><summary>Release notes display changes included in Release; including commit SHA, subject, and description </summary><img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/release-tab-cl2-circled.png"></details>
- Automated [semantic versioning](https://semver.org/) (MAJOR.MINOR.PATCH)
	- Every push increments the PATCH number, with MAJOR and MINOR being incremented maually. 
- Package samples 
	- Automatically converts Samples/ → Samples~/ for UPM compliance
	- Preserves sample scenes and assets
	- <details><summary>Enables the “Import” button in the "Samples" tab in the Package Manager</summary><img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/sample-import-example.png"></details>
- Dedicated upm branch publishing
	- Uses a clean orphan upm branch containing only package contents
	- Force-pushes releases to ensure the branch stays pristine
	- Prevents accidental inclusion of build artifacts
- UPM tarball & zip artifact generation
	- Generates .tgz and .zip archives from the clean UPM build
	- Archives are attached to Release
- Package Manager metadata support (package.json) 
	- <details><summary>Documentation link wired to README.md</summary><img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/package-doc-circled.png"></details>
	- <details><summary>Changelog link wired to GitHub Releases</summary><img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/package-cl-circled.png"></details>
	- <details><summary>Licenses link wired to GitHub License</summary><img src="https://raw.githubusercontent.com/Persomatey/unity-package-ci-cd-system-template/refs/heads/main/images/package-lic-circled.png"></details>

## Workflows 

### Package (`package.yml`)
Every time a push is made to the GitHub repository, this workflow will push the contents of the package folder to a `upm` package branch which Unity's Package Manager can refernce directly from. It will also create tarball files to install the package directly. This will also increment the PATCH version number. A Release Tag will be generated and the builds generated will be included in your repo page's "Releases" tab. 

### Versioning (`version-bump.yml`)
Used to manually version bump the version number. Should be in the format `X.Y.Z`. All future pushes will subsequently start incrementing based on the new MAJOR or MINOR version changes. 
	  - Ex: If the last version before triggering this workflow is `v0.0.42`, and the workflow was triggered with `v0.1.0`, the next `build.yml` workflow run will create the version tag `v0.1.1`. 

## Set up  
1. Fork/clone this repository (rename it to match your package or project)
2. Create initial version tag
	1. Navigate to your GitHub version tags page
      `github.com/username_or_org/repo_name/releases/new`
	2. Click "Tag: Select Tag"
	3. Set tag to v0.0.0
	4. Click "Create"
	5. Set "Release title"
	6. Click "Publish release"
3. Rename your Unity project name to whatever you want
	- `unity_project/` -> `MyAmazingUnityPackage/` 
4. Rename your Unity package folder
	- `unity_project/Packages/com.yourcompany.packagename/` -> `unity_project/Packages/com.amazeproductions.thebestpackageever/`
5. Edit package JSON contents in `unity_project/Packages/com.yourcompany.packagename/package.json` 
	- Change the following fields:
		- `name` -> the same as your package folder in Step 4
		- `displayName` -> the way you want your package named in the Package Manager
		- `description` -> describe what your package does
		- `documentationUrl` -> URL of your documentation 
			- I just have it pointed to the repo's README but if you have external docs, you can use that
		- `changelogUrl` -> URL of your changelog
			- I just have it pointed to the repo's Releases tab since the changelog is edited automatically there but you can put whatever you want
		- `license` -> whatever license type you use
		- `licenseUrl` -> URL pointing to the repo's LICENSE file 
		- `unity` -> `6000.0` for Unity 6, `6000.1` for Unity 6.1, `2022.3` for Unity 2022.3, etc.
		- `author` -> your name or company / url
		- `dependencies` -> populate with other package dependencies / respective package versions 
		- `samples` -> fill out if the default values are not to your liking
			- only necessary if you're using the Samples tab in the Package Manager, otherwise you can remove this field entirely 
6. Rename editor AsmDef file at `unity_project/Packages/com.yourcompany.packagename/Editor/com.yourcompany.packagename.Editor.asmdef`
	- `unity_project/Packages/com.yourcompany.packagename/Editor/com.yourcompany.packagename.Editor.asmdef` -> `unity_project/Packages/com.yourcompany.packagename/Editor/com.myamazingcompany.myamazingpackage.Editor.asmdef`
7. Edit editor AsmDef contents in `unity_project/Packages/com.yourcompany.packagename/Editor/com.yourcompany.packagename.Editor.asmdef`
	- Change the following fields:
		- `name` -> the same as your package folder in Step 4 appended with `.Editor`
		- `rootNamespace` -> the same as your package folder in Step 4 (except note the namespace's PascalCasing)
		- `references` -> include one that is the same as your package folder in Step 4 (except note the namespace's PascalCasing) appended with `.Runtime` (just in case you need it in the future)
			- include any others your scripts need, otherwise you can just leave the Runtime reference 
		- any other if the default values are not to your liking
8. Rename runtime AsmDef file at `unity_project/Packages/com.yourcompany.packagename/Runtime/com.yourcompany.packagename.Runtime.asmdef`
	- `unity_project/Packages/com.yourcompany.packagename/Runtime/com.yourcompany.packagename.Runtime.asmdef` -> `unity_project/Packages/com.yourcompany.packagename/Runtime/com.myamazingcompany.myamazingpackage.Runtime.asmdef`
9. Edit runtime AsmDef contents in `unity_project/Packages/com.yourcompany.packagename/Runtime/com.yourcompany.packagename.Runtime.asmdef`
	- Change the following fields:
		- `name` -> the same as your package folder in Step 4 appended with `.Runtime`
		- `rootNamespace` -> the same as your package folder in Step 4 (except note the namespace's PascalCasing) 
		- `references` -> include any your scripts need, otherwise you can leave this empty 
		- any other if the default values are not to your liking
10. Edit/add the namespace in your script(s)
	- note the namespace's PascalCasing
	- <details><summary>Example</summary>
      
		```
		using UnityEngine;
		
		namespace YourCompany.PackageName // EDIT THIS NAMESPACE HERE 
		{
			public class MyAmazingScript : MonoBehaviour
			{
				// Some code 
			}
		}
		```
		</details>
11. In `.github/workflows/package.yml`, in the `env`, set the following variables:
    - `PROJECT_PATH` (line 12) with the unity project name from Step 3
	- `PACKAGE_NAME` (line 13) with the unity package name from Step 4
12. (optional) but **HIGHLY** suggested)) Use Jeff Adulco's Unity GUID Regenerator package
    - If you create multiple packages using this template, this is a **MUST** as it allows users to use all of your packages without conflicting with each other 
    - Doing this at this early stage before you start developing will ensure you don't break any GUID references while you're building because, while Unity GUID Regenerator is good, it isn't perfect and can lead to broken references which are annoying. 
    1. Install using `https://github.com/jeffjadulco/unity-guid-regenerator.git`
    2. Highlight all files/folders in your package folder, right click, and regenerate

## Future Plans 
*No plans on when I'd release these features, would likely depend on my needs for a specific project/boredom/random interest in moving this project along.*
- [OpenUPM](https://openupm.com/) inteegration 
	- Ideally as an optional setting within the yaml 
	- This is necessary to have an automatic fully populated Version History tab in the Package Manager 
- Automated multi-version branches 
	- Having the `version-bump.yml` worflow generate new branches for each version MAJOR and MINOR version with their own CI 
		- Ex: Concurrently supported `v0.1.#` and `v0.2.#` and `v1.0.#` versions 
	- Necessary if you want multiple versions of a package suported in case there are compatability differences or feature additions across unique versions of Unity 
