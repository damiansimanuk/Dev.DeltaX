namespace DeltaX.JsonRpc.Exceptions
{
    public class RpcExceptionInvalidRequest : RpcException
    {
        public RpcExceptionInvalidRequest() : base(-32600, "Invalid Request", null) { }
    }
}
