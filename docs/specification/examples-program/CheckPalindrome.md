```cpy
func toLowerExample:void()
{
    print("Введите строку: ");
    let input_str: str = input();
    
    let lower_str: str = toLower(input_str);
    
    print("Результат: ", lower_str);
}

func main:void()
{
    toLowerExample();
}
```
