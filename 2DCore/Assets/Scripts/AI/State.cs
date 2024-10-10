public abstract class State
{
    public abstract void Enter(ObjectBase entity);

    public abstract void Exit(ObjectBase entity);

    public abstract void Excute(ObjectBase entity);
}
