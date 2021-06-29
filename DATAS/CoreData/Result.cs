using System;
using Newtonsoft.Json;

namespace CoreData
{
    public interface IResult<T>
    {
        T Data { get; set; }
        string Error { get; set; }
        bool Success { get; set; }
    }

    public class Result<T> : IResult<T>
    {
        [JsonIgnore]
        public static Result<T> Failed => Fail();
        [JsonIgnore]
        public static Result<bool> Successed => Result<bool>.Ok(true);
        [JsonProperty("data")]
        public T Data { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }
        [JsonProperty("success")]
        public bool Success { get; set; }

        public static Result<T> Ok(T data)
        {
            return new()
            {
                Data = data,
                Success = true
            };
        }

        public static Result<T> Fail(Exception exception)
        {
            return Fail(exception.Message);
        }

        public static Result<T2> From<T2>(T2 d) where T2 : class
        {
            return d != null ? Result<T2>.Ok(d) : Result<T2>.Failed;
        }

        public static Result<T> Fail(string error = "")
        {
            return new()
            {
                Data = default,
                Success = false,
                Error = error
            };
        }

        public static Result<bool> From(bool b)
        {
            return b ? Result<bool>.Successed : Result<bool>.Failed;
        }
    }
}