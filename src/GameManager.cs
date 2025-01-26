using Godot;

public class GameManager : Node
{
    [Export]
    private PackedScene _credits;

    [Export]
    private PackedScene _mainMenu;

    [Export]
    private PackedScene _play;

    [Export]
    private PackedScene _end;

    private Node _currentScene;

    public static GameManager Instance { get; private set; }
    public override void _Ready()
    {
        Instance = this;
    }

    public override void _Process(float delta)
    {
        if (_currentScene == null)
        {
            ChangeScene("MainMenu");
        }
        if( Input.IsActionJustPressed("quit"))
        {
            if (_currentScene.Name == "MainMenu")
            {
                GetTree().Quit();
            }
            else
            {
                ChangeScene("MainMenu");
            }
        }
        else if (Input.IsActionJustPressed("accept") && _currentScene.Name == "MainMenu")
        {
            ChangeScene("Play");
        }
    }

    public void ChangeScene(string sceneName)
    {
        if (_currentScene != null)
            _currentScene.QueueFree();
        switch(sceneName)
        {
            case "Credits":
                _currentScene = _credits.Instance();
                break;
            case "End":
                _currentScene = _end.Instance();
                break;
            case "Play":
                _currentScene = _play.Instance();
                break;
            default:
                _currentScene = _mainMenu.Instance();
                break;
        }
        AddChild(_currentScene);
    }
}
