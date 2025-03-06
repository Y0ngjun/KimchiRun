using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Settings")]
    public float jumpForce;

    [Header("References")]
    public Rigidbody2D PlayerRigidBody;
    public Animator PlayerAnimator;
    public BoxCollider2D PlayerCollider;
    public AudioSource audioSource;  
    public AudioClip jumpSound;    
    public AudioClip itemPickupSound;
    public AudioClip goldenSound;
    public AudioClip damagedSound;

    private bool isGrounded = true;
    public bool isInvincible = false;
    public bool doubleJumped = false;

    public float maxJumpSpeed = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            audioSource.PlayOneShot(jumpSound);
            PlayerRigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            PlayerAnimator.SetInteger("state", 1);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !doubleJumped)
        {
            audioSource.PlayOneShot(jumpSound);
            PlayerRigidBody.AddForceY(jumpForce * PlayerRigidBody.gravityScale * 0.7f, ForceMode2D.Impulse);
            doubleJumped = true;
        }

        if (PlayerRigidBody.linearVelocityY > maxJumpSpeed)
        {
            PlayerRigidBody.linearVelocityY = maxJumpSpeed;
        }
    }

    public void KillPlayer()
    {
        PlayerCollider.enabled = false;
        PlayerAnimator.enabled = false;
        PlayerRigidBody.AddForceY(jumpForce, ForceMode2D.Impulse);
    }

    void Hit()
    {
        GameManager.Instance.lives -= 1;
    }

    void Heal()
    {
        GameManager.Instance.lives = Mathf.Min(3, GameManager.Instance.lives + 1);
    }

    void StartInvincible(float f)
    {
        isInvincible = true;
        Invoke("StopInvincible", f);
    }

    void StopInvincible()
    {
        isInvincible = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Platform")
        {
            if (!isGrounded)
            {
                PlayerAnimator.SetInteger("state", 2);
            }
            isGrounded = true;
            doubleJumped = false;
        }

    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "Enemy")
        {
            audioSource.PlayOneShot(damagedSound);
            if (!isInvincible)
            {
                Destroy(collider.gameObject);
                Hit();
            }
        }
        else if (collider.gameObject.tag == "food")
        {
            audioSource.PlayOneShot(itemPickupSound);
            Destroy(collider.gameObject);
            Heal();
        }
        else if (collider.gameObject.tag == "golden")
        {
            audioSource.PlayOneShot(goldenSound);
            Destroy(collider.gameObject);
            StartInvincible(5f);
        }
    }
}
