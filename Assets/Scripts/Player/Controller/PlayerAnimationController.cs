using UnityEngine;
/// <summary>
/// Skrip ini bertindak sebagai "Juru Cat" atau penerjemah.
/// Tugasnya hanya satu: menerima data dari Player.cs dan mengubahnya menjadi perintah untuk komponen Animator.
/// Ini memisahkan logika game dari logika visual, membuat kedua sistem lebih mudah dikelola.
/// </summary>
[RequireComponent(typeof(Animator))]
public class PlayerAnimator : MonoBehaviour
{
    private Animator anim;
    
    // Menggunakan Animator.StringToHash adalah optimisasi.
    // Daripada mengirim "XVelocity" (sebuah string) setiap frame, kita ubah menjadi ID integer sekali saja.
    // Ini lebih cepat dan aman dari kesalahan pengetikan.
    private readonly int xVelocityHash = Animator.StringToHash("XVelocity");
    private readonly int yVelocityHash = Animator.StringToHash("YVelocity");
    private readonly int isGroundedHash = Animator.StringToHash("IsGrounded");
    private readonly int isWallSlidingHash = Animator.StringToHash("IsWallSliding");
    private readonly int jumpsLeftHash = Animator.StringToHash("JumpsLeft");
    private readonly int jumpTriggerHash = Animator.StringToHash("Jump");

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    /// <summary>
    /// Menerima data terbaru dari Player.cs dan memperbarui semua parameter di Animator.
    /// </summary>
    public void UpdateAnimationParameters(Rigidbody2D rb, bool isGrounded, bool isWallSliding, int jumpsLeft)
    {
        anim.SetFloat(xVelocityHash, Mathf.Abs(rb.velocity.x));
        anim.SetFloat(yVelocityHash, rb.velocity.y);
        anim.SetBool(isGroundedHash, isGrounded);
        anim.SetBool(isWallSlidingHash, isWallSliding);
        anim.SetInteger(jumpsLeftHash, jumpsLeft);
    }
    /// <summary>
    /// Memanggil trigger Jump di Animator.
    /// </summary>
    public void TriggerJump()
    {
        anim.SetTrigger(jumpTriggerHash);
    }
}