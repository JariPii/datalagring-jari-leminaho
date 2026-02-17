using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.DTOs.Attendees
{
    public record AddCompetenceDTO(string CompetenceName, byte[] RowVersion);
}
