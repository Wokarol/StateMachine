public class DropOutStack<T>
{
    private int capacity;
    private T[] items;
    private int top = 0;
    public int Count { get; private set; }

    public DropOutStack(int capacity)
    {
        this.capacity = capacity;
        items = new T[capacity];
        Count = 0;
    }

    public void Push(T item)
    {
        if (Count < capacity)
            Count += 1;
        items[top] = item;
        top = (top + 1) % items.Length;
    }
    public T Pop()
    {
        if (Count > 0) {
            top = (items.Length + top - 1) % items.Length;
            Count -= 1;
            return items[top];
        }
        return default;
    }
}