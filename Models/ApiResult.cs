using ApiResult.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiResult.Models
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class ApiResult<T> where T : class
    {
        public T? Data { get; set; }
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public EErrorType ErrorType { get; set; }

        private void SetError(string? message, EErrorType errorType)
        {
            IsSuccess = false;
            Message = message;
            ErrorType = errorType;
        }

        private void SetSuccess(T data)
        {
            Data = data;
            IsSuccess = true;
        }

        public virtual async Task<ApiResult<T>> ExecuteAsync(Func<Task<T>> func, params Func<T, ValidationResult>[] validations)
        {
            try
            {
                T data = await func();
                foreach (var validation in validations)
                {
                    var result = validation(data);
                    if (!result.IsValid)
                    {
                        SetError(result.ErrorMessage, EErrorType.ValidationFailed);
                        return this;
                    }
                }
                SetSuccess(data);
            }
            catch (Exception ex)
            {
                SetError(ex.Message, EErrorType.InternalServerError);
            }
            return this;
        }

        public virtual async Task<ApiResult<T>> ExecuteAsync(Func<Task<T>> func, Func<T, bool>? validation = null)
        {
            try
            {
                var data = await func();
                if (validation != null)
                {
                    if (validation(data))
                        SetSuccess(data);
                    else
                        SetError("Validation failed", EErrorType.BadRequest);
                }
                else
                    SetSuccess(data);
            }
            catch (Exception ex)
            {
                SetError(ex.Message, EErrorType.InternalServerError);
            }
            return this;
        }

        public virtual async Task<ApiResult<T>> ExecuteAsync(Func<Task<T>> func, Func<T, Task<bool>>? validation = null)
        {
            try
            {
                var data = await func();
                if (validation != null)
                {
                    if (await validation(data))
                        SetSuccess(data);
                    else
                        SetError("Validation failed", EErrorType.BadRequest);
                }
                else
                    SetSuccess(data);
            }
            catch (Exception ex)
            {
                SetError(ex.Message, EErrorType.InternalServerError);
            }
            return this;
        }

        public virtual ApiResult<T> OnSucces(Action<T> action)
        {
            if (IsSuccess)
            {
                action(Data);
            }
            return this;
        }

        public virtual ApiResult<T> OnError(Action<string, EErrorType> action)
        {
            if (!IsSuccess)
            {
                action(Message, ErrorType);
            }
            return this;
        }
    }
}
