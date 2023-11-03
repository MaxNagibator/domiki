namespace Domiki.Web
{
    public class Response<T> : Response
    {
        public T Content { get; set; }
    }

    public class Response
    {
        public ResponseType Type { get; set; }
    }

    public enum ResponseType
    {
        Success = 1,
        ErrorMessage = 2,
    }
}
