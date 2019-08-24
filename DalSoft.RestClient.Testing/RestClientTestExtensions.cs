using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DalSoft.RestClient.Testing
{
    public static class RestClientTestExtensions
    {
        public static Task<dynamic> Verify<TResponse>(this Task<dynamic> request, Expression<Func<TResponse, bool>> verify) where TResponse : class
        {
            return request
                .ContinueWith(task =>
                {
                    var response = typeof(TResponse) == typeof(string) ? task.Result?.ToString() : (TResponse)task.Result;

                    if (verify != null && response != null && verify?.Compile()(response) == true)
                        return task.Result;

                    var message = verify?.ToString() ?? "null";
                    throw new VerifiedFailed($"{Environment.NewLine}{message} was not verified");
                });
        }

        public static Task<dynamic> Verify(this Task<dynamic> request, Func<dynamic, bool> verify)
        {
            return request
                .ContinueWith(task =>
                {
                    var response = task.Result;

                    if (verify != null && response != null && verify(response) == true)
                        return task.Result;

                    var message = verify?.ToString() ?? "null";
                    throw new VerifiedFailed($"{Environment.NewLine}{message} was not verified");
                });
        }
    }
}