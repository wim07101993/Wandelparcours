using System;
using System.Linq;
using MongoDB.Bson;
using WebService.Helpers.Exceptions;
using WebService.Models;
using WebService.Services.Data;

namespace WebService.Services.Exceptions
{
    public class Throw : IThrow
    {
        public void PropertyNotKnown<T>(string propertyName)
            => throw new PropertyNotFoundException(
                $"the property with name {propertyName} could not be found on {typeof(T).Name}");

        public void NullArgument(string parameterName)
            => throw new WebArgumentNullException($"The argument {parameterName} cannot be null", parameterName);

        public void WrongTypeArgument(Type propertyType, Type valueType)
        {
            if (valueType == null)
                throw new WrongArgumentTypeException(
                    $"Cannot assign value to property of type {propertyType.Name}",
                    propertyType, null);

            throw new WrongArgumentTypeException(
                $"Type value of type {valueType.Name} is not assignable to {propertyType.Name}",
                propertyType,
                valueType);
        }

        public void NotFound<T>(int tag)
            => throw new NotFoundException($"There was no {typeof(T).Name} found with the given tag {tag}");

        public void NotFound<T>(string id)
            => throw new NotFoundException($"There was no {typeof(T).Name} found with the given id {id}");

        public void NotFound<T>(ObjectId id)
            => NotFound<T>(id.ToString());

        public void MediaTypeNotFound<T>(string passedValue)
            => throw new NotFoundException($"{passedValue} was not found in {typeof(T).Name}");

        public void Database<T>(EDatabaseMethod databaseMethod)
            => throw new DatabaseException(
                $"The database could not {databaseMethod.ToString().ToLower()} a {typeof(T).Name}", databaseMethod);

        public void Database<T>(EDatabaseMethod databaseMethod, string id)
            => throw new DatabaseException(
                $"The database could not {databaseMethod.ToString().ToLower()} the {typeof(T).Name} with id {id}",
                databaseMethod);

        public void Database<T>(EDatabaseMethod databaseMethod, ObjectId id)
            => Database<T>(databaseMethod, id.ToString());

        public void FileToLarge(int maxSize)
            => throw new WebArgumentException($"The passed file is to large, the max file size is {maxSize} bytes");

        public void Exception(string message = null)
            => throw new Exception(message);

        public void Unauthorized(params EAuthLevel[] minAuthLevels)
            => throw new UnauthorizedException(
                $"You need to ask the {minAuthLevels.Select(x => x.ToString()).ToJson()} for acces", minAuthLevel);

        public void Unauthorized()
            => throw new UnauthorizedException($"You need to ask the server for a token to get access");
    }
}