# EXPRESSION FUNCTIONS

These are function that can be called within parameter expressions. They are sorted by return type rather than category in this documentation as for now it seems like a better option.

## Vector2

### FromToRect
> ***FloatToRect(RectTransform from, RectTransform to)***  
Function calculates position transform from one rectangle space to another.

### Vector2
> ***Vector2(Float x, Float y)***  
Function creates a Vector2.

### RandomV2
> ***RandomV2()***  
Function creates a random unit Vector2  
> ***RandomV2(Float min, Float max)***  
Function creates a random Vector2 where both components will be randomized within the min/max range.  
> ***RandomV2(Float minX, Float maxX, Float minX, Float maxX)***  
Function creates a random Vector2 where each component uses its own min/max.

#### Normalize
> ***Normalize()***
Function returns a normalized Vector2 value.

#### Scale
> ***Scale(Vector2 value)***  
Function returns a Vector2 scaled by float.

#### Add(Generic)
> ***Add(Vector2 value1, Vector2 value2)***  
Function adds two Vector2 values and returns a Vector2 if evaluated Parameter is Vector2.

#### Random(Generic)
> ***Random(Float min, Float max)***  
Function creates a random Vector2 value when evaluated Parameter is Vector2 and both components will be randomized within the min/max range.

## Vector3

#### Vector3
> ***Vector3(Float x, Float y, Float z)***  
Function creates a Vector3.

#### RandomV3
Function creates a random Vector3.

#### Normalize
Function returns a normalized Vector3 value.

#### Scale
Function returns a Vector3 scaled by float.

#### Add<T>
Function adds two Vector3 values and returns a Vector3 if evaluated Parameter is Vector3.

#### Random<T>
Function creates a random Vector3 value when evaluated Parameter is Vector3.

## Int

#### GetChildIndex
Function returns index of a child transform specified as parameter.

#### Ceil
Function returns a ceiled Float as Int.

#### Random<T>
Function creates a random Int value when evaluated Parameter is Int.

## Float

#### RandomF
Function creates a random Float value.

#### Magnitude
Function returns a magnitude of a vector value.

#### Random<T>
Function creates a random Float value when evaluated Parameter is Float.

## Transform

#### GetChild
Function finds a child of a specified transform by its name.

#### GetParent
Function returns a parent of specified transform.

#### GetChildAt
Function returns a child of specified transform at index.

