## Пример программы 

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
---

## Ключевые особенности языка
* **Язык регистронезависимый**
* **Статическая строгая типизация** — тип переменной указывается при объявлении
* **Поддержка функций и структур**
* **Две категории переменных:** изменяемые (`let`) и константные (`const`)
* **Блочная область видимости** — переменные, объявленные в блоке `{ ... }`, не видимы вне его
* **Явные разделители инструкций (`;`)**
* **Поддержка ввода-вывода** через встроенные функции `input()`, `print()`
* **Операторы управления:** `if`, `while`, `for`, `return`
* **Функции имеют тип возвращаемого значения.**

---

## Семантические правила

1. **Объявления переменных**

   * Переменная должна быть объявлена **до использования**
   * Повторное объявление переменной **с тем же именем в одной области видимости запрещено**
   * Константа (`const`) не может быть изменена после инициализации
   * Тип присваиваемого выражения должен **соответствовать типу переменной**

2. **Функции**

   * Имя функции уникально в глобальной области видимости
   * Функция должна возвращать значение, если её тип не `void`
   * Параметры функции создают новую область видимости

3. **Область видимости**

   * Каждая функция и каждый блок `{ ... }` создают собственную область видимости
   * Доступ к переменным разрешён только в рамках текущей и внешних областей

4. **Ввод/вывод**

   * Встроенные функции:

     * `input()` — возвращает введённое значение
     * `print()` — выводит значения без переноса строки

---

## EBNF-грамматика 
(* === ПРОГРАММА === *)

program =
    { top_level_declaration },
    main_function,
    { top_level_declaration } ;

(* === ФУНКЦИЯ MAIN === *)

main_function =
    "func", "main", ":", "void", "(", ")", block ;

(* === ВЕРХНЕУРОВНЕВЫЕ ОБЪЯВЛЕНИЯ === *)

top_level_declaration =
      value_declaration
    | function_declaration
    | struct_declaration ;

(* === ОБЪЯВЛЕНИЯ ПЕРЕМЕННЫХ И КОНСТАНТ === *)

value_declaration =
    ( "let" | "const" ), identifier, ":", type, [ "=", expression ], ";" ;

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

(* === ИНСТРУКЦИИ === *)

statement =
      expression_statement
    | assignment_statement
    | value_declaration
    | if_statement
    | while_statement
    | for_statement
    | return_statement
    | io_statement
    | block ;

block =
    "{", { statement }, "}" ;

(* === ОПЕРАТОРЫ УПРАВЛЕНИЯ === *)

if_statement =
    "if", "(", expression, ")", block,
    [ "else", ( if_statement | block ) ] ;

while_statement =
    "while", "(", expression, ")", block ;

for_statement =
    "for", "(", for_init, ";", [ expression ], ";", [ for_update ], ")", block ;

for_init =
      value_declaration
    | expression_statement
    | /* пусто */ ;

for_update =
      expression
    | /* пусто */ ;

return_statement =
    "return", [ expression ], ";" ;

(* === ВВОД/ВЫВОД === *)

io_statement =
   ( "print" | "read" ), "(", [ expression ], ")", ";" ;

argument_list =
    expression, { ",", expression } ;

(* === ВЫРАЖЕНИЯ === *)

expression_statement =
    expression, ";" ;

assignment_statement =
    assignable_expr, "=", expression, ";" ;

assignable_expr =
      identifier
    | member_access_expr
    | index_access_expr ;

member_access_expr =
    (identifier | member_access_expr), ".", identifier ;

index_access_expr =
    (identifier | member_access_expr | index_access_expr), "[", expression, "]" ;

---

