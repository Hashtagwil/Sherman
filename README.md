# Introduction 
Sherman is a multiplayer game. 4 player or AI must kill each other to win. You can choose your tank and its color. You can play with a keyboard mouse or a controller.

# Getting Started
1.	Installation process
clone project with git
open project with unity hub https://docs.unity3d.com/2019.1/Documentation/Manual/GettingStartedOpeningProjects.html

2.	Software dependencies
unity version: 2021.3.8f1
visual studio
git

# VS integration
Unity must be configured to be used with VS.  This is done by choosing VS in the following window: preferences-> external tools -> External Script Editor.
More details can be found here: 
https://docs.microsoft.com/en-us/visualstudio/gamedev/unity/get-started/getting-started-with-visual-studio-tools-for-unity?pivots=windows#configure-unity-to-use-visual-studio

You must also install the Unity package in VS.  This is done through Tools -> Get Tools and Features... and choosing the "Game development with Unity" 
package in the gaming section.

# Build and Test
In Unity
File -> Build Settings
Add your scenes, build and run

# Contribute
Scene files are very easy to break what is done by another developer, so it is essential to create a test scene for yourself, test what needs to be tested, make a prefab and commit only the prefab for exemple.

Do not hesitate to export package to make a backup during merge and pull request.

add .gitconfig 

```
[merge]
tool = unityyamlmerge

[mergetool "unityyamlmerge"]
trustExitCode = false
cmd = 'C:\Program Files\Unity\Editor\Data\Tools\UnityYAMLMerge.exe' merge -p "$BASE" "$REMOTE" "$LOCAL" "$MERGED"
```

# Good Practice
When creating new object in a scene, make sure to give them significative name. If we have two object in the same scene with the same name when doing a merge via git, it won't be able to resolve conflict.

# Making an installer

You need to first build the new version via Unity.
To make the installer, we're using [Inno Setup](https://jrsoftware.org/isinfo.php)
The .iss file is already done, you just need to open it and change the variable. 
It should mostly be the version number and the ```#define MyAppBuildFolder```
You can then run InnoSetup and it will generate and installer. 
You can build the installer via Build => Compile
You can find the installer file via Build => Open Output Folder

# CI/CD

To make the CI/CD work, we'd need to setup a nwe AgentPool with a new agent running on someones computer to be able to have a custom environment to build the project properly
[More Info](https://medium.com/medialesson/continuous-integration-for-unity-3d-projects-using-azure-pipelines-e61ddf64ad79)