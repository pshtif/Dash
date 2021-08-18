# EXPRESSION PARAMETERS

These are function that can be called within parameter expressions. They are sorted by return type rather than category in this documentation as for now it seems like a better option.

Inside Dash parameter can also be a reference to other property within the evaluating graph. It utilizes a special syntax as follows.
```c#
[$DelayNode2.time]
```
The brackets are used to tell the evaluator that this parameter contains special characters, the $ sign tells it that this part is a reference to node model through it's ID since each node inside Dash graph has its own unique ID. The part after the . is telling it the name of the property we are referencing from the target node model.

Additionally this supports nested property lookup so this is also possible:
```c#
[$AnimateAnchoredPosition6.fromPosition.x]
```

Where you are referencing directly the X part from the Vector2 position.
