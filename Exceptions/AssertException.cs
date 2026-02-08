using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LeadTurbo.Exceptions
{
    /// <summary>
    /// 断言失败异常
    /// </summary>
    public class AssertException : Exception
    {
        string message = null;


        public AssertException()
        {
            message = base.Message;
        }


        /// <summary>
        /// 使用异常消息构建异常 
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string message,
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine(message);
            sb1.AppendLine(sourceFilePath);
            sb1.AppendLine(memberName);
            sb1.AppendLine(string.Format("{0}",sourceLineNumber));
            this.message = sb1.ToString();
            Debug.Write(message);

        }

       

        /// <summary>
        /// 使用异常消息构建异常 
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string formoat, object a, 
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine(string.Format(formoat, a));
            sb1.AppendLine(sourceFilePath);
            sb1.AppendLine(memberName);
            sb1.AppendLine(string.Format("{0}", sourceLineNumber));
            this.message = sb1.ToString();
            
            Debug.Write(message);
        }

       

        /// <summary>
        /// 使用异常消息构建异常 
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string formoat, object a, object b, 
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine(string.Format(formoat, a, b));
            sb1.AppendLine(sourceFilePath);
            sb1.AppendLine(memberName);
            sb1.AppendLine(string.Format("{0}", sourceLineNumber));
            this.message = sb1.ToString();
            
            Debug.Write(message);
        }

        /// <summary>
        /// 使用异常消息构建异常 
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string formoat, object a, object b, object c, 
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine(string.Format(formoat, a, b, c));
            sb1.AppendLine(sourceFilePath);
            sb1.AppendLine(memberName);
            sb1.AppendLine(string.Format("{0}", sourceLineNumber));
            this.message = sb1.ToString();
            
            Debug.Write(message);
        }

        /// <summary>
        /// 使用异常消息构建异常 
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string formoat, object a, object b, object c, object d, 
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine(string.Format(formoat, a, b, c, d));
            sb1.AppendLine(sourceFilePath);
            sb1.AppendLine(memberName);
            sb1.AppendLine(string.Format("{0}", sourceLineNumber));
            this.message = sb1.ToString();
            
            //Debug.Write(message);
        }

        /// <summary>
        /// 使用异常消息构建异常 
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string formoat, object a, object b, object c, object d, object e, 
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine(string.Format(formoat, a, b, c, d, e));
            sb1.AppendLine(sourceFilePath);
            sb1.AppendLine(memberName);
            sb1.AppendLine(string.Format("{0}", sourceLineNumber));
            this.message = sb1.ToString();
            
            Debug.Write(message);
        }

        /// <summary>
        /// 使用异常消息构建异常 
        /// </summary>
        /// <param name="message">异常消息</param>
        public AssertException(string formoat, object a, object b, object c, object d, object e, object f, 
            [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0,
            [System.Runtime.CompilerServices.CallerMemberName] string memberName = "",
            [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "")
        {
            StringBuilder sb1 = new StringBuilder();
            sb1.AppendLine(string.Format(formoat, a,b,c,d,e,f));
            sb1.AppendLine(sourceFilePath);
            sb1.AppendLine(memberName);
            sb1.AppendLine(string.Format("{0}", sourceLineNumber));
            this.message = sb1.ToString();
            
            Debug.Write(message);
        }


        public override string Message
        {
            get
            {
                return this.message;
            }
        }



    }
}
