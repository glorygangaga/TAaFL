```cpy
func main:void()
{
    let a:int = input();
    let b:int = input();
    let c:int = input();

    let d:int = b ** 2 - 4 * a * c;

    let x1:int = (- b + d ** 0.5) / (2 * a);
    let x2:int = (- b - d ** 0.5) / (2 * a);
    print(x1);
    print(x2);
}
```