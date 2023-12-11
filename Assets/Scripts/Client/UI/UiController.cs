namespace Common.UI
{
    public abstract class UiController<T> : IUiController where T : UiView
    {
        protected readonly T View;

        public UiController(T view)
        {
            View = view;
            View.Initialize();
        }

        public abstract void Finite();
    }
}