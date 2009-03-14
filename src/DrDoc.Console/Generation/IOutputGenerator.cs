using System.Collections.Generic;

namespace DrDoc.Generation
{
    public interface IOutputGenerator
    {
        string Convert(string templateName, OutputData data);
    }
}