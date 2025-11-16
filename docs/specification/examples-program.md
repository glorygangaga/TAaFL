
| Название          | Описание                                | Входные данные   | Выходные данные |
| ----------------- | --------------------------------------- | ---------------- | --------------- |
| SumNumbers        | складывает два числа                    | два числа        | сумма чисел     |
| CircleSquare      | вычисляет площадь круга по радиусу      | радиус круга     | площадь круга   |
| QuadraticEquation | находит два корня квадратного уравнения | три коэффициента | два корня       |

---

### SumNumbers
```
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

```
func main:void()
{
    let radius:int = input();
    let area:int = Pi * r ** 2;
    print(area);
}
```

---

### QuadraticEquation

```
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
