using Annium.Components.State.Operations.Internal;

namespace Annium.Components.State.Operations;

public static class OperationState
{
    public static IOperationState New() => new Internal.OperationState();

    public static IOperationState<T> New<T>() => new OperationState<T>();
}
