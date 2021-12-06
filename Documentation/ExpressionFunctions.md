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
> ***RandomV2(Float minX, Float maxX, Float minY, Float maxY)***  
Function creates a random Vector2 where each component uses its own min/max.

### RandomInCircle
> ***RandomInCircle()***  
Function creates a random Vector2 inside an unit circle.
> ***RandomInCircle(Float scale)***  
Function creates a random Vector2 inside an unit circle scaled by scale.
> ***RandomInCircle(Float scaleX, scaleY)***  
Function creates a random Vector2 inside an unit circle scaled by scaleX/scaleY

### RandomOnCircle
> ***RandomOnCircle()***  
Function creates a random Vector2 on an unit circle.
> ***RandomOnCircle(Float scale)***  
Function creates a random Vector2 on an unit circle scaled by scale.
> ***RandomOnCircle(Float scaleX, scaleY)***  
Function creates a random Vector2 on an unit circle scaled by scaleX/scaleY

#### Normalize
> ***Normalize(Vector2 value)***
Function returns a normalized Vector2 value.

#### Scale
> ***Scale(Vector2 value1, float value2)***  
Function returns a Vector2 scaled by float.
> ***Scale(Vector2 value1, Vector2 value2)***  
Function returns a Vector2 scaled by another Vector2.

#### Add(Generic)
> ***Add(Vector2 value1, Vector2 value2)***  
Function adds two Vector2 values and returns a Vector2 if evaluated Parameter is Vector2.

## Vector3

#### Vector3
> ***Vector3(Float x, Float y, Float z)***  
Function creates a Vector3.

#### RandomV3
> ***RandomV3()***  
Function creates a random unit Vector3  
> ***RandomV3(Float min, Float max)***  
Function creates a random Vector3 where all components will be randomized within the min/max range.  
> ***RandomV3(Float minX, Float maxX, Float minY, Float maxY, Float minZ, Float maxZ)***  
Function creates a random Vector3 where each component uses its own min/max.

#### Normalize
> ***Normalize(Vector3 value)***
Function returns a normalized Vector3 value.

#### Scale
> ***Scale(Vector3 value1, float value2)***  
Function returns a Vector3 scaled by float.
> ***Scale(Vector3 value1, Vector3 value2)***  
Function returns a Vector3 scaled by another Vector3.

#### Add(Generic)
> ***Add(Vector3 value1, Vector3 value2)***  
Function adds two Vector3 values and returns a Vector3 if evaluated Parameter is Vector3.

## Int

#### GetSiblingIndex
> ***GetSiblingIndex(Transform transform)***  
Function returns a singling index of specified transform

#### Ceil
> ***Ceil(Float value)***  
Function returns a ceiled Float as Int.

## Float

#### RandomF
> ***RandomF(Float min, Float max)***  
Function creates a random float from min to max

#### Magnitude
> ***Magnitude(Vector2 vector)***  
Function returns a magnitude of a Vector2 value.
> ***Magnitude(Vector3 vector)***  
Function returns a magnitude of a Vector3 value.

## Transform

#### GetChild
> ***GetChild(Transform transform, String childName)***  
Function finds a child of a specified transform by its name.

#### GetParent OBSOLETE
> ***GetParent(Transform transform)***  
Function returns a parent of specified transform.

#### GetChildAt
> ***GetChildAt(Transform transform, Int index)***  
Function returns a child of specified transform at index.

