using UnityEngine;

public class RotateWithDirection : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    public void UpdateWithRotation(Vector2 direction)
    {
        if (spriteRenderer.flipX && direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (!spriteRenderer.flipX && direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }

        animator.SetFloat("DirectionX", direction.x);
        animator.SetFloat("DirectionY", direction.y);

        animator.SetBool("Walking", true);
    }

    public void StopMovement()
    {
        animator.SetBool("Walking", false);
    }
}
