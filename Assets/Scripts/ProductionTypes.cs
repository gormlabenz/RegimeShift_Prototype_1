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

    public static ResourceType GetInputType(TransformerType transformer)
    {
        return transformer switch
        {
            TransformerType.AB => ResourceType.A,
            TransformerType.BC => ResourceType.B,
            TransformerType.CA => ResourceType.C,
            _ => throw new ArgumentException("Unknown transformer type")
        };
    }

    public static ResourceType GetOutputType(TransformerType transformer)
    {
        return transformer switch
        {
            TransformerType.AB => ResourceType.B,
            TransformerType.BC => ResourceType.C,
            TransformerType.CA => ResourceType.A,
            _ => throw new ArgumentException("Unknown transformer type")
        };
    }

    public static ResourceType GetNextType(ResourceType current)
    {
        return current switch
        {
            ResourceType.A => ResourceType.B,
            ResourceType.B => ResourceType.C,
            ResourceType.C => ResourceType.A,
            _ => throw new ArgumentException("Unknown resource type")
        };
    }

    public static ResourceType GetPreviousType(ResourceType current)
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