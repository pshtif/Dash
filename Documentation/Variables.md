# VARIABLES

In Dash variables are used to specify values that can be later evaluated in nodes directly or within an parameter expression. In Dash there are several ways how to define variables and they differ in implementation and usage.

## Graph Variables

Graph variables are variables bound and serialized on the specific DashGraph which means they are limited into the context of single graph and other limitations like option to reference scene objects.

*Since bound graphs are bing made OBSOLETE referencing of scene objects from any graph variables will be impossible as they are stored/serialized to asset.*
