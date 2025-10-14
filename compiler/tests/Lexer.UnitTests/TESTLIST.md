# Список тестов

## Класс Lexer

Ключевые слова:

- [ ] Инициализация переменной: `let name: str = "Alice";`
- [ ] Инициализация константы: `const name: str = "Alice";`
- [ ] Ввод: `const userData:str = input();`
- [ ] Вывод: `print("Hello world");`
- [ ] Объявление функции: `func nothing:void() {}`
- [ ] Возврат из функции: `func greet:str(user:str) { return user; }`
- [ ] Объявление структуры: `struct car { str brand; str model; int year;}`
- [ ] Импорт модулей: `import std;`
- [ ] Коментарии после кода: `let x:int = 5; // Комментарий после кода`
- [ ] Коментарии в одной строке: `// Это комментарий`
- [ ] Многострочный комментарий: `/*
   Это многострочный комментарий
*/`

Инициализация возможных типов данных:

- [ ] Инициализация строкового типа: `const string:str = "text";`
- [ ] Инициализация целочисленного типа: `const number:int = 11;`
- [ ] Инициализация действительного типа: `const number:float = 1.1;`
- [ ] Инициализация логического типа: `const isNumber:bool = false;`
- [ ] Инициализация массива: `const arr:int[] = [1, 2, 4, 5, 7];`

Операторы:

- [ ] Арифметический оператор суммы и вычитания: `const value:int = x + y - z;`
- [ ] Арифметический оператор умноженя и возведения в степень: `const area:float = PI * R ** 2;`
- [ ] Арифметический оператор деления и модуля: `const value:float = 7 % 2 / 4;`
- [ ] Арифметический оператор инкремента: `const value:int = x++;`
- [ ] Арифметический оператор декремента: `const value:int = x--;`
- [ ] Оператор сравнения равно: `const isSame:bool = x == y;`
- [ ] Оператор сравнения не равно: `const isNotSame:bool = x != y;`
- [ ] Оператор сравнения больше: `const isBigger:bool = x > y;`
- [ ] Оператор сравнения меньше: `const isLess:bool = x < y;`
- [ ] Оператор сравнения больше или равно: `const isBiggerOrSame:bool = x >= y;`
- [ ] Оператор сравнения меньше или равно: `const isLessOrSame:bool = x <= y;`
- [ ] Логические Операторы: `const value:bool = x > z and y > z or z == not(w);`

Ветвление и циклы:

- [ ] if ветвление: `if (x > 0) { print("positive"); }`
- [ ] else ветвь: `if (x > y) { print(x); } else { print(y); }`
- [ ] for цикл: `for (let i:int = 0; i < length; i++) { print(i); }`
- [ ] while цикл: `while (true) { print("Hello world"); }`
