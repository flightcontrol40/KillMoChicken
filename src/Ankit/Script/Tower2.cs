// File: src/Ankit/Scripts/Tower2.cs
using Godot;


public partial class Tower2 : BaseTower 
{
    protected override void InitializeComponents()
    {
        base.InitializeComponents();
        SetupSpawnPoints(2);  // Two spawn points for Tower2
    }
    protected override void InitializeTowerProperties()
    {
        // Rapid fire tower configuration
        ShootingInterval = 0.3f;    // Faster shooting
        RotationSpeed = 7.0f;       // Faster rotation
        BulletsPerShot = 2;         // Double shots
        BulletSpeed = 350f;         // Faster bullets
        BulletDamage = 8;           // Less damage per bullet

        GD.Print("=== Rapid Fire Tower Properties Initialized ===");
        GD.Print($"Type: Rapid Fire Tower");
        GD.Print($"Shooting Interval: {ShootingInterval}");
        GD.Print($"Rotation Speed: {RotationSpeed}");
        GD.Print($"Bullets Per Shot: {BulletsPerShot}");
        GD.Print($"Bullet Speed: {BulletSpeed}");
        GD.Print($"Bullet Damage: {BulletDamage}");
        GD.Print("===========================================");
        shootTimer.WaitTime = ShootingInterval;
    
    }

    protected override IBulletBuilder CreateBulletBuilder()
    {
        return new RapidBulletBuilder();
    }
}