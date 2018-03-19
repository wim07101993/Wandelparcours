using System;
using MongoDB.Bson;
using WebService.Services.Data;

namespace WebService.Services.Exceptions
{
    public interface IThrow
    {
        void PropertyNotKnown<T>(string propertyName);
        void NullArgument(string parameterName);

        void WrongTypeArgument(Type propertyType, Type valueType);

        void NotFound<T>(int tag);
        void NotFound<T>(string id);
        void NotFound<T>(ObjectId id);

        void MediaTypeNotFound<T>(string passedValue);

        void Database<T>(EDatabaseMethod databaseMethod);
        void Database<T>(EDatabaseMethod databaseMethod, string id);
        void Database<T>(EDatabaseMethod databaseMethod, ObjectId id);

        void FileToLarge(int maxSize);

        void Exception(string message = null);
    }
}