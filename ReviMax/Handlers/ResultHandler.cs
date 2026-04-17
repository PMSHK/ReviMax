using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReviMax.Core.Config;

namespace ReviMax.Handlers
{
    internal class ResultHandler<T>
    {
        public bool IsSuccess { get; } = false;
        public T Value { get; } = default;
        public string ErrorMessage { get; } = string.Empty;

        public ResultHandler(T value, bool isSuccess, string errorMessage)
        {
            this.IsSuccess = isSuccess;
            this.Value = value;
            this.ErrorMessage = errorMessage;
        }
        public static ResultHandler<T> Success(T value) => new ResultHandler<T>(value, true, string.Empty);
        public static ResultHandler<T> Failure(string errorMessage)
        {
            ReviMaxLog.Error(errorMessage);
            return new ResultHandler<T>(default, false, errorMessage);
        }
    }
}
