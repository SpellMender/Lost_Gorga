﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lost_Gorga
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance { get; private set; }

        public bool Interacting { get; private set; }
        public bool Moving { get; private set; }
        public bool Jumping { get; private set; }
        public bool Hanging { get; private set; }
        public bool FlipX { get; private set; }

        public bool KeepVertical { get; set; }

        [SerializeField]
        float moveSpeed = 5f;
        [SerializeField]
        float jumpMagnitude = 5f;

        [SerializeField]
        AudioClip jumpSound;

        Rigidbody2D player;
        SpriteRenderer sprite;
        Animator anim;
        AudioSource playerAudio;

        //DEBUG
        public bool afterFirstFrame = false;
        float inputMagnitude = 0;

        [SerializeField]
        int timeScale;

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

        // Start is called before the first frame update
        void Start()
        {
            player = GetComponent<Rigidbody2D>();
            sprite = GetComponent<SpriteRenderer>();
            anim = GetComponentInChildren<Animator>();
            playerAudio = GetComponent<AudioSource>();
        }

        private void FixedUpdate()
        {
            Move();
            Jump();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            if (afterFirstFrame)
            {
                SetInputFlags();
            }
            else
            {
                afterFirstFrame = true;
            }
            Interact();
            Animate();
        }

        void Move()
        {
            player.velocity = new Vector2(inputMagnitude * moveSpeed, player.velocity.y);
        }

        void Jump()
        {
            if (Input.GetKey(KeyCode.Space) && IsGrounded())
            {
                PlaySound(jumpSound);
                player.velocity = new Vector2(player.velocity.x, jumpMagnitude);
            }
        }


        //-----------------------------------------------------
        // Source: Code Monkey - 3 ways to do a Ground Check in Unity
        // Link: https://youtu.be/c3iEl5AwUF8?t=576
        //-----------------------------------------------------
        bool IsGrounded()
        {
            float detectionRange = 0.05f;
            BoxCollider2D collider = GetComponent<BoxCollider2D>();
            RaycastHit2D groundDetection = Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, Vector2.down, detectionRange, LayerMask.GetMask("Ground"));
            return groundDetection.collider != null;
        }

        void SetInputFlags()
        {
            if (inputMagnitude != 0)
            {
                // Set Moving Flag
                if (!Moving) Moving = true;

                // Set FlipX Flag
                if (inputMagnitude < 0) { FlipX = true; }
                else if (inputMagnitude > 0) { FlipX = false; }
            }
            else if (Moving) Moving = false;


            // Resolves input issue between scenes by reading horizontal input every frame on each specific key
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow))
            {
                inputMagnitude = Input.GetAxisRaw("Horizontal");
            }
            else
            {
                inputMagnitude = 0;
                Moving = false;
            }

            if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                inputMagnitude = 0;
                Moving = false;
            }


            if (!IsGrounded() && !Jumping) Jumping = true;
            else if (Jumping && IsGrounded()) Jumping = false;
        }

        void Interact()
        {
            if (Input.GetKeyUp(KeyCode.E)) { Interacting = true; }
        }

        void Animate()
        {
            // Face the sprite using FlipX flag
            if (sprite.flipX != FlipX) sprite.flipX = FlipX;

            // Change between animation states
            if (Jumping)
            {
                anim.Play("player_jump");
            }
            else if (Moving)
            {
                anim.Play("player_run");
            }
            else
            {
                anim.Play("player_idle");
            }
        }

        void PlaySound(AudioClip sound)
        {
            playerAudio.clip = sound;
            playerAudio.Play();
        }

    }
}
