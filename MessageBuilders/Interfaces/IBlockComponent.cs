namespace MessageBuilders.Interfaces
{
    public interface IBlockComponent<T>
    {
        string Key { get; }

        T Result { get; }

        void SetSubComponent(IBlockComponent<T> comp);
    }
}