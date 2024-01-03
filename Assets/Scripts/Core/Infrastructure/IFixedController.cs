namespace Core
{
    public interface IFixedController : IController
    {
        void UpdateFixedController(float deltaTime);
    }
}