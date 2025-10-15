# Список тестов

## Класс Lexer

Ключевые слова:

- [x] Инициализация переменной: `let name: str = "Alice";`
- [x] Инициализация константы: `const name: str = "Alice";`
- [x] Ввод: `const userData:str = input();`
- [x] Вывод: `print("Hello world");`
- [x] Объявление функции: `func nothing:void() {}`
- [x] Возврат из функции: `func greet:str(user:str) { return user; }`
- [x] Объявление структуры: `struct car { str brand; str model; int year;}`
- [x] Импорт модулей: `import std;`
- [x] Коментарии в одной строке: `// Это комментарий`
- [x] Коментарии после кода: `let x:int = 5; // Комментарий после кода`
- [x] Многострочный комментарий: `/* Это многострочный комментарий */`

Инициализация возможных типов данных:

- [x] Инициализация строкового типа: `const string:str = "text";`
- [x] Инициализация целочисленного типа: `const number:int = 11;`
- [x] Инициализация действительного типа: `const number:float = 1.1;`
- [x] Инициализация логического типа: `const isNumber:bool = false;`
- [x] Инициализация массива: `const arr:int[] = [1, 2, 4, 5, 7];`

Операторы:

- [x] Арифметический оператор суммы и вычитания: `const value:int = x + y - z;`
- [x] Арифметический оператор умноженя и возведения в степень: `const area:float = PI * R ** 2;`
- [x] Арифметический оператор деления и модуля: `const value:float = 7 % 2 / 4;`
- [x] Арифметический оператор постфиксного инкремента: `const value:int = x++;`
- [x] Арифметический оператор постфиксного декремента: `const value:int = x--;`
- [x] Арифметический оператор префиксного инкремента: `const value:int = ++x;`
- [x] Арифметический оператор префиксного декремента: `const value:int = --x;`
- [x] Оператор сравнения равно: `const isSame:bool = x == y;`
- [x] Оператор сравнения не равно: `const isNotSame:bool = x != y;`
- [x] Оператор сравнения больше: `const isBigger:bool = x > y;`
- [x] Оператор сравнения меньше: `const isLess:bool = x < y;`
- [x] Оператор сравнения больше или равно: `const isBiggerOrSame:bool = x >= y;`
- [x] Оператор сравнения меньше или равно: `const isLessOrSame:bool = x <= y;`
- [x] Логические Операторы: `const value:bool = (x > z) and (y > z) or (z == not(w));`

Ветвление и циклы:

- [x] if ветвление: `if (x > 0) { print("positive"); }`
- [x] else ветвь: `if (x > y) { print(x); } else { print(y); }`
- [x] for цикл: `for (let i:int = 0; i < length; i++) { print(i); }`
- [x] while цикл: `while (true) { print("Hello world"); }`
