using UnityEngine;

/// <summary>
/// Controls the behavior of an on-screen enemy.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    /// <summary>
    /// How fast the enemy can accelerate
    /// </summary>
    public float EnginePower = 1;

    /// <summary>
    /// How close it tries to get to the player
    /// </summary>
    public float ApproachDistance = 5;

    /// <summary>
    /// How fast the orbs it fires should move
    /// </summary>
    public float OrbVelocity = 10;

    /// <summary>
    /// How heavy the orbs it fires should be
    /// </summary>
    public float OrbMass = .5f;

    /// <summary>
    /// Period the enemy should wait between shots
    /// </summary>
    public float CoolDownTime = 1;

    /// <summary>
    /// Next time the enemy should fire
    /// </summary>
    private float NextFireTime = 0;

    /// <summary>
    /// Prefab for the orb it fires
    /// </summary>
    public GameObject OrbPrefab;

    /// <summary>
    /// Transform from the player object
    /// Used to find the player's position
    /// </summary>
    private Transform player;

    /// <summary>
    /// Our rigid body component
    /// Used to apply forces so we can move around
    /// </summary>
    private Rigidbody2D rigidBody;

    /// <summary>
    /// Vector from us to the player
    /// </summary>
    private Vector2 OffsetToPlayer => player.position - transform.position;

    /// <summary>
    /// Unit vector in the direction of the player, relative to us
    /// </summary>
    private Vector2 HeadingToPlayer => OffsetToPlayer.normalized;

    /// <summary>
    /// Initialize player and rigidBody fields
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void Start()
    {
        player = FindObjectOfType<Player>().transform;
        rigidBody = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Fire if it's time to do so
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void Update()
    {
        if (Time.time >= NextFireTime)
        {
            Fire();
            NextFireTime = Time.time + CoolDownTime;
        }
    }

    /// <summary>
    /// Make a new orb, place it next to us, but shifted one unit in the direction of the player
    /// Set its mass to OrbMass and its velocity to OrbVelocity units per second, in the direction of the player
    /// </summary>
    private void Fire()
    {
        Vector3 orbPosition = transform.position + new Vector3(HeadingToPlayer.x, HeadingToPlayer.y, 0f) * 1f;
        GameObject orb = Instantiate(OrbPrefab, orbPosition, Quaternion.identity);

        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();

        orbRb.velocity = HeadingToPlayer * OrbVelocity;

        orbRb.mass = OrbMass;
    }

    /// <summary>
    /// Accelerate in the direction of the player, unless we're nearby
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void FixedUpdate()
    {
        var offsetToPlayer = OffsetToPlayer;
        var distanceToPlayer = offsetToPlayer.magnitude;
        var controlSign = distanceToPlayer > ApproachDistance ? 1 : -1; 
        rigidBody.AddForce(controlSign * (EnginePower / distanceToPlayer) * offsetToPlayer);
    }

    /// <summary>
    /// If this is called, we're off screen, so give the player a point.
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void OnBecameInvisible()
    {
        ScoreKeeper.ScorePoints(1);
    }
}
