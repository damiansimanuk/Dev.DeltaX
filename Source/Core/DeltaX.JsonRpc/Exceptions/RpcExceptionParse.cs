namespace DeltaX.JsonRpc.Exceptions
{
    public class RpcExceptionParse : RpcException
    {
        public RpcExceptionParse() : base(-32700, "Parse error", null) { }
    }
}
