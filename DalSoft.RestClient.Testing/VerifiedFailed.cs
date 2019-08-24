using System;

namespace DalSoft.RestClient.Testing
{
    internal class VerifiedFailed : Exception
    {
        public VerifiedFailed(string message) : base(message)
        {
            
        }
    }
}