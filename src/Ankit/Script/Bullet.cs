using Godot;
using System;
using Chicken;

public partial class Bullet : Area2D
{
    // Speed of the bullet, with a default value of 400.
    [Export]
    public float Speed = 1000;
    
    // Damage inflicted by the bullet, with a default value of 10.
    [Export]
    public int Damage { get; set; } = 50;
    
    // Direction vector determining the bullet's movement direction.
    public Vector2 Direction;
    
    // Add these two new properties for tower range checking
    public Vector2 TowerPosition;
    public float TowerRange = 192f;
    
    // Notifier to detect when the bullet exits the screen.
    private VisibleOnScreenNotifier2D screenNotifier;
    
    private int FramesBeforeDeletion = 500;
    
    private bool _RemoveThis = false;

    // Called when the bullet node is added to the scene.
    public override void _Ready()
    {
        base._Ready();
        AddToGroup("Projectile"); // Adds the bullet to the "Projectile" group for easy management.
        
        // Connects to the "area_entered" signal to handle collisions with other areas.
        Connect("area_entered", new Callable(this, nameof(OnAreaEntered)));
        
        // Gets the screen notifier node to detect when the bullet leaves the screen.
        screenNotifier = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreenNotifier2D");
        if (screenNotifier != null)
        {
            // Connects to the "screen_exited" signal to remove the bullet when off-screen.
            screenNotifier.Connect("screen_exited", new Callable(this, nameof(OnScreenExited)));
        }
        else
        {
            GD.PrintErr("VisibleOnScreenNotifier2D not found in Bullet scene");
        }
        
        // Sets the bullet's rotation to match its movement direction.
        if (Direction != Vector2.Zero)
        {
            Rotation = Direction.Angle();
        }
        
        // Configures the bullet's collision layers and masks.
        SetupCollisions();
    }
    
    // Sets the collision layers and masks for the bullet.
    private void SetupCollisions()
    {
        CollisionLayer = 4; // Layer the bullet is on.
        CollisionMask = 2;  // Layers the bullet will collide with.
    }
    
    // Updates the bullet's position every frame based on its speed and direction.
    public override void _Process(double delta)
    {
        if (this._RemoveThis == true){
            this.QueueFree();
        }
        // Add range check before the screen boundary check
        float distanceFromTower = GlobalPosition.DistanceTo(TowerPosition);
        if (distanceFromTower > TowerRange)
        {
            this.CallDeferred("queue_free");
            return;
        }
        
        // Keep existing screen boundary check as a fallback
        if (!GetViewport().GetVisibleRect().HasPoint(GlobalPosition))
            this.CallDeferred("queue_free");
        
        Position += Direction * (float)(Speed * delta);
    }
    
    // Handles collision with other areas, specifically chickens, and deals damage.
    private void OnAreaEntered(Area2D area)
    {
        Node parent = area.GetParent();
        
        if (parent is BaseChicken chicken)
        {
            // Inflicts damage on the chicken and removes the bullet.
            chicken.TakeDamage(Damage);
            this._RemoveThis = true;
        }
    }
    
    // Removes the bullet when it exits the screen.
    private void OnScreenExited()
    {
        this.CallDeferred("queue_free");
    }
}