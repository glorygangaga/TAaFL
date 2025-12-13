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
| "void"
| identifier;

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
| empty_statement ;

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

(* === ВЫРАЖЕНИЯ === *)

expression =
ternary_expr ;

ternary_expr =
or_expr, [ "?", expression, ":", ternary_expr ] ;
    
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
prefix_expr, { ("\*" | "/" | "%" | "//"), prefix_expr } ;

prefix_expr =
{ prefix_operator }, power_expr ;

prefix_operator =
"++" | "--" | "+" | "-" | "not" ;

power_expr =
postfix_expr, { "\*\*", power_expr } ;

postfix_expr = primary_expr, { postfix_operator } ;

postfix_operator =
function_call
| index_access
| "++"
| "--" ;

primary_expr =
identifier
| literal
| boolean
| constant
| "null"
| "(", expression, ")" ;

(* === ПРИСВАИВАЕМЫЕ ВЫРАЖЕНИЯ === *)

assignable_expr =
identifier
| "(", assignable_expr, ")" ;


(* === ВЫЗОВЫ ФУНКЦИЙ И ЛИТЕРАЛЫ === *)

function_call = "(", [ argument_list ], ")" ;
index_access = "[", expression, "]" ;

expression_list = expression, { ",", expression } ;

argument_list = expression, { ",", expression } ;

(* === Основные лексемы === *)

literal = number | string | boolean | "null" ;
number = integer | float ;
integer = digit, { digit } ;
float = digit, { digit }, ".", digit, { digit } ;
string = '"', { character - '"' | escape*sequence }, '"' ;
character = ? любой символ Unicode ? ;
escape_sequence = "\\", ( "\"" | "\\" | "n" | "t" ) ;
boolean = "true" | "false" ;
identifier = (letter | "*"), { letter | digit | "\_" } ;
constant = "Pi" | "Euler" ;

letter = "A" | "B" | ... | "Z" | "a" | "b" | ... | "z" | "\_" ;
digit = "0" | "1" | ... | "9" ;
```