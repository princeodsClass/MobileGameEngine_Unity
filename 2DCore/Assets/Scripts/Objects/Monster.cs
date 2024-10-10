using UnityEngine;

public class Monster : ObjectBase
{
    private float _fMoveSpeed;
    // ��Ÿ ����� ���� ���� ��.

    public float MoveSpeed
    {
        set => _fMoveSpeed = Mathf.Max(0, value);
        get => _fMoveSpeed;
    }

    public override void SetObject(string name)
    {
        base.SetObject(name);

        gameObject.name = $"Monster({Index:D4})_{name}";
    }

    public override void Updated()
    {

    }
}
