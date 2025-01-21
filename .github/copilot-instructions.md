System Objective:
An abstract ecosystem in Unity where resources move and transform between different transformers. The number of resources equals the number of transformers, so each transformer starts with one resource.

Main Components:

EcosystemController:

- Central management system
- Manages resource allocation to transformers
- Initializes the system with a 1:1 mapping of resources to transformers

Resource:

- Has three main states: Moving, Waiting, Transforming, Unassigned
- Moves independently between transformers
- Starts with the status matching the first transformer
- Can wait in a queue if the target transformer is busy

Transformer:

- Defines its type (A, B, or C)
- Only processes one resource at a time
- Has a defined processing time
- Transforms resources from one state to the next

Workflow:

- Each transformer starts with a matching resource
- Resources are transformed (e.g., A→B→C→A)
- After transformation, the resource searches for the next matching transformer
- If the target transformer is busy, the resource joins the queue
- The EcosystemController selects the fastest available matching transformer

Special Features:

- Calculation of expected completion time for optimal transformer selection
- Resources maintain their assigned target until they reach it
- Clear status transitions and management for resources

This system should enable a continuous, self-regulating cycle of resource transformation, where resources are intelligently distributed between transformers.
