# CHANGE LOG

All notable changes to this project will be documented in this file.

## RELEASE CANDIDATES

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
