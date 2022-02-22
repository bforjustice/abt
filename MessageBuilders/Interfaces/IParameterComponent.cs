namespace MessageBuilders.Interfaces
{
    public interface IParameterComponent<T> : IBlockComponent<T>
    {
        void SetValue(T value);
    }
}