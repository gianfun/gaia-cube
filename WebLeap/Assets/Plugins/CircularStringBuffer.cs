using System;


public class CircularStringBuffer
{
    private string[] array;

    private int current = 0;

    private object locker = new object();

    public int Count
    {
        get;
        private set;
    }

    public int Capacity
    {
        get;
        private set;
    }

    public bool IsEmpty
    {
        get;
        private set;
    }

    public CircularStringBuffer(int capacity)
    {
        this.Capacity = capacity;
        this.array = new string[this.Capacity];
        this.current = 0;
        this.Count = 0;
        this.IsEmpty = true;
    }

    public virtual void Put(ref string item)
    {
        lock (this.locker)
        {
            if (!this.IsEmpty)
            {
                this.current++;
                if (this.current >= this.Capacity)
                {
                    this.current = 0;
                }
            }
            if (this.Count < this.Capacity)
            {
                this.Count++;
            }
            lock (this.array)
            {
                this.array[this.current] = item;
            }
            this.IsEmpty = false;
        }
    }

    public string Get(int index = 0)
    {
        string t;
        lock (this.locker)
        {
            if (this.IsEmpty || index > this.Count - 1 || index < 0)
            {
                t = string.Empty;
            }
            else
            {
                int num = this.current - index;
                if (num < 0)
                {
                    num += this.Capacity;
                }
                t = this.array[num];
            }
        }
        return t;
    }

    public void Resize(int newCapacity)
    {
        lock (this.locker)
        {
            if (newCapacity > this.Capacity)
            {
                string[] array = new string[newCapacity];
                int num = 0;
                for (int i = this.Count - 1; i >= 0; i--)
                {
                    string t = this.Get(i);
                    array[num++] = t;
                }
                this.array = array;
                this.Capacity = newCapacity;
            }
        }
    }
}

