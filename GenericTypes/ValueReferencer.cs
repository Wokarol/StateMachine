public class ValueReferencer<T>
{
    public T Value { get; set; }
    public bool Valid => !typeof(T).IsClass || Value != null;
}
