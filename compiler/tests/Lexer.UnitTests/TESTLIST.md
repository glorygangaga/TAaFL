# Список тестов

## Класс Lexer

- [ ] Разбор SELECT без FROM: `SELECT 2025;`
- [ ] Разбор FROM: `SELECT first_name FROM student;`
- [ ] Поддержка множественных полей: `SELECT first_name, last_name, email FROM student;`
- [ ] Определение ключевых слов без учёта регистра: `select first_name, last_name FrOM student;`
- [ ] Оператор сложения: `SELECT count + 1 FROM counter;`
- [ ] Оператор вычитания: `SELECT starts_at - ends_at + 1 FROM meeting;`
- [ ] Оператор умножения и десятичная дробь: `SELECT radius, 2 _ 3.14159265358 _ radius FROM circle;`
- [ ] Оператор деления: `SELECT duration / 2 FROM phone_call;`
- [ ] Разбор WHERE и операторы сравнения: `SELECT circle WHERE radius > 10 AND radius < 25 AND NOT is_deleted;`
- [ ] Разбор WHERE и операторы сравнения: `SELECT circle WHERE radius >= 10 AND radius <= 25;`
- [ ] Разбор OR и NOT: `SELECT to_be OR NOT to_be;`
- [ ] Разбор однострочных комментариев: `-- ...`
