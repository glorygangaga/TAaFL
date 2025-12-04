
| Название          | Описание                                             | Входные данные   | Выходные данные                       | Проверка                                                |
| ----------------- | ---------------------------------------------------- | ---------------- | ------------------------------------- | ------------------------------------------------------- |
| SumNumbers        | складывает два числа                                 | два числа        | сумма чисел                           | объявление переменных, ввод/вывод, выражения, операторы |
| CircleSquare      | вычисляет площадь круга по радиусу                   | радиус круга     | площадь круга                         | констант и возведедения в степень                       |
| QuadraticEquation | находит два корня квадратного уравнения              | три коэффициента | два корня                             | сложные выражения, приоритетность оперторов             |
| Factorial         | считает факториал входного числа                     | число N          | факториал N                           | цикл for, пользовательские функции                      |
| Fibonacci         | считает N-е число Фибоначчи                          | число N          | N-е число Фибоначчи                   | двойная рекурсия, простые условия                       |
| QuadraticEquation | находит корни квадратного уравнения или их отстутвие | три коэффициента | количество корней и вычисленные корни | сложные, вложенные условия                              |
| FizzBuzz          | определяет кратность чисел в цикле                   | число N          | результат кратности                   | проверка всех типо данных, кроме float                  |
| ToLower           | переводит строку в нижний регистр                    | строка           | строка в нижнем регистре              |                                                         |

---

# Примеры программ для lw4

### SumNumbers
```cpy
func main:void()
{
    let a:int = input();
    let b:int = input();
    let result:int = a + b;

    print(result);
}
```

---

### CircleSquare

```cpy
func main:void()
{
    let radius:float = input();
    let area:float = Pi * r ** 2;
    print(area);
}
```

---

### QuadraticEquation

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

---

# Примеры программ для lw5

### Factorial — читает число N и считает его факториал N!, используя цикл 

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

---

### Fibonacci — читает число N и считает N-е число Фибоначчи, используя рекурсию

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

---

### QuadraticEquation — решение квадратного уравнения с определением нуля, одного или двух корней
Программа должна читать три числа: коэффициенты квадратного уравнения
Программа должна правильно определять количество корней уравнения и выводить сначала количество корней (0, 1 или 2), а затем вычисленные корни

```cpy
func calculate_discriminant:int(a:int, b:int, c:int)
{
    return b**2 - 4 * a * c;
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
            let x1:int = (- b + d ** 0.5) / (2 * a);
            let x2:int = (- b - d ** 0.5) / (2 * a);
            /// возведение в степень можно заменить на встроенную функцию pow(d, 0,5)
            print(2, x1, x2)
        }
    }
}
```
---

#  Примеры программ для lw6 

---

 ### FizzBuzz — читает число за числом в цикле и далее печатает:

“Fizz”, если число делится на 3
“Buzz”, если делится на 5
“FizzBuzz”, если число делится и на 3, и на 5
Само число в остальных случаях

```cpy
func fizzBuzz:str(n:int)
{
    let divisibleBy3:bool = (i % 3 == 0);
    let divisibleBy5:bool = (i % 5 == 0);
    let out:str = "";
    if (divisibleBy3)
    {
        out =  out + "Fizz";
    }
    if (divisibleBy5)
    {
        out = out + "Buzz";
    }
    else
    {
        out = out + toString(i);
    }

    return out;
}

func main:void()
{
    let num:int;
    while(true)
    {
        num = input();
        if (num == 0)
        {
            break;
        }
        print(fizzBuzz(num));
    }
}

```
---

### Пример 2: CheckPalindrome (str, bool)

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

