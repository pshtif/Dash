# EXPRESSION PARAMETERS

As mentioned in the general documentation Dash Animation System supports more complex parameter evaluation than basic parameters.

### Nested property evaluation

For example it supports nested property evaluation for a parameters as follows.

```c#
[target.position.x]
```

Which will resolve the x property of the target transform position and evalute to float. To utilize nested evaluation parameter needs to be in square brackets to enable the . character as a valid character for parameter name.

### Node reference evaluation

Dash parameter can also be a reference to other property within the evaluating graph. It utilizes a special syntax as follows.
```c#
[$DelayNode2.time]
```
The brackets are used to tell the evaluator that this parameter contains special characters, the $ sign tells it that this part is a reference to node model through it's ID since each node inside Dash graph has its own unique ID. The part after the . is telling it the name of the property we are referencing from the target node model.

Additionally the nested evaluation works here as well:
```c#
[$AnimateAnchoredPosition6.fromPosition.x]
```

Where you are referencing directly the X part from the Vector2 position.


### Reserved parameters

Dash also internally implements reserved parameters. These are commonly used parameters that are evaluated internally by Dash. At the moment there is really a short list of such parameters but this list will get bigger once the most common use cases are covered.

> ***mousePosition***  
Returns a Vector2 value of current Input.mousePosition

> ***controller***  
Returns a DashController instance of a graph that evaluates this parameter