using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GanoExcel.Web.Base
{
    [Serializable]
    public class BaseException : Exception 
    {
        public BaseException() :
            base()
        {
        }

        public BaseException(string message) :
            base(message)
        {
        }

        public BaseException(string message, Exception innerException) :
            base(message, innerException)
        {
        }      
    }

    [Serializable]
    public class InvalidOrBlankConnectionStringException : BaseException
    {
        public InvalidOrBlankConnectionStringException() :
            base()
        {
        }

        public InvalidOrBlankConnectionStringException(string message) :
            base(message)
        {
        }

        public InvalidOrBlankConnectionStringException(string message, Exception innerException) :
            base(message, innerException)
        {
        }
    } 

}
