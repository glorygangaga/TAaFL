 # Грамматика верхнего уровня языка  CPY

### 1. Примеры программ

**Пример 1: Основные конструкции**

```cpy
/// Глобальная константа
const Pi:float = 3.1415;

func factorial:int(n:int)
{
    if (n <= 1) 
    {
        return 1;
    }
    return n * factorial(n - 1);
}

func main:void()
{
    let name:str = input("Введите имя: ");
    print("Привет, ", name);

    let num:int = 5;
    print("Факториал ", num, " равен ", factorial(num));
}
```

**Пример 2: Управление потоком (ветвление и циклы)**
```cpy
func main:void()
{
    let score:int = 85;

    if (score >= 90) 
    {
        print("Отлично!");
    }
    elif (score >= 75) 
    {
        print("Хорошо!");
    }
    else 
    {
        print("Нужно подтянуть!");
    }

    let i:int = 0;
    while (i < 3) 
    {
        print("Счетчик: ", i);
        i = i + 1;
    }

    for (let j:int = 0; j < 3; j = j + 1) 
    {
        if (j == 1) 
        {
            continue; 
        }
        print("j = ", j);
    }
}
```

---

## 2.Ключевые особенности языка

* **Структура программы:** Точка входа — функция `main:void()`. Все глобальные объявления (функции, константы) должны находиться до неё
* **Статическая строгая типизация:** Тип переменной или функции указывается явно и проверяется на этапе компиляции
* **Последовательное исполнение:** Инструкции выполняются сверху вниз
* **Управление памятью и областью видимости:** Блочная область видимости (`{}`). Поддержка изменяемых (`let`) и константных (`const`) переменных
* **Управление потоком:** Поддерживаются инструкции `if`/`elif`/`else`, `while`, `for`
* **Ввод-вывод:** Через встроенные функции `input()` и `print()`
* **Чёткий синтаксис:** Обязательные разделители инструкций (`;`)

---

## 3. EBNF-грамматика
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
| function_declaration;

(* === ИНСТРУКЦИИ === *)

statement =
expression_statement
| assignment_statement
| value_declaration
| block
| input_statement
| print_statement
| if_statement
| while_statement
| for_statement
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

(* === ВВОД-ВЫВОД === *)

input_statement =
"input", "(", [ expression ], ")" ";" ;

print_statement =
"print", "(", [ expression_list ], ")" ";" ;

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

(* === ТИПЫ === *)

type =
"int"
| "float"
| "str"
| "bool"
| "void";

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
"for", "(", variable_declaration, expression, ";",  expression , ")", block ;

break_statement = "break", ";" ;

continue_statement = "continue", ";" ;

return_statement =
"return", [ expression ], ";" ;

```
