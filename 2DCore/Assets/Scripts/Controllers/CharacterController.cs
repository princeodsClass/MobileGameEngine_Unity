using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [SerializeField]
    float _fWalkSpeed = 2f;

    [SerializeField]
    float _fRunSpeed = 4f;

    [SerializeField]
    float _fDistance4Run = 1.5f;

    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private BoxCollider2D _boxCollider;

    private Vector2 _mousePosition;
    private Vector2 _screenBottomLeft;
    private Vector2 _screenTopRight;

    private Vector2 _direction;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _boxCollider = gameObject.GetComponent<BoxCollider2D>();

        _screenBottomLeft = Camera.main.ScreenToWorldPoint(new Vector2(0, 0));
        _screenTopRight = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, Screen.height));
    }

    private void Update()
    {
        _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (IsMouseInsideCollider())
        {
            SetMoving(false);
        }
        else if (IsMouseOnScreen())
        {
            SetMoving(true);
        }
        else
        {
            SetMoving(false);
        }
    }

    private bool IsMouseInsideCollider()
    {
        return _boxCollider.OverlapPoint(_mousePosition);
    }

    private bool IsMouseOnScreen()
    {
        return _mousePosition.x >= _screenBottomLeft.x && _mousePosition.x <= _screenTopRight.x &&
               _mousePosition.y >= _screenBottomLeft.y && _mousePosition.y <= _screenTopRight.y;
    }

    private void FlipSprite(bool flip)
    {
        _spriteRenderer.flipX = flip;
    }

    private float GetMooveSpeed()
    {
        return GetDistance() ? _fRunSpeed : _fWalkSpeed;
    }

    private void SetMoving(bool state)
    {
        _animator.SetBool("isMove", state);
        _animator.SetBool("isFar", GetDistance());
    }

    private bool GetDistance()
    {
        return Vector2.Distance(_mousePosition, new Vector2(transform.position.x, transform.position.y)) > _fDistance4Run;
    }

    public void MoveToTarget()
    {
        if (IsMouseInsideCollider())
        {
            SetMoving(false);
        }
        else if (IsMouseOnScreen())
        {
            SetMoving(true);

            _direction = (_mousePosition - (Vector2)transform.position).normalized;

            transform.position += (Vector3)_direction * GetMooveSpeed() * Time.deltaTime;
            FlipSprite(_mousePosition.x < transform.position.x);
        }
        else
        {
            SetMoving(false);
        }
    }
}
