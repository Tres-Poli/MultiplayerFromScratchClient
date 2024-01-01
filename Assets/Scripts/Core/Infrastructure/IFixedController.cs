namespace Core
{
    public interface IFixedController : IController
    {
        void UpdateController(float deltaTime);
    }
}