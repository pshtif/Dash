# NODES

Nodes are basic building blocks of Dash graphs. All logic happens inside nodes the graph and Dash just execute connections between them. 

## Model

Each node contains its own model which represents the properties of that particular node (model is present even if there are no properties). State of the model doesn't change between different executions of the node, each execution just gets value from the model and executes upon it.

## Node

As stated each node has its model that holdes the parameters while the node itself executes upon them. Nodes are stateless and execution with the same parameters will always execute the same way.

## Node Type Categories

There are currently six different node type categories that groups nodes with similar functionality.

### Animation Nodes

Animation nodes animate properties or a state of a target in some way either using tweens or Unity animation. Each animation node inherits AnimationNodeBase as well as AnimationNodeModelBase so they have a common animation functionality as well as common retargeting functionality. 

Animation common properties are *__time__*, *__delay__*, and *__easing__*.

Retargeting common properties are *__retarget__*, *__isChild__*, *__useReference__*, *__target/targetReference__* which enables you to change current target in the NodeDataFlow specifically for this animation node without it being actually changed in the flow. If *__retarget__* is not checked the node will execute on the target in the NodeFlowData.

#### *AnimateAnchoredPosition*

This nodes animates anchored position of a RectTransform component on a target. 

```
useFrom - boolean check if you want to specify from position as well
fromPosition - specifies the position value where the anchored position should animate from (applicable only when useFrom is checked)
isFromRelative - specifies if fromPosition is relative value (applicable only when useFrom is checked)

toPosition - specifies the position value where the anchored position should animate to
isToRelative - specifies if the toPosition is relate value
```

#### *AnimateColor*

This node animates color or alpha of either *TextMeshPro, Image* or *CanvasGroup*

```
targetType - specifies what target type we are animating, some of them have full color capabilities some only alpha
toAlpha - specifies the alpha value to which we are going to animate (applicable only when targetType is CanvasGroup)
isToRelatibe - specifies if toAlpha is relative value (applicable only when targetType is CanvasGroup)

toColor - specifies the color value to which we are going to animate (applicable only when targetType is not CanvasGroup)
```

#### *AnimateRotation*

This node animates rotation of a Transform/RectTransform component on a target.

```
useFrom - boolean check if you want to specify from rotation as well
fromRotation - specifies the rotation value where the rotation should animate from (applicable only when useFrom is checked)
isFromRelative - specifies if fromRotation is relative value (applicable only when useFrom is checked)

toRotation - specifies the rotation value where the rotation should animate to
isToRelative - specifies if the toRotation is relate value
```

#### *AnimateScale*

This node animates scale of a Transform/RectTransform component on a target.

```
useFrom - boolean check if you want to specify from scale as well
fromScale - specifies the scale value where the scale should animate from (applicable only when useFrom is checked)
isFromRelative - specifies if fromScale is relative value (applicable only when useFrom is checked)

toScale - specifies the scale value where the scale should animate to
isToRelative - specifies if the toScale is relate value
```

#### *AnimateSizeDelta*

This node animates sizeDelta of a RectTransform component on a target. It basically animates with and height of a RectTransform bsaed on their anchor type.

```
useFrom - boolean check if you want to specify from sizeDelta as well
fromSizeDelta - specifies the sizeDelta value where the sizeDelta should animate from (applicable only when useFrom is checked)
isFromRelative - specifies if fromSizeDelta is relative value (applicable only when useFrom is checked)

toSizeDelta - specifies the sizeDelta value where the sizeDelta should animate to
isToRelative - specifies if the toSizeDelta is relate value
```

#### *AnimateTextNode* *__[EXPERIMENTAL]__*

This node animates text of TextMeshPro component on target. It animates text per character basis, there will be multiple types of per character animations available currently only supports scaling in experimental stage.

*__WARNING Experimental nodes may have different properties later and you may run into serialization issues of existing graphs in new version of Dash.__*
```
characterDelay - delay in time between animation of two characters
```

#### *AnimateSizeDelta*

This node animates sizeDelta of a RectTransform component on a target. It basically animates with and height of a RectTransform bsaed on their anchor type.

```
useFrom - boolean check if you want to specify from sizeDelta as well
fromSizeDelta - specifies the sizeDelta value where the sizeDelta should animate from (applicable only when useFrom is checked)
isFromRelative - specifies if fromSizeDelta is relative value (applicable only when useFrom is checked)

toSizeDelta - specifies the sizeDelta value where the sizeDelta should animate to
isToRelative - specifies if the toSizeDelta is relate value
```

#### *AnimateToRect*

This node animates RectTransform on target to specified RectTransform, you can also specify which part of transform should be take into account position/rotation/scale.

```
toTarget - specifies the target of the RectTransform to animate to

useToPosition - specifies if position should be used in this animation
useToRotation - specifies if rotation should be used in this animation
useToScale - specifies if scale should be used in this animation
```

#### *AnimateWithClip*

This node animates target using a specified Unity AnimationClip or special DashAnimation.

This node doesn't inherit AnimationNodeBase thats why properties like time, delay and easing are specified directly on this node.

```
useAnimationTime - specifies if we want to use time from the specified clip or override it with custom value
time - specifies the duration of the animation (applicable only when useAnimationTime is checked)
delay - specifies the delay to wait till animation starts
easing - specifies the easing for the animation

isReverse - specifies if the animation should play in reverse
isRelative - specifies if the animation should animate properties in relative manner (applicable only if useDashAnimation is checked)
useDashAnimation - specifies if we want to use DashAnimation or standard Unity AnimationClip

source - specifies the DashAnimation asset to use (applicable only when useDashAnimation is checked)
clip - specifies the Unity AnimationClip asset to use (applicable only when useDashAnimation is not checked)
```

#### *AnimateWithPreset*

This node animates target using a specified preset.

Presets are custom implementations for any type of programmatic animation. You can implement your own custom presets by implementing IAnimationPreset interface. This node will then automatically look for any implementation of this interface within the codebase.

```
preset - specifies the preset to use for the animation
```

### Creation Nodes

Creation nodes handle creation or destroyment of objects. 

#### *Destroy*

This node destroys target game object.

This node imherits RetargetNodeBase so it can be locally retargeted without affecting the node data flow.

```
immediate - specifies if DestroyImmediate should be used
```

#### *SpawnImage*

This node spawns a game object with an Image component attached to it.

_This node imherits RetargetNodeBase so it can be locally retargeted without affecting the node data flow._

```
spawnedName - specifies the name that will be assigned as name of the spawned game object
sprite - specifies the sprite that should be assigned to the Image component
position - specifies the anchored position of the spawned game object

setTargetAsParent - specifies if the target should be set as a parent of the spawned game object
retargetToSpawned - specifies if the node flow data should be retargeted to the spawned game object

createSpawnedAttribute - specifies if an attribute should be created within node flow data with the spawned game object as value (applicable only when retargetToSpawned is not checked)
spawnedAttributeName - specifies the name of the attribute with spawned object (applicable only when createSpawnedAttribute is checked)

```