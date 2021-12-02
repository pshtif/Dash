# EXPRESSION 

Parametric properties in Dash can either use direct values or a string expression that will be evaluated at runtime. To switch a property from value type to expression type you need to toggle the icon right beside the property in the inspector.

Expression can consist of parameters, operators, functions or values. All of these have their own rules.

## Parameters

Parameters inside expression are identificators that should be evaluated externally. Basically anything that is not a function, value or operator will be handled as a parameter and evaluator will try to resolve it. Generally parameters should contain only alpha numeric characters and they should never start with a number.

In following example the evaluator will try to resolve test as a variable and then add 5 and divide it by 2. This expression consists only of operators and single parameter.
```c#
(test+5)/2
```

For more detailed documentation on Dash parameters can go to [Expression Parameters documentation](ExpressionParameters.md)

## Operators

Expressions support the standard operators as follows.

### Additive
```
+ -
```
### Multiplicative
```
* / %
```
### Logical
```
|| &&
```
### Bitwise
```
& | ^ << >>
```
### Relational
```
= == != <> < <= > >=
```
### Primary
```
( )
```
### Unary
```
! - ~
```

## Values

These are basic value types that are supported.

### Integer
```
1234
- any number without the floating point will be evaluated as Int32
```

### Float
```
123.456
- any number containing floating point will be evaluated as Float
```

### Double
```
2.41e7
- any number using scientific notation will be evaluated as Double
```

### String
```
'hello world'
-any part of expression enclosed with ' will be evaluated as a string value, special characters need to be escaped by \
```

### Boolean
```
true
- boolean can be either true or false
```

## Functions

A function is made of a name followed by braces, containing optionally any number of arguments as values or parameters or even other functions.

```
RandomF(1,2)
```

For documentation on implemented functions you can go to [Expression Functions documentation](ExpressionFunctions.md)

## Macros

Starting from Dash 0.6.1 it is now possible to utilize macros inside expressions. Macros are nothing else than predefined expressions that you can define in Tools>Dash>Expression Editor Window which can be then used instead of an expression or inside a more complex expresion. The syntax is as follows.

```c#
{SOME_MACRO}+1
```

Macro name is always uppercase alphanumeric string with the only exception being the character _ which can be also used. Also macros when used inside expressions should always be wrapped inside curly braces {MACRO} even though the braces are not part of its name.

Current limitation of macros is that macros inside macros are not supported (future support is considered).