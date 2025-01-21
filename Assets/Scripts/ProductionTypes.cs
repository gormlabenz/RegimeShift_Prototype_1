using System;

public static class ProductionTypes
{
    public enum ResourceType
    {
        A,
        B,
        C
    }

    public enum TransformerType
    {
        AB,
        BC,
        CA
    }

    public static ResourceType GetTransformerInputType(TransformerType transformer)
    {
        return transformer switch
        {
            TransformerType.AB => ResourceType.A,
            TransformerType.BC => ResourceType.B,
            TransformerType.CA => ResourceType.C,
            _ => throw new ArgumentException("Unknown transformer type")
        };
    }

    public static ResourceType GetTransformerOutputType(TransformerType transformer)
    {
        return transformer switch
        {
            TransformerType.AB => ResourceType.B,
            TransformerType.BC => ResourceType.C,
            TransformerType.CA => ResourceType.A,
            _ => throw new ArgumentException("Unknown transformer type")
        };
    }

    public static ResourceType GetNextResourceType(ResourceType current)
    {
        return current switch
        {
            ResourceType.A => ResourceType.B,
            ResourceType.B => ResourceType.C,
            ResourceType.C => ResourceType.A,
            _ => throw new ArgumentException("Unknown resource type")
        };
    }

    public static ResourceType GetPreviousResourceType(ResourceType current)
    {
        return current switch
        {
            ResourceType.A => ResourceType.C,
            ResourceType.B => ResourceType.A,
            ResourceType.C => ResourceType.B,
            _ => throw new ArgumentException("Unknown resource type")
        };
    }
}