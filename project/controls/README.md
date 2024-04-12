# Runtime HUD & UI
![hud](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/a2a9ec02-d489-4134-b048-18ecbae0c6e5)

# Hotbar
![hot bar](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/a7cc0839-d4aa-4aca-89ec-04f9ffca3923)
- At runtime press number 1 - 6 to select a mode.
- Then press current selected mode number again to cycle through it's sub modes.

# Edit Modes

  1. Place Mode - Place a new instance in the scene
  2. Mine Mode - Not implemented yet
  3. Paint Mode - Change the mesh of a cell object
  4. Move Mode - Relocate or Resize a selected instance in the scene
  5. Select Mode - Select a single, multiple, or child of an instance
  6. Regenerate Mode - Regenerate the mesh of a selected instance


## Place Mode

-	Left click ray casts for an instance in the scene. If one is hit it will place a new instance into the scene.
-	Hold E 'action' to place instance at the controller’s current position
    - ( Holding E is only implemented for Tile & Bound modes right now )

### Place Bound Instance
![place bound](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/eaf15287-78b0-438c-8bbb-7e428d818529)

### Place Tile Instance
![place tile](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/d9abefc4-2e78-40ca-9f69-206c0ed90f58)

### Place Air Cell Instance
![place air](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/6b856492-8a8a-4d16-9986-fb3d83afc5bf)

## Mine Mode

- Not implemented yet

![mine mode](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/f6b3f237-381e-49ff-96ad-f03ff4b859b7)

## Paint Mode
- Paint mode will change the mesh material of cells in a selected Tile Instance.
- Left click ray casts for an instance in the scene. If it is a cell, it will change the mesh to the associated material.

### Paint Floor Mesh
![paint floor](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/ae17f1b0-7bb8-4d20-aa07-86bffdfbbf4f)

### Paint Wall Mesh
![paint wall](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/2f808f1d-1ce8-4ab2-a973-445341046c16)

### Paint Ceiling Mesh
![paint ceiling](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/f862265c-d6cc-4a10-a1aa-03d1972c169a)

## Move Mode
- Move selected instances in the scene while maintaining their alignment to the same grid.
    - Relocate:
        - Look at a selected instance and scroll the mouse wheel to translate it through the scene.
        - Hold E & left click to relocate the selected instance to the controller’s current position. 
    - Resize:
        - Look at selected the instance and scroll the mouse wheel. If a ray cast from the camera hits an instance’s collider it will translate the hit face.

### Move Relocate Mode
![relocate instance](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/fd658c26-55f3-4d03-9134-eb6aa253f745)

### Move Resize Mode
![resize instance](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/da10324c-3a13-4093-895e-f9f36f0318aa)

## Select Mode

- Try to select/deselect an instance with left click.
    - Single:
        - Selecting an instance will select the hit instance and deselect any current selected instances.
    - Multiple:
        - Selecting an instance will add the instance to a collection of selected instances.
    - Children:
        - Selecting instances is limited to the children of current selected instances.
            - This helps by disabling all other colliders making it easier to select cells or sockets of Tile Instances.

### Select Single
![select single](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/f998f67a-bb0b-4a1f-bbf8-efef842380e5)

### Select Multiple
![select multiple](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/7b8076c2-f32b-4d85-972a-29b8836c93c9)

### Select Children
![select children](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/e1a3135b-f398-45a7-9898-806ca632c7a9)

## Regenerate Mode
- While a Tile Instance is selected hold E & Left click to regenerate the surface tiles.
    - Bounded:
        - On input the manager will attempt to get the first selected bound instance & first selected tile instance. If a bound and tile instance are found the 
    - Unbounded:
        - On input the manager will attempt to get the first selected tile instance. If a tile instance is found it will be fully regenerated.

### Regenerate Bounded
![regen bounded](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/69b23dc1-f820-449c-b904-4eb78cdf0c16)

### Regenerate Unbounded
![regen unbounded](https://github.com/SomeGuyEight/CaveGenerationSystem/assets/137923841/09c3117a-79e2-4430-b303-2459d5f37b34)

