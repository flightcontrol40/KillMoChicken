using Chicken;
using DifficultyCalculator;
using Godot;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

public partial class Main : Node
{
	public bool PlacingTurret = false;
	private bool secondclick = false;
	public BaseTower newTower;

	private RoundManager.RoundManager roundm;
	private LevelManager levelm;
	private Shop shop;
	private Button StartRoundButton;
	private PauseMenu pauseMenu;
	private LevelSelector levelSelector;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    // Initialize LevelManager
    levelm = GetNode<LevelManager>("LevelManager");
    // Subscribe to level Selector signal
    levelSelector = GetNode<LevelSelector>("LevelSelector");
    levelSelector.GameStarted += () => {
        if (pauseMenu != null)
        {
            pauseMenu.EnablePauseMenu(); // Show pause menu when game actually starts
        }
    };
    // Initialize RoundManager
    roundm = GetNode<RoundManager.RoundManager>("RoundManager");
    // Subscribe to RoundManager Signals
    shop = GetNode<Shop>("Shop");
    pauseMenu = GetNode<PauseMenu>("PauseMenu");

    StartRoundButton = GetNode<Button>("Shop/Shop Panel/StartRoundButton");
    StartRoundButton.Pressed += () => {
        if (!PlacingTurret)
        {
            GD.Print("Round Starting");
            roundm.startRound();
        }
    };
        HealthBar health = GetNode<HealthBar>("PlayerHealth/HealthBar");
        levelm.level.Connect(Level.SignalName.HealthChanged, Callable.From<int>(health.OnHealthChanged));
        levelm.level.Connect(Level.SignalName.MoneyChanged, Callable.From<int>(shop.AddRemoveMoney));
    // Connect the GameLost signal
    roundm.GameWon += OnGameWon;
    roundm.GameLost += OnGameLost;



	// Initialize Start Round Button
	StartRoundButton = GetNode<Button>("Shop/Shop Panel/StartRoundButton");
	StartRoundButton.Pressed += () => {
		if (!PlacingTurret)
		{
			GD.Print("Round Starting");
			roundm.startRound();
		}
	};

	// Initialize LoseMenu and its buttons
	Control loseScreen = GetNode<Control>("LoseMenu");
	loseScreen.Visible = false;

	Button restartButton = loseScreen.GetNode<Button>("VBoxContainer/Button");
	restartButton.Pressed += ReloadLevel;

	Button exitButton = loseScreen.GetNode<Button>("VBoxContainer/Button3");
	exitButton.Pressed += () => {
		GD.Print("Exiting to main menu...");
		GetTree().ChangeSceneToFile("res://src/Austin/Scenes/main.tscn");
	};
}

	//Shows Loose Menu on Game Lost
	public void OnGameLost()
	{
		GD.Print("Player has lost. Showing lose screen.");
		Control loseScreen = GetNode<Control>("LoseMenu");
		loseScreen.Visible = true;

		// Hide pause menu when game is lost
		if (pauseMenu != null)
		{
			pauseMenu.DisablePauseMenu();
		}
	}

	//Reload the level when Restart is pressed
	private void ReloadLevel()
	{
		GD.Print("Restarting the game...");
		levelm.level.ResetLevel(); // Reset the level
		GetTree().ReloadCurrentScene(); // Reload the scene
	}

    private void OnGameWon(){
        GD.Print("Player has Won. Showing Win screen.");
        Control winScreen = GetNode<Control>("WinScreen");
        winScreen.Visible = true;
        // Hide pause menu when game is won
        if (pauseMenu != null)
        {
            pauseMenu.DisablePauseMenu();
        }
    }


	private void PlaceTower()
	{
		if (PlacingTurret)
		{
			if (newTower == null)
			{
				PlacingTurret = false;
			}
			else
			{
				newTower.GlobalPosition = GetViewport().GetMousePosition();
				if (Input.IsActionJustReleased("place_tower"))
				{
					if (secondclick == false)
					{
						secondclick = true;
					}
					else if (newTower.IsValidPosition())
					{
						secondclick = false;
						PlacingTurret = false;
						newTower.ToggleCollisionShapes();
						newTower = null;
					}
					else
					{
						GD.PushWarning("Invalid Placement");
					}
				}
				var PlacementColor = newTower.GetNode("Placement/ColorRect") as ColorRect;
				if (newTower.IsValidPosition())
				{
					PlacementColor.Color = new Color(0x3C3C3C6A);
				}
				else
				{
					PlacementColor.Color = new Color(0xFF3C3C9D);
				}
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		PlaceTower();
	}

	public void SetPaused(bool isPaused)
{
	GetTree().Paused = isPaused;
	// You might want to show/hide a pause overlay here
}
}
