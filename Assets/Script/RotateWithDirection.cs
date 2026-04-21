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

        animator.SetBool("DirectionSide", Mathf.Abs(direction.x) > 0);
        animator.SetBool("DirectionUp", direction.y > 0);
    }
}
