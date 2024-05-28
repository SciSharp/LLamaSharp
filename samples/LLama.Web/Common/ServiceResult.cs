namespace LLama.Web.Common
{
    public class ServiceResult<T> : ServiceResult, IServiceResult<T>
    {
        public T Value { get; set; }
    }


    public class ServiceResult
    {
        public string Error { get; set; }

        public bool HasError
        {
            get { return !string.IsNullOrEmpty(Error); }
        }

        public static IServiceResult<T> FromValue<T>(T value)
        {
            return new ServiceResult<T>
            {
                Value = value,
            };
        }

        public static IServiceResult<T> FromError<T>(string error)
        {
            return new ServiceResult<T>
            {
                Error = error,
            };
        }
    }

    public interface IServiceResult<T>
    {
        T Value { get; set; }
        string Error { get; set; }
        bool HasError { get; }
    }
}
