```cpy
func fibonacci:int(n:int)
{
    if (n <= 1)
    {
        return n;
    }
    return fibonacci(n - 1) + fibonacci(n - 2);
}

func main:void()
{
    let n:int = input();

    print(fibonacci(n));
}
```