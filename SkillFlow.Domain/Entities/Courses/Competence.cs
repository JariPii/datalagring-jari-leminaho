using SkillFlow.Domain.Courses;
using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Courses
{
    public class Competence : BaseEntity<CompetenceId>
    {
        private readonly List<Instructor> _instructors = new();
        //public CompetenceId Id { get; private set; }
        public CompetenceName Name { get; private set; }

        public Competence(CompetenceId id, CompetenceName name)
        {
            Id = id;
            Name = name;
        }

        protected Competence() { }

        public virtual IReadOnlyCollection<Instructor> Instructors => _instructors.AsReadOnly();

        public void UpdateCompetenceName(CompetenceName newName)
        {
            if (Name == newName) return;            

            Name = newName;
            UpdateTimeStamp();

        }

        public void AddInstructor(Instructor instructor)
        {
            ArgumentNullException.ThrowIfNull(instructor);

            if(!_instructors.Any(i => i.Id == instructor.Id))
            {
                _instructors.Add(instructor);
                UpdateTimeStamp();
            }
        }

    }
}
