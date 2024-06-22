# Level Editor in Unity

This project is a Level Editor built with Unity, designed to allow users to create, edit, and save levels for games. The editor includes features such as placing objects, filling areas, random placement, and removing objects. Additionally, it supports setting patrol points and spawn points for enemies and players.

## Table of Contents

- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Adding Your Models](#adding-your-models)
- [Scripts Overview](#scripts-overview)
- [License](#license)

## Features

- **Create and Save Levels:** Users can create new levels, save them to disk, and load them later.
- **Object Placement:** Place objects in the level with a visual preview, including rotating objects before placing them.
- **Area Filling:** Fill areas with selected objects.
- **Random Placement:** Place random objects in the level.
- **Object Removal:** Remove objects from the level.
- **Patrol and Spawn Points:** Set patrol points for enemies and a spawn point for the player.

## Installation

1. **Clone the repository:**
    ```sh
    git clone https://github.com/alpenevil/level-editor.git
    ```
2. **Open the project in Unity:**
    - Open Unity Hub, click on "Add", and select the cloned project folder.

## Usage

1. **Start the Editor:**
    - Open the `LevelEditor` scene in Unity and enter Play mode.
2. **Create a New Level:**
    - Use the provided tools to place objects, fill areas, and set patrol/spawn points.
3. **Camera Controls:**
    - **Movement:** Use the arrow keys or A, D to move the camera left and right.
    - **Zoom:** Use W to zoom in and S to zoom out.
    - **Rotation:** Hold the right mouse button and drag to rotate the camera.
    - **Panning:** Hold the middle mouse button and drag to pan the camera.
4. **Save/Load Levels:**
    - Use the save/load buttons to manage your levels.
5. **Test Levels:**
    - Load saved levels in the test scene.

## Adding Your Models

To use your own models in the Level Editor, follow these steps:

1. **Prepare Your Prefabs:**
    - Ensure that your model is set up as a prefab.
    - Position the model so that it is located in the first quadrant of the XZ coordinate plane.

2. **Create an Objects Database:**
    - Navigate to the `Scripts` folder, locate the `ObjectsDatabaseSO` script.
    - Right-click on the script and select `Create > Objects Database SO`.

3. **Fill in the Database:**
    - Open the newly created Objects Database asset.
    - Fill in all the fields except for the `Icon` field.

4. **Generate Icons:**
    - Open the `IconGenerator` scene located in the `Scenes` folder.
    - In the hierarchy, find the `IconGenerator` GameObject.
    - Replace the `Database` reference with your own Objects Database.
    - Enter Play mode. The icons will be generated and automatically filled into the database.

5. **Update the Placement System:**
    - Open the `GridPlacementSystem` scene.
    - In the hierarchy, find the `PlacementSystem` GameObject.
    - Replace the `Database` reference with your own Objects Database.

6. **Update the Test Scene:**
    - Open the `TestScene`.
    - In the hierarchy, find the `PlacementSystem` and `TestSceneManager` GameObjects.
    - Replace their `Database` references with your own Objects Database.

Now you can use the system with your own models.

## Contributing

If you'd like to contribute to this project, please fork the repository and use a feature branch. Pull requests are warmly welcome.

## License

This project is released into the public domain under The Unlicense. For more details, see the [LICENSE](LICENSE) file.

## Credits

- **AurynSky:** Built-in models used in the project.
