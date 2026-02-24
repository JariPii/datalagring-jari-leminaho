using System;
using System.Collections.Generic;
using System.Text;

namespace SkillFlow.Application.Abstractions.Caching
{
    public interface IAttendeeCacheBuster : ICacheBuster { }
    public interface ICourseCacheBuster : ICacheBuster { }
    public interface ILocationCacheBuster : ICacheBuster { }
    public interface ICompetenceCacheBuster : ICacheBuster { }
    public interface ICourseSessionCacheBuster : ICacheBuster { }
}
