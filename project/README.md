# Getting the project set up

## Required

* If you have access to the Template_SGExampleProject.0.1.0.zip file just unzip the file and launch it as a new project in unity. You can skip to # 3 below.
* If you don't have the file you will just need to create a new project from scratch.

1. Start a new untiy 3D URP project Editor Version 2023.1.18f1

2. Download the UnityRegistry Assets used by the SGExampleProject

    - UnityInputSystem
    - UnityUI
    - Mathematics
    - Cinemachine
    - Probuilder
    - TextMeshPro

3. Download the required Paid assets
   - [Tessera Pro - Asset Store Page](https://assetstore.unity.com/packages/tools/level-design/tessera-pro-161077) by [Boris the Brave](https://assetstore.unity.com/publishers/44953) - [Boris the Brave's website](https://www.boristhebrave.com)
   - [Odin Inspector & Serializer - Asset Store Page](https://assetstore.unity.com/packages/tools/utilities/odin-inspector-and-serializer-89041) by [Sirenix](https://assetstore.unity.com/publishers/3727)

4. Download & import the [SGProjectFiles.0.1.0](project/unity-packages/SGProjectFiles.0.1.0.unitypackage) Unity package


## Optional if you would like to use the generation statistics options

5. Make the three changes inside Tessera outlined in [TesseraModifications](project/scripts/TesseraModifications.cs)

6. Update the scripts inside Assets/_/Scripts
    - Either uncomment out the lines outlined in [TesseraModifications](project/scripts/TesseraModifications.cs)

or

    - Download & import the [SGScripts_ModifiedSGScriptsAfterTesseraModifications.0.1.0](project/unity-packages/SGScripts_ModifiedSGScriptsAfterTesseraModifications.0.1.0.unitypackage) Unity package & let it override the scripts in Assets/_/Scripts
       - [GenerationManager](project/scripts/GenerationSystem/GenerationManager.cs)
       - [project/scripts/GenerationSystem/InstanceGenerator.cs](InstanceGenerator)
       - [FullGenerationStats](project/scripts/Statistics/FullGenerationStats.cs)
       - [StatsCSVWriter](project/scripts/Statistics/StatsCSVWriter.cs)
