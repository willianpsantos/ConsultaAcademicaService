using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace ConsultaAcademica.Core
{
    public class MoodleResponseException : Exception
    {
        public MoodleError MoodleErrorData { get; set; }

        public string RawMoodleError { get; set; }

        public Exception MoodleErrorConvertException { get; set; }
        
        public MoodleResponseException(string xmlOrJson) : base()
        {
            RawMoodleError = xmlOrJson;
            MoodleErrorData = GetMoodleError(xmlOrJson);
        }

        private MoodleError GetMoodleErrorByXml(string xml)
        {
            var moodlerError = new MoodleError();

            try
            {
                var document = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
                var exceptionElement = document.Element("EXCEPTION");
                var message = exceptionElement.Element("MESSAGE").Value;
                var debugInfo = exceptionElement.Element("DEBUGINFO").Value;

                moodlerError.Exception = exceptionElement.Attribute("class").Value;
                moodlerError.Message = message;
                moodlerError.DebugInfo = debugInfo;
            }
            catch(Exception ex)
            {
                moodlerError.Exception = ex.GetType().FullName;
                moodlerError.Message = ex.Message;
                moodlerError.DebugInfo = ex.StackTrace;
                MoodleErrorConvertException = ex;
            }

            return moodlerError;
        }

        private MoodleError GetMoodleError(string xmlOrJson)
        {
            try
            {
                if (xmlOrJson.IsXml())
                {
                    return GetMoodleErrorByXml(xmlOrJson);
                }

                return JsonConvert.DeserializeObject<MoodleError>(xmlOrJson);
            }
            catch(Exception ex)
            {
                MoodleError moodlerError = new MoodleError()
                {
                    Exception = ex.GetType().FullName,
                    Message = ex.Message,
                    DebugInfo = ex.StackTrace
                };
            
                MoodleErrorConvertException = ex;
                return moodlerError;
            }
        }
    }
}
