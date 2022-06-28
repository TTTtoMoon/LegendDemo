namespace RogueGods
{
    public interface IStateMachine
    {
        public void SetOwner(object owner);
        public void Destroy();
    }
}
