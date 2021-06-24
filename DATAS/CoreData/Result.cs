using System;

namespace CoreData
{
    public interface IResult<T>
    {
        T Data { get; set; }
        string Error { get; set; }
        bool Success { get; set; }
    }
    public class Result<T>: IResult<T>
    {
        public T Data { get; set; }
        public string Error { get; set; }
        public bool Success { get; set; }
        public static Result<T> Failed => Fail();
        public static Result<bool> Successed => Result<bool>.Ok(true);

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

        public static Result<T> Fail(string error="")
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