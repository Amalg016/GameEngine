# AJEngine

A 2D game engine built with C# using Silk.NET (OpenGL) with an ImGui-based editor.

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

## Project Structure

```
GameEngine/
├── src/
│   ├── GameEngine/           # Engine library (no editor dependency)
│   └── GameEngine.Editor/    # Editor (ImGui UI, level editor, inspector)
├── SampleGame/               # Example game using the engine library
└── Data/                     # Assets, shaders, textures, editor data
```

## Running the Editor

```bash
# Build
dotnet build src/GameEngine.Editor/GameEngine.Editor.csproj -c Debug -p:Platform=x64

# Run (must run from the output directory so assets are found)
cd src/GameEngine.Editor/bin/x64/Debug/net8.0
dotnet GameEngine.Editor.dll
```

## Building a Game

Create a new console project that references the engine library:

```bash
mkdir MyGame && cd MyGame
dotnet new console
dotnet add reference ../src/GameEngine/GameEngine.csproj
```

Then write your game entry point:

```csharp
using GameEngine.Core.Application;
using GameEngine.Core.Utilities;
using GameEngine.scenes;

// 1. Define your scene
public class MySceneInitializer : sceneInitializer
{
    public override void loadResources(Scene scene)
    {
        AssetPool.getShader("Assets/Shader/shader.vert", "Assets/Shader/shader.frag", "DefaultShader");
        // Load your textures, spritesheets, etc.
    }

    public override void Init(Scene scene)
    {
        // Create and add game objects to the scene
    }

    public override void imgui() { }

    public override void Exit(Scene scene)
    {
        foreach (var batch in scene.renderer.batches)
            batch.OnExit();
    }
}

// 2. Subclass the engine core
class MyGameCore : GameEngineCore
{
    private static MyGameCore _instance;
    public static new MyGameCore Instance => _instance ??= new MyGameCore();

    protected MyGameCore() : base()
    {
        SceneManager.SceneInitializerFactory = () => new MySceneInitializer();
    }

    protected override sceneInitializer CreateInitialScene() => new MySceneInitializer();
}

// 3. Run it
class Program
{
    static void Main(string[] args)
    {
        MyGameCore.Instance.Start();
    }
}
```

Build and run your game:

```bash
dotnet build -c Release -p:Platform=x64
cd bin/x64/Release/net8.0
dotnet MyGame.dll
```

> **Note:** Make sure the `Assets/` folder (shaders, textures) is available in the working directory where you run the game. Copy them from `Data/Assets/` or set up a `CopyAssets` target in your `.csproj` — see [SampleGame.csproj](SampleGame/SampleGame.csproj) for reference.

## Running the Sample Game

```bash
dotnet build SampleGame/SampleGame.csproj -c Release -p:Platform=x64
cd SampleGame/bin/x64/Release/net8.0
dotnet SampleGame.dll
```
