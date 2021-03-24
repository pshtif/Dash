# CHANGE LOG

All notable changes to this project will be documented in this file.

## BETA RELEASES

### Release 0.2.7b - 22.03.2021

#### Fixed
- fixed correct addIndexAttribute check in ForLoopNode

#### Changed
- cleanup and refactoring base classes
- minor properties renaming
- minor refactoring during documentation writeup
- added dependencies on various properties
- restructured documentation

#### Added
- complete Node documentation

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
