namespace MessageBuilders.Interfaces
{
    using System.Collections.Generic;

    public interface IRequestCreator
    {
        IGeneralRestRequest Create(IDictionary<string, string> parameters);
    }
}