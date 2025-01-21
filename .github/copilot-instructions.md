System Goal:
An abstract ecosystem in Unity where resources move and transform between different transformers. The number of resources equals the number of transformers, so each transformer starts with one resource.
Main Components:

EcosystemController
Resource
Transformer

Workflow:

Each transformer starts with a matching resource
Resources are transformed (e.g., A→B→C→A)
After transformation, the resource searches for the next matching transformer
If the target transformer is busy, the resource joins the queue
The EcosystemController selects the fastest available matching transformer

This system should enable a continuous, self-regulating cycle of resource transformation, where resources are intelligently distributed between transformers.
