using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private Transform GFX;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform feetPos;
    [SerializeField] private float groundDistance = 0.25f;
    [SerializeField] private float jumpTime = 0.3f;
    [SerializeField] private Vector3 originalRotation;
    [SerializeField] private float slideRotationAngle = 15f;
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float colliderSize;
    [SerializeField] private Vector3 originalScale;
    [SerializeField] private CapsuleCollider2D playerCollider;
    [SerializeField] private AudioSource player_jumps;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;
    private bool isGrounded = false;
    private bool isJumping = false;
    private float jumpTimer;
    private float startTime;
    private bool isPlaying;
    public float minSwipeDistY;
    bool touchJump;

	public float minSwipeDistX;
		
	private Vector2 startPos;
    GameManager gm;

    private void Start() {
        gm = GameManager.Instance;
        originalRotation = GFX.localEulerAngles;
        originalScale = transform.localScale;
        originalColliderSize = playerCollider.size;
        originalColliderOffset = playerCollider.offset;
    }

    private void Update() {
        isGrounded = Physics2D.OverlapCircle(feetPos.position, groundDistance, groundLayer);
        isPlaying = gm.isPlaying;

        if (isGrounded && isPlaying) {
            animator.SetBool("isMidAir", false);
            animator.SetBool("isMoving", true);
        }
        if (isGrounded && (Input.GetButtonDown("Jump") || touchJump)) {
            player_jumps.Play();
            animator.SetBool("isJumping", true);
            isJumping = true;
            rb2d.velocity = Vector2.up * jumpForce;
        }

        if (isJumping && (Input.GetButton("Jump") || touchJump)) {
            
            if (jumpTimer < jumpTime) {
                rb2d.velocity = Vector2.up * jumpForce;

                jumpTimer += Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump") || touchJump) {
            animator.SetBool("isJumping", false);
            animator.SetBool("isMidAir", true);
            isJumping = false;
            jumpTimer = 0;
        }

        if (isGrounded && Input.GetButton("Slide")) {
            animator.SetBool("isSliding", true);
            animator.SetBool("isMoving", false);
            
            transform.localScale = new Vector3(originalScale.x, crouchHeight, originalScale.z);

            GFX.localEulerAngles = new Vector3(GFX.localEulerAngles.x, GFX.localEulerAngles.y, slideRotationAngle);

            playerCollider.size = new Vector2(originalColliderSize.x, originalColliderSize.y * colliderSize);
            playerCollider.offset = new Vector2(originalColliderOffset.x, originalColliderOffset.y - (originalColliderSize.y - originalColliderSize.y * 1.1f));

            if(isJumping) {
                transform.localScale = originalScale;
                GFX.localEulerAngles = originalRotation;

                playerCollider.size = originalColliderSize;
                playerCollider.offset = originalColliderOffset;
            }
        }

        if (Input.GetButtonUp("Slide")) {
            animator.SetBool("isSliding", false);
            animator.SetBool("isMoving", true);

            transform.localScale = originalScale;
            GFX.localEulerAngles = originalRotation;

            playerCollider.size = originalColliderSize;
            playerCollider.offset = originalColliderOffset;
        }

        
        //Mobile Controlls
        if (Input.touchCount > 0) {
            Debug.Log("touch");
			
			Touch touch = Input.touches[0];
			
			switch (touch.phase) {
				
			case TouchPhase.Began:

				startTime = Time.time;
				
				startPos = touch.position;
				
				break;

			case TouchPhase.Ended:

					float swipeDistVertical = (new Vector3(0, touch.position.y, 0) - new Vector3(0, startPos.y, 0)).magnitude;
                    float swipeDuration = Time.time - startTime;
                    float adjustedJumpForce = CalculateAdjustedJumpForce(swipeDistVertical, swipeDuration);

					if (swipeDistVertical > minSwipeDistY) {
                        float swipeValue = Mathf.Sign(touch.position.y - startPos.y);
                        if (swipeValue > 0 && isGrounded) { // up swipe
                            Debug.Log("Swipe Up");
                            touchJump = true;
                            // Apply the adjusted jump force
                            rb2d.velocity = Vector2.up * adjustedJumpForce;
                        }
                    }
                break;
			}
		}else {
            touchJump = false;
        }
    }
    private float CalculateAdjustedJumpForce(float swipeDist, float swipeDuration)
    {
        // Example calculation: increase jump force linearly with swipe distance and duration
        float calculatedForce = jumpForce + (swipeDist * swipeDuration * 1f);

        // Define the maximum allowed jump force
        float maxJumpForce = 25f; // Set this to your desired maximum jump force

        // Clamp the calculated force to ensure it doesn't exceed the maximum
        return Mathf.Clamp(calculatedForce, jumpForce, maxJumpForce);
    }
}