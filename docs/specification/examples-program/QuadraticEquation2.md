```cpy
func calculate_discriminant:float(a:float, b:float, c:float)
{
    return b** 2 - 4 * a * c;
}

func main:void()
{
    let a:float = input();
    let b:float = input();
    let c:float = input();

    if (a == 0)
    {
        if (b == 0)
        {
            print(0);
        }
        else
        {
            print(1, - c / b);
        }
    }
    else
    {
        let d:float = calculate_discriminant(a, b, c);

        if (d < 0)
        {
            print(0);
        }
        elif (d == 0)
        {
            let x:float = - b / (2 * a);
            /// x можно заменить на x1 для для проверки области видимости
            print(1, x);
        }
        else
        {
            let x1:float = (- b + d ** 0.5) / (2 * a);
            let x2:ifloatnt = (- b - d ** 0.5) / (2 * a);
            /// возведение в степень можно заменить на встроенную функцию pow(d, 0,5)
            print(2, x1, x2)
        }
    }
}
```
