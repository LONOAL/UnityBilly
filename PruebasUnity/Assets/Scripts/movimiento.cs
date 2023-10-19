using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moverse : MonoBehaviour
{
    private float horizontal;
    private float speed = 4f;
    private float jumpPower = 10f;
    private bool isFacingRight = true;
    private bool canDash = true;
    private bool isDashing;
    private bool isAttacking;
    private float dashingPower = 5f;
    private float dashingTime = 0.25f;
    private float attackTime = 0.25f;
    private float dashingCooldown = 0.5f;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private Animator animator;

    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        animator.SetFloat("Speed", Mathf.Abs(horizontal));

    if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            animator.SetBool("isJumping", true);
            animator.SetBool("isOnAir",true);
        }

    if (Input.GetKeyUp(KeyCode.Space) && rb.velocity.y > 0f)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 0.5f);
        }

    if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
    {
        animator.SetBool("isDashing", true);
        StartCoroutine(Dash());
    }

    if (Input.GetKeyDown(KeyCode.Z))
    {
        animator.SetBool("isAttacking", true);
        StartCoroutine(Attack());
    }


    animator.SetBool("isDashing", isDashing);
    animator.SetBool("isAttacking", isAttacking);
    
    Flip();
    isFalling();
    animator.SetBool("isOnAir", !Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer));
    }

    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
    }

    private void isFalling()
    {
        if (rb.velocity.y < 0)
        {
            animator.SetBool("isJumping", false);
            animator.SetBool("isFalling", true);
        }else
            animator.SetBool("isFalling", false);
    }

    private bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private void Flip()
    {
        if (isFacingRight && horizontal < 0f || !isFacingRight && horizontal > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    
    private IEnumerator Dash()
        {
            canDash = false;
            isDashing = true;
            float originalGravity = rb.gravityScale;
            rb.gravityScale = 0f;
            rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
            tr.emitting = true;
            yield return new WaitForSeconds(dashingTime);
            tr.emitting = false;
            rb.gravityScale = originalGravity;
            isDashing = false;
            yield return new WaitForSeconds(dashingCooldown);
            canDash = true;
        }

    private IEnumerator Attack()
    {
        isAttacking = true;
        yield return new WaitForSeconds(attackTime);
        isAttacking = false;
    }
}