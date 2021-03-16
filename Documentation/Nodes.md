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

This nodes animates anchored position of a Rect component on a target transform. 

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





