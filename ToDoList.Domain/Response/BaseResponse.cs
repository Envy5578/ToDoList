﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Enum;

namespace ToDoList.Domain.Response
{
    public class BaseResponse<T> : IBaseResponse<T>
    {
        public string Description { get; set; }
        public StatusCode StatusCode {  get; set; }
        public T Data { get; set; }
    }

    public interface IBaseResponse<T>
    {
        string Description { get; set; }
        StatusCode StatusCode { get; }
        T Data { get; }
    }
}
