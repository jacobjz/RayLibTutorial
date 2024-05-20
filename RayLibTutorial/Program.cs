using Raylib_cs;
using System.Numerics;

namespace HelloWorld;

class Program
{
    const int fps = 144;
    const int width = 1600;
    const int height = 800;
    public static void Main()
    {
        Raylib.InitWindow(width, height, "Solar system");

        Camera2D camera = new() { Zoom = 1f, Offset = new Vector2 { X = width/2, Y = height/2 } };

        //Image img = Raylib.LoadImage("assets/mily-way.png");
        //Texture2D texture = Raylib.LoadTextureFromImage(img);
        //texture.Width = width;
        //texture.Height = height;

        InputInterpreter inputInterpreter = new();
        Raylib.SetTargetFPS(fps);

        var game = GameInit();

        while (!Raylib.WindowShouldClose())
        {            
            inputInterpreter.InterpretKeys(game);
            camera = inputInterpreter.InterpretMouse(camera);

            //World
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginMode2D(camera);

            //Objects
            game.DrawObjects(camera);

            Raylib.EndMode2D();

            //UI
            game.DrawUi(camera);

            Raylib.EndDrawing();
        }

        Raylib.CloseWindow();
    }

    private static Game GameInit()
    {
        var gameObjects = new List<GameObject>();
        var currentState = new GameState { GameObjects = gameObjects, Options = new GameOptions { ShowUi = true } };
        Game game = new() { CurrentState = currentState };


        var moon = new Moon { Radius = 2, Color = Color.Gray, Name = "Moon" };
        var earth = new Planet { Radius = 25, Color = Color.Blue, Name = "Earth" };
        
        var sun = new Sun 
        { 
            Name = "Sun",
            X = 0,
            Y = 0,
            Radius = 100,
            Color = Color.Orange,
        };

        sun.AddSatellite(earth, 240);
        earth.AddSatellite(moon, 40);

        gameObjects.Add(sun);
        return game;
    }
}
public class Game 
{
    public GameState CurrentState { get; set; }

    public void DrawObjects(Camera2D camera)
    {
        foreach (var gameObject in CurrentState.GameObjects)
        {
            gameObject.Update();
            gameObject.Draw(camera);
        }
    }

    public void DrawUi(Camera2D camera)
    {
        var mouse = Raylib.GetMousePosition();
        Vector2 sunScreenToWorld = Raylib.GetScreenToWorld2D(mouse, camera);

        if (CurrentState.Options.ShowUi)
        {
            Raylib.DrawText("Time: " + Raylib.GetTime().ToString("0.00"), 0, 0, 16, Color.White);
            Raylib.DrawText("Fps: " + Raylib.GetFPS().ToString(), 0, 16, 16, Color.White);
            Raylib.DrawText($"X:{sunScreenToWorld.X:.} Y: {sunScreenToWorld.Y:.}", 0, 32, 16, Color.White);
        }

    }
}


public class GameState
{
    public List<GameObject> GameObjects = new List<GameObject>();
    public GameOptions Options { get; set; }
}

public class GameOptions
{
    public bool ShowUi { get; set; }
}

public class Moon : GameObject
{
    public int Mass { get; set; }
}

public class Planet : GameObject
{
    public Planet() { }
    public Planet(int x, int y, int radius)
    {
        X = x;
        DX = x;
        Y = y;
        DY = y;
        Radius = radius;
    }

    double DX { get; set; }
    double DY { get; set; }
    public int Mass { get; set; }
    public double Theta { get; set; }
    public override void Update()
    {
        X += 1;
        Y += 1;
    }
}

public class Sun : GameObject
{
    public int Mass { get; set; }
}

public abstract class GameObject : IGameObject
{
    public string Name { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Radius { get; set; }

    public Color Color { get; set; }

    public List<GameObject> Children { get; set; } = new List<GameObject>();

    public GameObject Parent { get; set; }

    public void Draw(Camera2D camera)
    {
        Raylib.DrawCircle(X, Y, Radius, Color);
        foreach (var child in Children)
        {
            child.Update();
            child.Draw(camera);
        }
    }
    public virtual void AddSatellite(GameObject gameObject)
    {
        gameObject.Parent = this;
        Children.Add(gameObject);
    }

    public virtual void AddSatellite(GameObject gameObject, int distance) 
    {
        gameObject.X = X + distance;
        gameObject.Parent = this;
        Children.Add(gameObject);
    }
    
    public virtual void Update(){ }
}

public interface IGameObject
{
    void Update();
    void Draw(Camera2D camera);
    void AddSatellite(GameObject gameObject);
    void AddSatellite(GameObject gameObject, int distance);
}