using SkillFlow.Domain.Entities.Locations;

namespace SkillFlow.Domain.Exceptions
{
    public class LocationNotFoundException(LocationId id) : DomainException($"{id.Value} not found");

    public class LocationNameAllreadyExistsException(LocationName name) : DomainException($"{name.Value} allready exists");

    public class LocationHasCourseSessionException(LocationId id) : DomainException($"{id.Value} can not be deleted, it is tied to a course session");

    public class InvalidLocationNameException(string message) : DomainException($"Invalid location name: {message}");
}
