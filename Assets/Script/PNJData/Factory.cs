public abstract class Factory<T, DataType>
{
    public DataType Data { get; }

    public Factory(DataType data)
    {
        Data = data;
    }

    public T PickElement()
    {
        return Pick();
    }

    protected abstract T Pick();
}