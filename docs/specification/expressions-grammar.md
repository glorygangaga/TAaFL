* Нужно описать грамматику выражений для своего языка программирования

#  Грамматика выражений языка

## 1. Общие принципы синтаксиса выражений

Выражения могут состоять из:

* **литералов** (int, float, str, bool, константы Pi, Euler);
* **идентификаторов** (в том числе имена переменных и функций);
* **вызовов функций**;
* **арифметических**, **сравнительных** и **логических операторов**;
* **операторов инкремента и декремента (`++`, `--`)**;
* **группирующих скобок** `()`;
* **операции доступа к полям** через `.` (точку);
* **унарных операторов** `+`, `-`, `not`;
* **индексирования массивов ([])**;
* **операции присваивания** `=`;
* **оператора указания типа** `:` (в объявлениях, не внутри выражений)

---

##  2. Операторы

### Арифметические операторы

| Символ | Описание                    |
| :----- | :-------------------------- |
| `+`    | сложение или унарный плюс   |
| `-`    | вычитание или унарный минус |
| `*`    | умножение                   |
| `/`    | деление                     |
| `//`   | деление без остатка         |
| `%`    | остаток от деления          |
| `**`   | возведение в степень        |

---

### Операторы инкремента и декремента
| Символ | Описание                            |
| :----- | :---------------------------------- |
| `++`   | увеличение значения переменной на 1 |
| `--`   | уменьшение значения переменной на 1 |

```
++i;   // префиксный инкремент
i++;   // постфиксный инкремент
--j;   // префиксный декремент
j--;   // постфиксный декремент
```
  
---

### Операторы сравнения

| Символ | Описание         |
| :----- | :--------------- |
| `==`   | равно            |
| `!=`   | не равно         |
| `<`    | меньше           |
| `>`    | больше           |
| `<=`   | меньше или равно |
| `>=`   | больше или равно |

---

### Логические операторы

| Символ | Описание             |
| :----- | :------------------- |
| `and`  | логическое И         |
| `or`   | логическое ИЛИ       |
| `not`  | логическое отрицание |

---

### Прочие операторы

| Символ | Описание                            |
| :----- | :---------------------------------- |
| `.`    | доступ к полю структуры или объекта |
| `()`   | вызов функции или группировка       |
| `[]`   | индексирование массива              |
| `=`    | присваивание                        |
| `:`    | указание типа (в объявлениях)       |

---

##  3. Приоритет и ассоциативность операторов

| Приоритет (высокий → низкий) | Операторы              | Ассоциативность |
| :--------------------------- | :--------------------- | :-------------- |
| 1                            | `()` `[]` `.`          | слева           |
| 2                            | постфиксные `++`, `--` | слева           |
| 3                            | префиксные `++`, `--`  | справа          |
| 4                            | `**`                   | справа          |
| 5                            | `*`, `/`, `%`, `//`    | слева           |
| 6                            | `+`, `-`, `not`        | слева           |
| 7                            | `<`, `>`, `<=`, `>=`   | слева           |
| 8                            | `==`, `!=`             | слева           |
| 9                            | `and`                  | слева           |
| 10                           | `or`                   | слева           |
| 11                           | `=`                    | справа          |


---

## 4. Встроенные функции для чисел

| Имя              | Описание                        |
| ---------------- | ------------------------------- |
| `abs(x)`         | модуль числа                    |
| `min(a, b, ...)` | минимальное значение            |
| `max(a, b, ...)` | максимальное значение           |
| `pow(a, b)`      | возведение в степень            |
| `round(x)`       | округление до ближайшего целого |
| `ceil(x)`        | округление вверх                |
| `floor(x)`       | округление вниз                 |

**Числовые константы:**

* `Pi` (3.14159265358)
* `Euler` (2.71828182846)

---

## 5. Грамматика выражений (в нотации ISO EBNF)

**Основные лексемы**
literal = number | string;
number = integer | float ;
integer = digit, { digit } ;
float = digit, { digit }, ".", digit, { digit } ;
string = '"', { character - '"' | escape_sequence }, '"' ;
character = ? любой символ Unicode ? ;
escape_sequence = "\\", ( "\"" | "\\" | "n" | "t" ) ;
boolean      = "true" | "false" ;
identifier   = (letter | "_"), { letter | digit | "_" } ;
constant = "Pi" | "Euler" ;

letter       = "A" | "B" | ... | "Z" | "a" | "b" | ... | "z" | "_" ;
digit        = "0" | "1" | ... | "9" ;


**Выражения**

expression =
      or_expr ;
  
or_expr =
      and_expr, { "or", and_expr } ;
      
and_expr =
      equality_expr, { "and", equality_expr } ;
      
equality_expr =
      comparison_expr, { ("==" | "!="), comparison_expr } ;

comparison_expr =
      arith_expr, { ("<" | ">" | "<=" | ">="), arith_expr } ;

arith_expr =
      term_expr, { ("+" | "-"), term_expr } ;

      
term_expr =
      power_expr, { ("*" | "/" | "%" | "//"), power_expr } ;

power_expr =
      unary_expr, { "**", unary_expr } ;

unary_expr =
      ( "++" | "--" ), unary_expr
    | ( "+" | "-" | "not" ), unary_expr
    | postfix_expr ;

postfix_expr = primary_expr, { postfix_operator } ;

postfix_operator = 
      function_call
    | member_access  
    | index_access
    | "++" 
    | "--" ;

function_call = "(", [ argument_list ], ")" ;
member_access = ".", identifier ;
index_access = "[", expression, "]" ;

primary_expr = 
      identifier
    | literal
    | boolean
    | constant
    | array_literal
    | struct_literal
    | "(", expression, ")" ;

array_literal = "[", [ expression_list ], "]" ;
expression_list = expression, { ",", expression } ;

struct_literal = identifier, "{", [ field_initializer_list ], "}" ;
field_initializer_list = field_initializer, { ",", field_initializer } ;
field_initializer = identifier, ":", expression ;

argument_list = expression, { ",", expression } ;


**ИНСТРУКЦИИ**

program = { statement } ;

statement = 
      expression_statement
    | assignment_statement
    | variable_declaration
    | function_declaration
    | struct_declaration
    | if_statement
    | while_statement
    | for_statement
    | return_statement
    | block ;

expression_statement = expression, ";" ;

assignment_statement = assignable_expr, "=", expression, ";" ;

assignable_expr = 
      identifier
    | member_access_expr
    | index_access_expr ;

member_access_expr = (identifier | member_access_expr), ".", identifier ;
index_access_expr = (identifier | member_access_expr | index_access_expr), "[", expression, "]" ;

variable_declaration = ( "let" | "const" ), identifier, ":", type, [ "=", expression ], ";" ;

type = "int" | "float" | "str" | "bool" | "void" | identifier | type, "[]" ;

function_declaration = "func", identifier, ":", type, "(", [ parameter_list ], ")", block ;

struct_declaration = "struct", identifier, "{", { field_declaration }, "}" ;
field_declaration = identifier, ":", type, ";" ;

parameter_list = parameter, { ",", parameter } ;
parameter = identifier, ":", type ;

if_statement = "if", "(", expression, ")", block, [ "else", ( if_statement | block ) ] ;

while_statement = "while", "(", expression, ")", block ;

for_statement = "for", "(", for_init, ";", [ expression ], ";", [ for_update ], ")", block ;
for_init = [ variable_declaration | expression_statement ] ;
for_update = expression ;

return_statement = "return", [ expression ], ";" ;

block = "{", { statement }, "}" ;
