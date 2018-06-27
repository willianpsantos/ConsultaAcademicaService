using System;
using System.Collections.Generic;
using System.Text;

namespace ConsultaAcademica.Core
{
    public enum MoodleTokenType
    {
        OfficialMoodleApiFunctions,
        LocalMoodleExternalApiGetInfoToken
    }

    public abstract class AbstractProxy
    {
        protected IAbstractService Service;        
        protected string MoodleBaseUrl;
        protected string MoodleServiceUrl;
        protected string MoodleToken;
        protected string MoodleGetInfoServiceToken;        
        protected volatile string LastUrl;

        public bool CanLog { get; set; }

        public AbstractProxy(IAbstractService abstractService)
        {
            Service = abstractService;
        }

        public AbstractProxy AddMoodleBaseUrl(string url)
        {
            MoodleBaseUrl = url;
            return this;
        }

        public AbstractProxy AddMoodleServiceUrl(string url)
        {
            MoodleServiceUrl = url;
            return this;
        }

        public AbstractProxy AddMoodleToken(string token)
        {
            MoodleToken = token;
            return this;
        }

        public AbstractProxy AddMoodleGetInfoServiceToken(string token)
        {
            MoodleGetInfoServiceToken = token;
            return this;
        }

        protected virtual void Log(string message)
        {
            if (!CanLog)
                return;

            System.Console.WriteLine(message);
        }

        public void BuildMoodleClient(AbstractMoodleServiceClient client, MoodleTokenType tokenType)
        {
            client.AddBaseUrl(MoodleBaseUrl).AddServiceUrl(MoodleServiceUrl);

            switch(tokenType)
            {
                case MoodleTokenType.OfficialMoodleApiFunctions:
                    client.AddToken(MoodleToken);
                    break;
                case MoodleTokenType.LocalMoodleExternalApiGetInfoToken:
                    client.AddToken(MoodleGetInfoServiceToken);
                    break;
            }     
        }       
    }
}
