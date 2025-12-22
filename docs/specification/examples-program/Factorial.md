```cpy
func factorial:int(n:int)
{
    let result:int = 1;

    for (let i:int = 1; i < n + 1; i++)
    {
        result = result * i;
    }

    return result;
}

func main:void()
{
    let n:int = input();

    let result:int = factorial(n);

    print(result);
}
```