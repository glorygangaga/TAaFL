### 1. Пример программы

```
struct Point
{
    x: int;
    y: int;
}

func add:int(a:int, b:int)
{
    return a + b;
}

func main:void()
{
    let p:Point = { x:1.0, y:2.0 };
    const msg:str = "Sum: ";
    let result:int = add(3, 5);
    print(msg, result);
}
```

## 2.Ключевые особенности языка

* **Язык регистронезависимый**
* **Статическая строгая типизация** — тип переменной указывается при объявлении
* **Поддержка функций и структур**
* **Две категории переменных:** изменяемые (`let`) и константные (`const`)
* **Блочная область видимости** — переменные, объявленные в блоке `{ ... }`, не видимы вне его
* **Явные разделители инструкций (`;`)**
* **Поддержка ввода-вывода** через встроенные функции `input()`, `print()`
* **Операторы управления:** `if`, `while`, `for`, `return`
* **Функции имеют тип возвращаемого значения.**
* **Не поддерживаются неявные преобразования**


## 3. Структура программы

Программа в  языке имеет **единую точку входа** — функцию `main`, аналогично языкам **C/C++**.
Все остальные объявления (переменных, функций, структур) должны находиться **до** или **после** `main`, но **на верхнем уровне** (вне других функций).

Программа состоит из:

* глобальных объявлений (переменные, константы, структуры, функции);
* обязательного объявления функции `main`, которая является точкой входа программы.

---

## 4. Область действия (Scope)

Каждая функция формирует собственную **область видимости** (scope).
Внутри функции можно объявлять локальные переменные, которые скрывают глобальные с тем же именем.
Блоки (`{ ... }`) также могут иметь свои области видимости для временных переменных.

Типы областей:

* **глобальная область** — для объявлений вне функций;
* **локальная область** — внутри функций или блоков `{ ... }`.


**Семантические правила:**

* Повторное объявление переменной в одной области видимости — **ошибка**.
* Обращение к необъявленной переменной — **ошибка**.
* Объявления внутри блока видимы **только до конца этого блока**.

---

## 5. Переменные и константы

В языке различаются **изменяемые** (`let`) и **неизменяемые** (`const`) значения.

**Семантические правила:**

* Каждая переменная должна быть инициализирована при объявлении.
* Константы (`const`) нельзя изменять после объявления.
* Переменные (`let`) могут быть переопределены только в новой области видимости.

---

## 6. Ввод и вывод

Ввод и вывод реализованы через встроенные функции `input` и `print`.

---

## 7. Инструкции (Statements)

Инструкции выполняются **последовательно** внутри функций.
Каждая инструкция завершается точкой с запятой `;`.

---

## 8. Разделители инструкций

Инструкции разделяются **точкой с запятой `;`**.
Это облегчает чтение и разбор кода, а также позволяет восстанавливать работу парсера после ошибок.

---

## 9. Семантические правила

1. Нельзя объявить переменную с тем же именем в одной области видимости.
2. Нельзя использовать переменную до её объявления.
3. Функция `main` должна быть объявлена **ровно один раз**.
4. Функция `main` не принимает аргументов и не возвращает значение.
5. Все ветви управления должны корректно завершаться (`return` при наличии возвращаемого типа).

---


## 10. EBNF-грамматика
```
(* === ПРОГРАММА === *)

program =
{ top_level_declaration },
main_function ;

(* === ФУНКЦИЯ MAIN === *)

main_function =
"func", "main", ":", "void", "(", ")", block ;

(* === ВЕРХНЕУРОВНЕВЫЕ ОБЪЯВЛЕНИЯ === *)

top_level_declaration =
value_declaration
| function_declaration
| struct_declaration ;

(* === ИНСТРУКЦИИ === *)

statement =
expression_statement
| assignment_statement
| value_declaration
| block
| if_statement
| while_statement
| for_statement
| switch_statement
| break_statement
| continue_statement
| return_statement
| empty_statement ;

expression_statement =
expression, ";" ;

assignment_statement =
assignable_expr, "=", expression, ";" ;

empty_statement = ";" ;

block =
"{", { statement }, "}" ;

(* === ОБЪЯВЛЕНИЯ === *)

value_declaration =
variable_declaration
| constant_declaration;

variable_declaration =
"let", identifier, ":", type [ "=", expression ], ";";

constant_declaration =
"const", identifier, ":", type "=", expression, ";";

(* === ОБЪЯВЛЕНИЯ ФУНКЦИЙ === *)

function_declaration =
"func", identifier, ":", type, "(", [ parameter_list ], ")", block ;

parameter_list =
parameter, { ",", parameter } ;

parameter =
identifier, ":", type ;

(* === ОБЪЯВЛЕНИЯ СТРУКТУР === *)

struct_declaration =
"struct", identifier, "{", { field_declaration }, "}" ;

field_declaration =
identifier, ":", type, ";" ;

(* === ТИПЫ === *)

type =
"int"
| "float"
| "str"
| "bool"
| "void"
| identifier
| type, "[]" ;

(* === УПРАВЛЯЮЩИЕ КОНСТРУКЦИИ  === *)

if_statement =
"if", "(", expression, ")", block, { elif_clause }, [ else_clause ] ;

elif_clause = 
    "elif", "(", expression, ")", block ;

else_clause = 
    "else", block ;

while_statement =
"while", "(", expression, ")", block ;

for_statement =
"for", "(", for_init, [ expression ], ";", [ for_update ], ")", block ;

for_init =
variable_declaration
| expression_statement
| empty_statement  ;

for_update =
expression
| empty_statement ;

switch_statement =
"switch", "(", expression, ")", "{", { case_clause }, [ default_clause ],"}" ;

case_clause =
"case", expression, ":", { statement } ;

default_clause =
"default", ":", { statement } ;

break_statement = "break", ";" ;

continue_statement = "continue", ";" ;

return_statement =
"return", [ expression ], ";" ;

```