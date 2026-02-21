using SkillFlow.Domain.Entities.Attendees;
using SkillFlow.Domain.Interfaces;
using SkillFlow.Domain.Primitives;

namespace SkillFlow.Domain.Entities.Competences
{
    public class Competence : BaseEntity<CompetenceId>, IAggregateRoot
    {
        private readonly List<Instructor> _instructors = new();
        public CompetenceName Name { get; private set; }

        protected Competence(CompetenceId id, CompetenceName name)
        {
            Id = id;
            Name = name;
        }

        protected Competence() { }

        public static Competence Create(CompetenceName name)
        {
            return new Competence(CompetenceId.New(), name);
        }

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
