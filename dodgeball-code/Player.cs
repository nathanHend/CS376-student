using UnityEngine;

/// <summary>
/// Control the player on screen
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    /// <summary>
    /// Player rigidbody
    /// </summary>
    private Rigidbody2D rb;

    /// <summary>
    /// Prefab for the orbs we will shoot
    /// </summary>
    public GameObject OrbPrefab;

    /// <summary>
    /// How many frames have been run and the firing cooldown
    /// </summary>
    private int frames = 0;
    private int frameCooldown = 5;

    /// <summary>
    /// How fast our engines can accelerate us
    /// </summary>
    public float EnginePower = 1;
    
    /// <summary>
    /// How fast we turn in place
    /// </summary>
    public float RotateSpeed = 1;

    /// <summary>
    /// How fast we should shoot our orbs
    /// </summary>
    public float OrbVelocity = 10;

    /// <summary>
    /// Initialize on runtime
    /// </summary>
    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Handle moving and firing.
    /// Called by Uniity every 1/50th of a second, regardless of the graphics card's frame rate
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void FixedUpdate()
    {
        Manoeuvre();
        MaybeFire();
    }

    /// <summary>
    /// Fire if the player is pushing the button for the Fire axis
    /// Unlike the Enemies, the player has no cooldown, so they shoot a whole blob of orbs
    /// </summary>
    void MaybeFire()
    {
        frames++;
        if (Input.GetButton("Fire") && frames % frameCooldown == 0) 
        { 
            FireOrb();
        }
    }

    /// <summary>
    /// Fire one orb.  The orb should be placed one unit "in front" of the player.
    /// transform.right will give us a vector in the direction the player is facing.
    /// It should move in the same direction (transform.right), but at speed OrbVelocity.
    /// </summary>
    private void FireOrb()
    {
        Vector3 orbPosition = transform.position + transform.right;

        GameObject orb = Instantiate(OrbPrefab, orbPosition, Quaternion.identity);
        Rigidbody2D orbRb = orb.GetComponent<Rigidbody2D>();
        orbRb.velocity = transform.right * OrbVelocity;
    }

    /// <summary>
    /// Accelerate and rotate as directed by the player
    /// Apply a force in the direction (Horizontal, Vertical) with magnitude EnginePower
    /// Note that this is in *world* coordinates, so the direction of our thrust doesn't change as we rotate
    /// Set our angularVelocity to the Rotate axis time RotateSpeed
    /// </summary>
    void Manoeuvre()
    {
        // Get the input
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");


        Vector2 moveForce = new Vector2(moveX, moveY) * EnginePower;

        rb.AddForce(moveForce);
        // Rotate
        rb.angularVelocity = Input.GetAxis("Rotate") * RotateSpeed;
    }

    /// <summary>
    /// If this is called, we got knocked off screen.  Deduct a point!
    /// </summary>
    // ReSharper disable once UnusedMember.Local
    void OnBecameInvisible()
    {
        ScoreKeeper.ScorePoints(-1);
    }
}
