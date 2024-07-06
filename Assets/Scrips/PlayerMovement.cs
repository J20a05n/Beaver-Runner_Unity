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
    private bool isPlaying;
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
        if (isGrounded && Input.GetButtonDown("Jump")) {
            player_jumps.Play();
            animator.SetBool("isJumping", true);
            isJumping = true;
            rb2d.velocity = Vector2.up * jumpForce;
        }

        if (isJumping && Input.GetButton("Jump")) {
            
            if (jumpTimer < jumpTime) {
                rb2d.velocity = Vector2.up * jumpForce;

                jumpTimer += Time.deltaTime;
            } else {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump")) {
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
    }
}
