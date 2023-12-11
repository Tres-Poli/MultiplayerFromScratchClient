namespace Core
{
    public interface IUpdateController : IController
    {
        void Update(float deltaTime);
    }
}