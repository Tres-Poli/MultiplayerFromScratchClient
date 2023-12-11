namespace Core
{
    public interface IFixedController : IController
    {
        void FixedUpdate(float deltaTime);
    }
}