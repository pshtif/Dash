# CUSTOM NODES

In Dash it is very easy to implement custom nodes, even though it is preferred to use Dash provided nodes for compatibility sometimes it is necessary to use custom nodes in your projects. For example when the node needs to be tied to specific part of your project, like caching etc.

## Node Classes

To create a custom node it is as simple as extending two Dash base classes which are NodeBase and NodeModelBase where the former is the execution part of the node and the latter the data/serialized part of node that the execution is run upon.

At the same time the implementation needs to have OnExecuteStart method overriden.

This is the simplest for of custom node.
```c#
public class CustomNode : NodeBase<CustomNodeModel>
{
    protected override void OnExecuteStart(NodeFlowData p_flowData)
    { 
        OnExecuteEnd();
    }
}
```

Whereas the simplest custom model can be completely empty.
```c#
public class CustomNodeModel : NodeModelBase
{
}
```

If you try this you can see that this will create a custom node for you in the menu to pick but the node will have 0 inputs as well as zero outputs. To define additional properties of node we will use attributes as needed.

There are couple of attributes used for nodes to define either visual or functional aspects of the node. 

## Node Attributes

#### OutputCountAttribute
```c#
[OutputCount(1)]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies the number of outputs the node will have.

*If you want to have dynamic number of outputs you need to implement it using custom getters and logic. Documentation here.*

#### InputCountAttribute
```c#
[InputCount(1)]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies the number of inputs the node will have.

*If you want to have dynamic number of inputs you need to implement it using custom getters and logic. Documentation here.*

#### OutputLabelsAttribute
```c#
[OutputCount(2)]
[OutputLabels("Output1","Output2")]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies the UI labels for all the outputs for this node.

#### InputLabelsAttribute
```c#
[InputCount(2)]
[InputLabels("Input1","Input2")]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies the UI labels for all the inputs for this node.

#### CategoryAttribute
```c#
[Category(NodeCategoryType.OTHER, "Examples")]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies the category of the node. NodeCategoryType is an enum for all the current node types that have distinct visual representation in Dash (Animation, Logic...). The second parameter is label which can be different and specifies where in the node sits in the  node creation context menu.

#### ExperimentalAttribute
```c#
[Experimental]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies if the node is marked as experimental.

#### SizeAttribute
```c#
[Size(220,85)]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies the size (width/height) of the node in the editor.

*If you want to have a node with dynamic sizing you need to implement it using custom getters and logic. Documentation here.*

#### TooltipAttribute
```c#
[Tooltip("Animate RectTransform position on current target.")]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies the tooltip for this node, this tooltip is visible in node creation context menu.

#### DisableBaseGUIAttribute
```c#
[DisableBaseGUI]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies that you don't want for Dash to render the base node gui and you will implement the whole rendering yourself in the node implementation.

*For example the Connector node is using this to have completely different GUI than other nodes.*

#### SkinAttribute
```c#
[SkinAttribute("CustomTitle", "CustomBody"]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies custom skins for node UI title and body to offer reskinning of custom nodes.

#### CustomInspectorAttribute
```c#
[CustomInspector]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies that Dash will not use its default inspector when inspecting this node and you will need to implement your own custom inspector.

*Documentation here.*

#### DebugOverrideAttribute
```c#
[DebugOverride]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies if Dash will use default debug of node execution for its internal console or you want to implement your own (or skip it).


#### SingleInstanceAttribute *OBSOLETE*
```c#
[SingleInstance]
public class CustomNodeModel : NodeModelBase
...
```
This attribute specifies that there can be only one node of this type in the graph.

