using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lost_Gorga.Alt
{
    public class AltPlayer : MonoBehaviour
    {
        public static AltPlayer instance { get; private set; }

        [SerializeField] float speed = 5, jumpMagnitude = 9, jumpHeight = 5, gravity = 9.8f;

        [SerializeField] AudioClip jumpSound, splashStepSound, slowSplashSound, hardStepSound;
        [SerializeField] int jumpSoundResetTime = 8, waitFramesForInput;
        int jumpSoundResetTimer, waitFrameCountdown;

        float moveDelta, moveInput, jumpDelta, jumpCap , gravDelta, gravVelocity, lastPosY;
        bool isMoving, isJumping, isFalling, jumpInput, isGrounded, flipX;

        public bool afterFirstFrame = true;

        Vector3 motionVector;

        Rigidbody2D playerBody;
        SpriteRenderer playerSprite;
        Animator playerAnim;
        AudioSource playerAudio;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            playerBody = GetComponent<Rigidbody2D>();
            playerSprite = GetComponent<SpriteRenderer>();
            playerAnim = GetComponentInChildren<Animator>();
            playerAudio = GetComponent<AudioSource>();

            // Sets the jump cap when we start
            jumpCap = playerBody.transform.position.y + jumpHeight;

            // Remembers first Y position
            lastPosY = playerBody.transform.position.y;

            waitFrameCountdown = waitFramesForInput;
        }

        private void Update()
        {
            #if !UNITY_WEBGL
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
            #endif

            if (afterFirstFrame)
            {
                SetMovementFlags();
                SetJumpFlags();
                ReadAxes();
            }
            else
            {
                if (waitFrameCountdown-- == 0)
                {
                    afterFirstFrame = true;
                    waitFrameCountdown = waitFramesForInput;
                }
            }

            //TimeDilation();
            Animate();
        }
        void FixedUpdate()
        {
            isGrounded = IsGrounded();
            if (!isFalling && (playerBody.transform.position.y >= jumpCap) || (!isJumping && !jumpInput && !isGrounded && lastPosY >= playerBody.transform.position.y))
            {
                //jumpInput = false;
                //isJumping = false;
                isFalling = true;
                motionVector = new Vector3(motionVector.x, 0);
            }
            ApplyMotion();
            lastPosY = playerBody.transform.position.y;
        }

        float GetPlayerDeltaTime()
        {
            return Time.deltaTime;
        }

        Vector3 Move()
        {
            moveDelta = moveInput * speed * GetPlayerDeltaTime();

            return new Vector3(moveDelta, 0, 0);
        }

        Vector3 Jump()
        {
            jumpDelta = jumpMagnitude * GetPlayerDeltaTime();

            return new Vector3(0, jumpDelta, 0);
        }

        Vector3 Fall()
        {
            gravDelta = gravity * GetPlayerDeltaTime();
            gravVelocity -= gravDelta * GetPlayerDeltaTime();

            return new Vector3(0, gravVelocity, 0);
        }

        void ApplyMotion()
        {
            bool isMotionDelta = false;

            // Motion is relative to current position
            motionVector = transform.position;

            // Add horizontal movement
            if (isMoving)
            {
                motionVector += Move();
                if (!isMotionDelta) isMotionDelta = true;
            }

            // Reset gravity if we're on the ground
            if (isGrounded)
            {
                gravVelocity = 0;
            }
            else
            {
                // Fall if we're not
                motionVector += Fall();
                if (isFalling) motionVector += Fall(); // When going down fall at double speed
                if (!isMotionDelta) isMotionDelta = true;

                if (playerAudio.clip == hardStepSound) PlaySound(false);
            }

            // Add jump from input to landing
            if ((isGrounded && jumpInput) || (isJumping && !isGrounded))
            {
                motionVector += Jump();
                if (!isFalling) motionVector += Jump(); // When going up rise at double speed
                if (!isMotionDelta) isMotionDelta = true;
            }

            // If there is a change apply movement
            if (isMotionDelta) playerBody.MovePosition(motionVector);

        }

        bool IsGrounded()
        {
            Vector3 boundsLimiter = new Vector3(0.15f, 0.4f, 0);
            float detectionRange = 0.05f + boundsLimiter.y / 2;
            BoxCollider2D playerCollider = GetComponent<BoxCollider2D>();
            RaycastHit2D groundDetection = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size - boundsLimiter, 0f, Vector2.down, detectionRange, LayerMask.GetMask("Ground"));

            Color rayColor;
            if (groundDetection.collider != null) rayColor = Color.green; else rayColor = Color.red;
            Debug.DrawRay(playerCollider.bounds.center + new Vector3(-playerCollider.bounds.extents.x + boundsLimiter.x / 2, 0), new Vector3(playerCollider.bounds.size.x - boundsLimiter.x, 0), rayColor);
            Debug.DrawRay(playerCollider.bounds.center + new Vector3(0, playerCollider.bounds.extents.y - boundsLimiter.y / 2), new Vector3(0, -playerCollider.bounds.size.y + boundsLimiter.y - detectionRange, 0f), rayColor);

            // Resets the jump cap when we're on the ground
            if (groundDetection.collider != null)
            {
                jumpCap = playerBody.transform.position.y + jumpHeight;
                isFalling = false; //When we hit the ground we are done falling.
            }

            return groundDetection.collider != null;
        }

        void ReadAxes()
        {
            // Resolves input issue between scenes by reading horizontal input every frame on each specific key
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                moveInput = Input.GetAxisRaw("Horizontal");
            }
            else if (/*(moveInput != 0) && */(Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) || isGrounded)
            {
                moveInput = 0;
                isMoving = false;
            }
        }

        void SetMovementFlags()
        {
            if (moveInput != 0)
            {
                // Set Moving Flag
                if (!isMoving) isMoving = true;

                // Set FlipX Flag
                if (moveInput < 0 && !flipX) { flipX = true; }
                else if (moveInput > 0 && flipX) { flipX = false; }
            }
            else if (isMoving) isMoving = false;
        }

        void SetJumpFlags()
        {
            // Countdown jump sound reset timer
            if (jumpSoundResetTimer > 0) --jumpSoundResetTimer;

            // Set jumpInput flag when we push space on the ground
            if (Input.GetKey(KeyCode.Space) && isGrounded && !jumpInput && !isJumping)
            {
                jumpInput = true;
                if (jumpSoundResetTimer == 0)
                {
                    playerAudio.volume = 0.7f;
                    PlaySound(jumpSound, false);
                    jumpSoundResetTimer = jumpSoundResetTime;
                }
            }
            /*else */if (Input.GetKeyUp(KeyCode.Space))
            {
                isJumping = false;
                isFalling = true;
                //gravVelocity = 0;
            }

            // Swap jumping flags when we're off the ground
            if (!isGrounded && jumpInput && !isJumping)
            {
                jumpInput = false;
                isJumping = true;
            }

            // Unset isJumping when we're back on the ground
            if (isGrounded && isJumping)
            {
                isJumping = false;
            }
        }

        void Animate()
        {
            // Face the sprite using flipX flag
            if (playerSprite.flipX != flipX) playerSprite.flipX = flipX;

            // Set animation state
            if (!isGrounded) playerAnim.Play("player_jump");
            else if (isMoving) playerAnim.Play("player_run");
            else playerAnim.Play("player_idle");
        }

        void PlaySound(AudioClip sound, bool looping = false)
        {
            if (playerAudio.clip != sound) playerAudio.clip = sound;
            if (playerAudio.loop != looping) playerAudio.loop = looping;
            if (!playerAudio.isPlaying) playerAudio.Play();
        }

        void PlaySound(bool stop)
        {
            if (stop) playerAudio.Stop();
            playerAudio.loop = false;
        }

    }
}
