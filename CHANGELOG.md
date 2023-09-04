# CHANGE LOG

All notable changes to this project will be documented in this file.

## RELEASES

### Release 0.14.6 - 5.9.2023

#### Added

- added SetVariable overload for non generic version of setting of existing variables.
- added graph utility functions HasInputOfName, HasOutputOfName, HasOnCustomEventOfName
- added migration utility for Parameters from value

#### Changed

- changed DashEditorConfig now moved to Assets/Editor/Resources instead of Assets/Resources/Editor
- changed CreateAttributeNode now highlights attributes with non expression names that collide with reserved names
- changed SubGraphs and SubGraph node are no longer experimental, big hurray! 
- changed Input/OutputNode now cannot have whitespace in name [WARNING]
- changed new attribute DisallowWhitespace that are used for any property that shouldn't contain whitespace
- changed usage of different input textfields for various GUI elements
- changed a lot of refactoring and architecture change to avoid locked dependencies and move closer to Nodemon architecture
- changed variable names now cannot contain whitespace are now autostripped of whitespace in renaming [WARNING]
- changed variable inspector now shows variables that use reserved names as red to avoid them or atleast be aware that collision may occur
- changed OnFinishedDelay is now parameter in ForLoopNode

#### Fixed

- fixed CreateAttributeNode now correctly creates a new list of attributes if there were none
- fixed deserialization of cached empty empty graphs crashed on null exception
- fixed ForLoopNode now correctly calculates time for reverse loops

### Release 0.14.5 - 11.6.2023

#### Changed

- changed CreateAttributeNode refactor to be more robust

#### Fixed

- fixed Vector4 casting for GUI Vector4Field

### Release 0.14.4 - 14.4.2023

#### Changed

- changed temporarily removed some parameter functions like Promote to Variable
- changed internal handling of parameter in context menus
- changed zoom value is now stored per graph 

#### Fixed

- fixed node context menu going for infinite recursion and crashing Unity editor

### Release 0.14.3 - 4.4.2023

#### Added

- added show/invalidate node ids for exposed properties in controller advanced options

#### Changed

- changed AddListener collections handling for better performance and less GC overhead
- changed dependency upped for DashTween 0.1.9

### Release 0.14.2 - 5.3.2023

#### Added

- added StopNode which can be used to stop the graph itself or just the connected part

#### Changed

- changed version number handling directly from package info (static for embedded)
- changed INodeAccess interace was refactored to internal

#### Fixed

- fixed various null checks on potentionally unserialized Parameters
- fixed AnimateToTransform node check if expression evaluated to null to avoid exception

### Release 0.14.1 - 31.01.2023

#### Added

- added stopOnDisable property to stop graph when gameobject or DashController is disabled
- added bindOnDisable to bind OnDisable to a graph input

#### Changed

- changed autoStart/autoStartInput and autoOnEnable/autoOnEnableInput renamed to bind*

### Release 0.13.12 - 17.1.2023

#### Added

- added Tools->Dash->Reserialize menu to find and force reserialize all DashGraph assets

### Release 0.14.0 - 16.01.2023

#### Added

- added graph creation caching
- added custom serialization formatters for nodes, connections and parameters
- added additional logs and verbose graph scanning

#### Changed

- changed namespace and assembly definitions merge
- changed refactoring through the whole codebase for redundancy cleanup and overall code
- changed editor mouse decoupling
- changed SubGraph bound serialization to node [BREAKING]
- changed access interfaces to internal access
- changed SerializedValue for broader usecases

### Release 0.13.11 - 10.1.2023

#### Added

- added --verbose option for more detailed graph scanning

### Release 0.13.10 - 9.1.2023

#### Changed

- changed sub graph initialization to the same initialization of main graphs for performance

### Release 0.13.9 - 04.01.2023

#### Fixed

- fixed expression switching for targetName in RetargetNodeModelBase migration

### Release 0.13.8 - 27.12.2022

#### Added

- added Disconnect Node in node context menu for quick node disconnection from all
- added Create Dash Controller in unity create object menu
- added option to allow attribute type changes in data flow
- added CreateAttributeNode that has more robust attribute creation and will sunset SetAttributeNode
- added option for direct attribute creation for SubGraphNode
- added SetVariable<T> for DashVariablesController

#### Changed

- changed creation of graph copies for asset graphs with 5-6x performance boost
- changed all Dash menu items refactored from individual windows to single class for readability
- changed default time value for all animation nodes to 0
- changed default useExpression value for all retargeting nodes to true
- changed renamed target property to targetName property in Retargeting nodes
- changed DashTween dependency upped to 0.1.8
- changed removed IInternalVariableAccess, IInternalGraphAccess and refactored to internal
- changed removed IEditorControllerAccess and refactored to internal

#### Fixed

- fixed refactoring of variables now correctly handles Parameter types
- fixed delayed call on migration window to avoid update window focus before Unity start
- fixed SetAttribute now evaluates expression as untyped if specific type is not set
- fixed now editor correctly doesn't allow connection of node to itself (still possible through reroutes)

### Release 0.13.7 - 4.12.2022

#### Fixed

- fixed annoying bug with graph scanning upon Unity startup and stuck windows

### Release 0.13.6 - 2.12.2022

#### Added

- added new experimental nodes CustomExecute and CustomExecuteAsync for custom execution without node override
- added new graph scanning that will replace checksum/aot scanning in final version once Bound removed

#### Changed

- changed connection handling now changed to industry standard drag+drop
- changed AnimateWithPresetNode is made obsolete will be removed [BREAKING]
- changed AccumulationNode is no longer experimental
- changed windows/tools cleanup refactor
- changed GetParent expression function was put back

#### Fixed

- fix node creation now correctly doesn't offer creation of subgraph node inside the same graph

### Release 0.13.5 - 25.11.2022

#### Added

- added CreateGraphCopy to DashController to have an option to disable cloning graph asset instances (95% performance boost when copying not needed)
- added new expression editor with highlighting
- added expression debug so you can mark expressions for debug output

#### Changed

- changed primary support Unity version from 2019.3 to 2020.3 version
- changed expression UI to shortened version when out of focus
- changed minor optimizations to avoid graph initialization on OnEnable/Start when inputs are not enabled
- changed obsolete unreachable code cleanup

#### Fixed

- fixed null checks on retarget empty expressions
- fixed parametrization of RetargetToChildrenNode

### Release 0.13.4 - 11.11.2022

#### Added

- added support and serialization for prefab modifications on DashVariablesController
- added support for graph override in prefab

#### Changed

- changed DashController initialization now initializes on demand when using Send/Add methods not just on Awake
- changed DashControllerVariables direct reference type variables now support scene referencing
- changed removed support for ExposedReference variables [BREAKING]
- changed ExposedReferences in node properties now cache initial exposedName
- changed a lot of UI cleanup and changes for variables and node inspector
- changed custom expression fields now reset their previous value when switching from direct to expression or back
- changed unified color scheme and moved to Theme
- changed removed default SceneGUI as it was obsolete

#### Fixed

- fixed setting exposed reference to null not reset its values on all graph usages

### Release 0.13.3 - 27.10.2022

#### Added

- added DashSettingsWindow to have single settings window for various Dash settings
- added more UI for retarget nodes to show invalid/empty references
- added speed based tweens support for AnimateAnchoredPositionNode, AnimatePositionNode, AnimateScaleNode, AnimateSizeDeltaNode

#### Changed

- changed minor settings refactors


### Release 0.13.2 - 21.10.2022

#### Added

- added PlayAudioPreview for preview mode using IAudioManager

#### Changed

- changed linq optimizations on execution
- changed retarget expressions now show on node UI as actual expression

### Release 0.13.1 - 9.9.2022

#### Added

- added IAudioManager and Get/SetAudioManager on DashCore
- added PlayAudioNode to use AudioManager implementation
- added ability to define node subcategories
- added custom inspectors can now also force invalidation
- added custom inspectors can now customize group

#### Changed

#### Fixed

### Release 0.13.0 - 16.8.2022

#### Added

- added pooling to SpawnImageNode
- added warnings that bound graphs are going to be obsolete

#### Changed

- changed various legacy properties accross multiple nodes are being promoted to Parameters
- changed graphs now cannot be created as bound anymore

#### Fixed

- fixed now when parameter is switched to expression inspector shows all dependent properties as it cannot predict the value of dependency and user needs to be able to set dependent values

### Release 0.12.9 - 10.8.2022

#### Added

- added multiple documentation topics
- added DebugNode option to output to Unity console
- added various properties as Parameters this will be a continued effort to parametrize existing legacy properties

#### Changed

- changed various optimizations on property fetching
- changed to Documentation attribute to allow external documentation for custom nodes

#### Fixed

- fixed multiple UI/UX issues on editing asset graphs like disabling scene referencing etc.
- fixed null exceptions on uninitialized data in specific cases
- fixed UI issues for custom nodes

### Release 0.12.8 - 15.7.2022

#### Added

- Added select connected to select all nodes from this one down the output tree
- Added AnimateImageFillNode to animate fill of filled image type.

#### Fixed

- Fixed generics for exposed reference reflection

### Release 0.12.5 - 14.7.2022

#### Added

- Added support to retarget to exposed references

#### Changed

- Changed some iterations for better performance
- Changed checksums now use tuples to allow checking multiple graphs in single prefab

#### Fixed

- Fixed setting/removing start/enable input nodes through context menu
- Fixed debug override initialization, node execution is now debugged once per overriden node in debug window
- Fixed DashController inspector should be disabled correctly on any prefab instances

### Release 0.12.4 - 25.6.2022

#### Changed

- Changed dependency on DashTween to 0.1.4

#### Fixed

- Fixed correct once removal for callback listeners

### Release 0.12.3 - 22.6.2022

#### Fixed

- Fixed SerializationUtility ambiguity in latest Unity

### Release 0.12.2 - 15.6.2022

#### Fixed

- Fixed multiple Dependency attributes for properties now work correctly
- Fixed various property dependencies in Node inspector

### Release 0.12.1 - 14.6.2022

#### Fixed

- Fixed various warnings for node checksum workflow

### Release 0.12.0 - 13.6.2022

#### Added

- Added support for custom node category label
- Added new UI for Parameter properties to directly assign compatible variables
- Added node checksums as well as matching
- Added support for ExposedReference types in variables
- Added If function for expressions

#### Changed

- Changed custom expression classes are now defined implicitly by ExpressionFunctions attribute instead of adding them to config explicitly
- Changed expression editor no longer holds definitions of expression classes [BREAKING]
- Changed checksum timestamp/filenames are now readble date time
- Changed clear expression/value when changing paramter from expression to value and vice versa
- Changed node model properties without group/order attribute are sorted as if group/order was 0

#### Fixed

- Fixed scene references now not allowed for non exposed sypes

### Release 0.11.0 - 26.5.2022

#### Added

- Added arrange nodes functionality to arrange nodes down the stream from a specific node
- Added checksum window/scanner for checksum backups to solve/spot serialization issue 
- Added ability to create SubGraphNodes from asset graphs directly in node creation context menu

#### Changed

- Changed on new variable creation it is assigned default value instead of null
- Changed id and name generation to work with any namespace, needed for custom node creation outside of Dash namespace
- Changed minor node ui changes
- Changed various serialization/handling of subgraph nodes and their UI especially when switching from/to asset/bound graphs
- Changed ValidationWindow was removed as it was obsolete in last couple of versions

#### Fixed

- Fixed AnimateToTransform tween to Transform when not using RectTransform
- Fixed skipping enumeration of missing component scripts for bindables
- Fixed creating input/output node will automatically invalidate its name to unique names
- Fixed undo/redo performing and serialization on bound graphs

### Release 0.10.1 - 2.5.2022

#### Changed

- Changed NCalc is now in Dash.NCalc namespace to avoid global namespace conflict with other packages using NCalc

#### Fixed

- Fixed input/output indices serialization bug from 0.10.0
- Fixed Theme class not added as scanned AOT type, you can now specify classes explicitly by DashEditorOnly attribute
- Fixed when duplicating Input/Output nodes they automatically generate unique input/output names now
- Fixed when packing nodes to SubGraph it automatically generates input/output naming based on index

### Release 0.10.0 - 29.4.2022

#### Added

- Added AddListeners can now be added with once flag so they will be removed after first invocation
- Added support for 2021 Unity and their namespace changes
- Added new type of nodes called Connector which can be used for connection/graph management

#### Changed

- Changed increment property on IncrementText node to be parametrized
- Updated Odin Serializer to the latest version

#### Fixed

- Fixed subgraph node now gets correctly duplicated with its bound graph

### Release 0.9.0 - 29.3.2022

#### Added

- Added support to change controller graph at runtime

#### Changed

- Changed subgraph node UI
- A lot of various changes in subgraph implementation

#### Fixed

- Fixed subgraph output execution
- Fixed subgraph input execution in multi input cases
- Fixed node debugging trying to access already destroyed controller
- Fixed ForLoop node Stop implementation

### Release 0.8.0 - 25.2.2022

#### Added

- Added support for binding and unbinding of subgraphs
- Added support for nested subgraph serialization
- Added nested subgraph editing
- Added custom target option for DashController
- Added ability to pack selected nodes to subgraph
- Added ability to unpack subgraph into a current graph
- Added explicit type inflation for generics in AOT editor

#### Changed

- Changed node ids now don't render when you are zoomed out

#### Fixed

- Fixed DashEditorConfig now saved upon explicit or scanned AOT types change
- Fixed nodes don't select retarget object in editor if there is no controller
- Fixed nodes don't select retarget object if there is cyclic resolving involved
- Fixed Color field in node inspector overflow width incorrectly


### Release 0.7.1RC - 17.1.2022

#### Fixed

- Fixed SetImageNode execution
- Fixed SubGraph nullcheck on empty asset

### Release 0.7.0RC - 14.1.2022

#### Added

- Added documentation handling, attribute, linkage. UI...
- Added support for scene object referencing for variables on bound graphs in editor
- Added Range attribute support for numeric types as well as numeric Parameters
- Added UI for SubGraphNode to show used asset SubGraph where applicable
- Added option to store sprite to attribute for SetImageNode
- Added DashVariablesController to specify Controller as well as global variables
- Added IVariables interface can now be used to implement custom variable containers
- Added DashVariablesController supports field reflection binding to variables on start
- Added AnimateNode interface for direct editing and preview of TO/FROM values (PREVIEW)
- Added option to disable Dash logo in inspector

#### Changed

- Changed expression evaluation now explicitly casts return type to parameter type if both are numeric types
- Changed internal graph structures are now initialized before awake call
- Changed type cache now contains full type names for lookup among different namespaces to avoid collision
- Changed DashGlobalVariablesController was removed in favor of more robust DashVariablesController


#### Fixed

- Fixed SetImageNode to correctly end execution
- Fixed SubGraph open editor to correct bound controller
- Fixed ExposedReferences cloning when duplicating nodes (Regression bug since 0.6.0)
- Fixed execution error checking now correctly identifies UnityEngine.Object instances and tries to check against its overloaded null check as well

### Release 0.6.3RC - 17.12.2021

#### Added

- Added numeric types are now explicitly converted on expression evaluation so expression evaluating to double will be assignable to int etc.

#### Fixed

- Fixed AnimateToTransformNode now uses correct target transform

### Release 0.6.2RC - 13.12.2021

#### Added

- Added advancec inspector for DashController to view exposed reference tables.
- Added option to cleanup DashController and DashGraph exposed references by gathering the GUIDs.
- Added null checks for AnimateColorNode when update is called due to end user not killing tweens explicitly on destroyed object.
- Added option for KillOnNullEncounter on AnimateNodeBase to force tween kill on any null encounter.
- Added Obsolete attribute and UI for obsolete nodes handling.
- Added INodeMigratable interface to specify and implement migration of Obsolete nodes when necessary.
- Added AnimateToTransformNode
- Added SetAttributeNode that should be used instead of AddAttributeNode as more robust solution.
- Added updated documentation on Macros and Custom functions.

#### Changed

- Changed DashTween is not a separate package and used inside Dash as dependency. This way we can do updates on Tween library independently.
- Changed a lot of GitHub repository changes and separation of sourcecode bases Testbed, Dash, DashTween.

#### Fixed

- Fixed after 0.6.0 change graph is no longer marked ditry on all GUI updates.


### Release 0.6.1RC - 01.12.2021

#### Added

- Added AnimatePositionNode
- Added SetVariable method to make add/modify on variables easier
- Added node input chain and retarget resolving for further tooling [EXPERIMENTAL]
- Added macro support in expression evaluation
- Added macro editing support to Expression editor window
- Added ability to override custom context menus for node implementations
- Added new custom UIs to work with AnimationNodes retargeting and from/to binding [EXPERIMENTAL]
- Added UseNativeSize, IsMaskable and IsRaycastTarget to SpawnImageNode to modify default state
- Added StoreToAttribute on AnimateColorNode now works for CANVASGROUP type as well

#### Changed

- Changed minor changes to GUI to unify inspector layouts (widths,spacing etc.)
- Changed GenericMenu usage was completely reworked to use RuntimeGenericMenu in various aspects of the editor
- Changed initial update for tween now called upon start [WARNING]

#### Fixed

- Fixed correct references asssigning/visualisation in editor due to different Controller injection introduced in 0.6.0
- Fixed removed unused properties to fix editor warnings
- Fixed custom implicit casting where source/target were reversed in number types


### Release 0.6.0RC - 16.11.2021

#### Added

- Added if Parameter is of a string type and returned expression evaluation is not implicitly assignalbe to it it is converted using ToString() explicitly
- Added support for custom expression function through custop user defined classes using DashExpressionsEditorWindow [PREVIEW]

#### Changed

- Changed Dash editor windows now all have minimal size to ensure valid UI states
- Changed removed text/variable from DebugNode now everything can be debugged using parametrized debug property [BREAKING]
- Changed removed graph version migration and warnings, should be on lower level
- Changed selection logic now separated from DashEditorCore to SelectionManager
- Changed theme logic now separated from DashEditorCore using a scriptable Theme asset instead
- Changed classes folder structure to match what is editor related even though it is in Runtime assembly
- Changed how Controller injection works during Preview, removed IEditorGraphAccess [POSSIBLE BREAK]
- Changed DashCore is now DashRuntimeCore
- Changed updated OdinSerializer to 2021.8.11
- Changed removed IconAttribute as it is obsolete now and was ambiguous to the Unity 2021 new IconAttribute as well

#### Fixed

- Fixed type enumeration reworked to private member as it seems Unity changed API for GenericMenu internally
- Fixed DashTween was always expecting OnCokmplete to be defined and called now it is optional
- Fixed null error on Variables inspector when no Graph attached on Controller

### Release 0.5.4RC3 - 4.10.2021

#### Added

- Added Graph variable inspector to Controller inspector as well
- Added priority for event listeners
- Added expression support to DebugNode
- Added EndEventNode to end sequences instead of using custom SendEventNode
- Added additive node selecting using shift
- Added asynchronity indicator for nodes
- Added more UI infor for AccumulateNode
- Added Store/RestoreNode to store/restore the state of transform hierarchy (PREVIEW)
- Added UI to support Graph box resizing
- Added hangling of cyclic node referencing
- Added expression function RandomF can now be called without parameters as well to generate random 0-1 value
- Added visuals for Start/OnEnable Input nodes
- Added Set/RemoveAsStartInput Set/RemoveAsOnEnableInput node context menu for Input nodes
- Added reconnect ui and functionality
- Added expression reference parameter resolving
- Added inspector support for Color variables
- Added expression functions for Color creation
- Added event sequencers
- Added multiple new debug events for DashDebugConsole (controller, dash, sequencer...)

#### Changed

- Changed refactored Graph stopping to node interfaces
- Changed non supported variable types no longer fire Debug warnings just show up in editor
- Changed implementation of copy pasting of property referencing to more intuitive UI
- Changed just started tweens now fire update with from position in initial frame
- Changed controllers list now shallow cloned to avoid iteration errors on controllers change as a result of event action
- Changed local retargeting UI visuals
- Changed removed arbitrary SetListener method for nodes, uses callback methods internally now
- Changed refactored DashDebugConsole to handle internal events using wrapper classes

#### Fixed

- Fixed CallMethod AOT assembly lookup changes
- Fixed null checks in json import/export
- Fixed correctly removing tweens from active stack on completion on multiple nodes
- Fixed expression reference lookups

### Release 0.5.3RC3 - 19.08.2021

#### Added
- Added DebugOverride attribute for custom nodes to override execution debug
- Added RandomOnCircle and RandomInsideCircle expression functions
- Added prefab editor window for later ID pooling (PREVIEW)
- Added further documentation on expression functions and parameters
- Added pooling system into the Core
- Added support for pooling for SpawnUIPrefabNode and DestroyNode

#### Changed
- Changed when parameter is null it is threated as a node execution error, returns default value.
- Changed local retargeting is now the Target information in DashDebugConsole instead of NodeFlowData target parameter

#### Fixed
- Fixed RetargetToChildrenNode and ForLoopNode correctly assings and destroys tweens on Graph Stop
- Fixed large time steps in editor preview causing infinity loops in OnComplete->Start tween chains.

### Release 0.5.2RC3 - 27.07.2021

#### Added
- Added warning log when initializing graph of older version at runtime
- Added ERROR messages in Dash debugger window
- Added runtime caching of expression functions reflection lookup
- Added ability to define custom Expression function classes for evaluation

#### Changed
- Changed removed generic expression functions to avoid AOT issues
- Changed cleanup of various serialized properties to minify serialization
- Changed error handling in node execution and parameter evaluation to first encountered instead of last encountered
- Changed more distinctive UI colors/background when in preview mode
- Changed Dash debug window now orders items from top/old to bottom/new like standard console
- Changed split config into Runtime and Editor as API changes that need runtime configuration are being introduced

#### Fixed
- Fixed build time bug in compilation flags

### Release 0.5.0RC3 - 24.06.2021

#### Added

- Added support for custom inspector implementations for NodeModels
- Added SetMaskableNode
- Added CallMethodNode for calling methods on any Component on DashController's GameObject
- Added inspector for Global Variables in Graph Editor
- Added GetCanvas expression function
- Added Canvas to supported types
- Added Store to Attribute option for all animation nodes (to store the previous value)

#### Changed

- Changed coloring for Parametrized properties in inspector for better visual feedback
- Changed zooming in/out now depends on mouse cursor position
- Changed various properties ordering within node models
- Changed refactored property groups
- Changed DashGlobalVariables now explicitly state that they don't support Prefab Overrides at it ended up in runtime issues  
- [POTENTIONALLY BREAKING] changed serialization approach to automatically handle refactoring updates

#### Fixed



### Release 0.4.8RC2 - 15.06.2021

#### Added

- Added SoundNode for playing sounds from graphs
- Added AccumulatedNode to manage execution by accumulating various inputs  
- Added SetActiveNode UI in editor now shows activation state.

#### Changed

- Changed preview mode now offsets execution to next frame to avoid "jump" or missed time

#### Fixed

- Fixed AnimateTextNode now has correct execution time
- Fixed preview now correctly dirties Graph and Controller
- Fixed SetVariableNode now avoids execution if the expression is empty
- Fixed dragging nodes and boxes in prefab now sets the graph as dirty
- Fixed actiovation/deactivation of node connection now sets the graph as dirty
- Fixed marking node as preview now sets the graph as dirty
- Fixed deleting connection now sets the graph as dirty
- Fixed renaming box now sets the graph as dirty
- Fixed correct mouse check for connections to other than first input

### Release 0.4.7RC2 - 03.06.2021

#### Added

- Added an option to use unscaled time inside Dash
- Added CANVASGROUP alpha to AnimateColorNode
- Added useFrom and From parameters for all types of AnimateColorNode

#### Changed

- Changed now cloning listeners list during event dispatch so removing a listener on dispatch will not cause iteration error
- Changed Previewing in Prefab editor now completely reworked in no longer saves/loads scene but tries to save load prefab stage content
- Changed when switching between scene and prefab editor the editing graph is reset to avoid editing graph cross scene/prefab stage.

#### Fixed

- Fixed various minor fixes in UI

### Release 0.4.6RC2 - 28.05.2021

#### Added

- Added dot resolving for variable properties so it is possible to resolve something like this [target.sizeDelta.x] in parametrized properties

#### Changed

- [BREAKING] Changed attributeType to correct variableType name in SetVariableType node, you will need to recreate all your SetVariable nodes unfortunately. The autofix for such changes is planned in future releases.

#### Fixed

- Fixed correct variable binding to properties.
- Fixed AOT scanner error when scanned DashController doesn't have any graphs.
- Fixed when creating using SetVariableNode it correctly creates variable of specified type instead of Object.

### Release 0.4.5RC2 - 24.05.2021

#### Added

- Added option to execute specified Graph input on OnEnable of DashController gameobject

#### Changed

- Various resizes of inspector and popup GUIs

#### Fixed

- Fixed error when selected node list could be null after undo
- Fixed controller error in debug when destroyed previously

### Release 0.4.4RC2 - 17.05.2021

#### Added

- Added AOT editor, generator and project scanning for Dash referenced types to preserve them on AOT platforms

### Release 0.4.3RC2 - 11.05.2021

#### Added

- Added OnDestroy on DashController now stops underlying graph
- Added Stop method on DashGraph that stops all running animations within graph and all subgraphs.
- Added Unbind method and Unbind controller on OnDestroy.

#### Changed

- Changed Moved DashEditorConfig to Assets/Resources/Editor folder to avoid packaging it into build. This will create a new config!
- Changed: Global events are sent as local while in Preview mode.

### Release 0.4.2RC2 - 05.05.2021

#### Added

- Added SetListener method that removes all current listeners and sets a new one
- Added Initial commit of experimental debug console window.

#### Changed

- Changed version info moved to Runtime assembly
- Changed Advanced context creation menu is now the default one.

#### Fixed

- Fixed dragging view is now decoupled and the only mouse interaction that should work during preview or play mode.
- Fixed various editor references in runtime assembly.
- Fixed migration warning should now show the correct version format.
- Fixed last connection now correctly gets saved when editing in Prefab

### Release 0.4.1RC2 - 03.05.2021

#### Added

- Added multiple changes added to AnimateTextNode like parametrization etc.

#### Fixed

- Fixed SINE_INOUT easing now behaves correctly
- Fixed delayed tweens should no longer call update before the delay runs out with negative values
- Fixed easing is now correctly reset on tween pooling so next tweens that didn't set easing explicitly will have correct LINEAR value
- Fixed right click now gets correctly consumed when over model insepctor
- Fixed multiple transform checks through framework to handle destroyed gameobject transforms with correct Unity null check
- Fixed opening new graph created inside assetbrowser will no longer incorrectly show migration warning

### Release 0.4.0RC2 - 28.04.2021

#### Added

- Added new installation option using Unity Package Manager viz README
- Added initial support for Graph versioning and migration validation between versions
- Added support for new type of lookup variables so instead of binding to gameobject/component they lookup in hierarchy on runtime
- Added support for COPY/PASTE of variables
- Added support for retargeting expressions to take any component/gameobject and retarget to its transform instead of need for direct reference
- Added Global event listeners on DashCore
- Added support for COPY/PASTE of nodes between graphs
- Added new supported variable types (Bool, String, Vector4, Quaternion, Transform, RectTransform)
- Added StopAnimationsNode to stop all animations within graph or a particular target
- Added String() expression function to convert variable/attribute to string to avoid casting issues (for example when setting text using indices) 
- Added more EXPERIMENTAL SubGraphNode functionality (UI, logic, handling...)

#### Changed

- Removed DoTween from the framework **[BREAKING]** (all easings on existing animation nodes will be LINEAR after migration)
- All AnimationNodes now inherit AnimationBaseNode
- All AnimationNodes were refactored to enable tween/animation lookup
- AnimateClipNode properties are now parametrized
- whitespace are now no longer removed from expressions (for string usage)
- FlowData parameter is now mandatory on evaluation
- DashController auto start disabled by default
- usage of SetParent in SpawnNode to avoid UI scaling issues upon spawning.

#### Fixed

- fixed incorrectly rendered variables UI when bound
- fixed no longer settings DashController dirty when there was no change detected
- fixed GUI for minimizing/maximizing property groups in model inspector
- fixed correct component names in binding context menu
- fixed bound graph now correctly set to dirty after serialization validation
- fixed right mouse button drag now works in play mode
- fixed all catched exceptions now correctly sent to debug output when there is error in node execution
- fixed crash when cloning nodes with exposed references to Asset graphs

### Release 0.3.0RC - 16.04.2021

#### Fixed
- Mainly preparations for RC release

## BETA RELEASES

### Release 0.2.9b - 09.04.2021

#### Fixed
- fixed issue with cloned nodes referencing same parameter instance
- fixed issue with invalid inspector of new DashController
- fixed issue with callback listeners not working when there zero node listeners

#### Changed
- multiple node methods to create, delete, clone nodes were moved to Editor assembly

#### Added
- added DashGlobalVariables for global variables support 

### Release 0.2.8b - 03.04.2021

#### Fixed
- duplication of nodes with exposed references now create a copy of exposed reference instead of using the same incorrectly
- rotation parameter in AnimateRotationNode now can accept negative values and rotates correctly
- when there is no preview node the previewer is disabled

#### Changed
- easing property in all animation nodes is now Parameter **BREAKING NEEDS RESERIALIZATION**

#### Added

### Release 0.2.7b - 22.03.2021

#### Fixed
- fixed color property dependency in AnimateToColorNode
- fixed correct addIndexAttribute check in ForLoopNode

#### Changed
- visual feedback on editing controller in graph view
- cleanup and refactoring base classes
- minor properties renaming
- minor refactoring during documentation writeup
- added dependencies on various properties
- restructured documentation

#### Added
- complete Node documentation
- added option for code based listeners using actions

### Release 0.2.6b - 15.03.2021

#### Fixed
- fixed tab key correctly working and not forced to used event in overlay views

#### Changed
- refactored ui utils mainly due to referencing
- removed paid graphics assets
- changed OnMouseClickNode to OnButtonClickNode correctly
- changed priority of NodeFlowData resolving to highest

#### Added
- added referencing for Expression fields
- added referencing for ExposedReference fields
- added new example scene with free and selfcreated assets
- added general mouse click node

### Release 0.2.5b - 08.03.2021

#### Fixed
- fixed mouse blocking for variables and inspector views

#### Changed
- added additional focus management, clicking in graph will defocus fields etc.
- [BREAKING] graph variables no longer use g_ prefix, so all old g_ stuff will stop working

#### Added
- added an UI to search for nodes in editor
- additional expression functions
- new property referencing through graph within expressions, first introduced in Machina but now reworked with event better implementation in Dash that also handles nested reflection
- execution count for debugging is now nested (for subgraph solving)
- refactored execution counting and interfaces, now gathered per update (small performance hit but not crucial as this is only for editor time debugging)
- added functionality and context menu for reference copy/pasting

### Release 0.2.4b - 01.03.2021

#### Fixed
- fixed leaking error state to next parameter resolving
- fixed correct invalidation of node on any inspector values change
- fixed unique ID validation
- fix when no node is selected focus breaks for graph variable view

#### Changed
- removed Enter/Exit node as they are obsolete
- renamed and restructuralized node categories
- detailed text on nodes and custom gui is now invisible when zoomed out

#### Added
- started adding tooltips/help to nodes
- implemented InputNode type
- first rollout of SubGraphs HIGHLY EXPERIMENTAL (new paradigms needed)
- added a lot of UI stuff for sub graph handling
- added UI to go directly to animation preset code
- new implementation for Create Node menu through popup with search and tooltips
- delete nodes shortcut


### Release 0.2.3b - 22.02.2021

#### Fixed
- fixed correct output mouse over highlighting when zoomed
- fixed issue with multiple controllers tagged as preview during scene reload
- fixed issues with new nodes disappearing on assembly reload

#### Changed
- property naming in model inspector is now Nicified same as Unity aka toPosition is To Position
- changed implementation for Extracted/DashAnimations to Asset workflow
- cleaned up serialization data to avoid serialization of unnecessary data also reimplemented things like groupMinimized from Dictionary to integer bitmask
- Undo reimplemented as RecordObject got slow on larger graphs
- ExecutionEnd will now not be fired when there is errorInExecution this helps in resolving errors
- small GUI, color and name changes
- refactored and split access Interfaces

#### Added
- shortcuts in editor window (Duplicate)
- Node comments
- GraphBoxes similar to UE4 where you can group nodes into boxes for better visualisation and workflow
- added visualisation which nodes are going to be selected through region selection
- added option for property group ordering
- added option to use AnimationClip as well as DashAnimation inside AnimateWithClipNode
- ability to drag view with Right button as well, Alt+Drag is still functional for now as well
- propagation of Expression/Resolve errors to node level for better debugging.
- added GUI for help information when graph has no nodes 
- first documentation draft for expression functions


### Release 0.2.2b - 15.02.2021

#### Fixed
- execution visualization now correctly visualizes executed connections
- fixed parenting bound graph execution/cloning
- all retargeting now correctly works even with null target attribute on input if not retargeting to child

#### Changed
- setSpawnedAsTarget renamed to retargetToSpawned on SpawnImageNode
- SpawnImageNode positions on anchoredPosition now
- name of graph is now ControllerName[Bound] instead of Bound
- experimental nodes are now in their correct category just marked as experimental instead of having separate category
- various cleanups and small UI changes
- retargeting useReference set to false by default
- expression evaluation reimplementation
- OnMouseClickNode now can bind button using string path instead of reference, also advanced find option
- OnMouseClickNode retargeting to button is optional
- SendEventNode has no outputs now it is an end node
- switched to a different NCalc implementation
- removed external Antlr dependency
- updated README 3rd Party.
- executeOnNull property removed, most cases ended up in even more critical condition and confusing situations
- removed AddIntVariableNode it was obsolete due to the new more generic AddAttributeNode
- expression evaluation for toProperty on animation nodes is now done on start and not per update
- changes to error handling and expression casting through the framework

#### Added
- SpawnUIPrefabNode
- NullNode
- added an option to delete multiple nodes on selection.
- added duplication of selected Node/Nodes implementation.
- experimental nodes now have icon indicator
- previewing on any node, you can now select a node to preview, Enter node is a default one
- add generic AddAttributeNode
- added more verbose warning messages on encountering null
- Instant node preview
- RetargetAdvancedNode can retarget using Find even to multiple targets at once.
- added an option to all retargeting using find instead of just path.
- added ton expression functions to complete functionality also merged functions from Machina.
- added implementation for generic wrapper types Parametrization


### Release 0.2.1b - 03.02.2021

#### Changed
- a lot of changes across codebase, merged latest Machina changes and naming

### 0.2.0b - 02.02.2021

#### Added
- initial commit
