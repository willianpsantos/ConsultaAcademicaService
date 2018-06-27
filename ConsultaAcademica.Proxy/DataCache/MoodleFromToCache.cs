using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsultaAcademica.Proxy
{
    public enum MoodleCachaValueType
    {
        Category,
        Course,
        User
    }

    public struct MoodleCacheValues
    {
        public long Modality { get; set; }

        public string ApiIdentifier { get; set; }

        public long MoodleId { get; set; }

        public MoodleCachaValueType Type { get; set; }
    }

    public static class MoodleFromToCache
    {
        public static ConcurrentBag<MoodleCacheValues> Categories;
        public static ConcurrentBag<MoodleCacheValues> Courses;
        public static ConcurrentBag<MoodleCacheValues> Users;


        public static void AddCategory(long modality, string apiCourseName, long moodleCategoryId)
        {
            if(Categories == null)
            {
                Categories = new ConcurrentBag<MoodleCacheValues>();
            }
            
            bool success = Categories.Where(x => x.ApiIdentifier == apiCourseName && x.Modality == modality).Count() > 0;

            if(success)
            {
                return;
            }

            MoodleCacheValues values = new MoodleCacheValues();
            values.Modality = modality;
            values.ApiIdentifier = apiCourseName;
            values.MoodleId = moodleCategoryId;
            values.Type = MoodleCachaValueType.Category;

            Categories.Add(values);
        }

        public static void AddCourse(long modality, string apiDisciplineName, long moodleCourseId)
        {
            if (Courses == null)
            {
                Courses = new ConcurrentBag<MoodleCacheValues>();
            }
            
            bool success = Courses.Where(x => x.ApiIdentifier == apiDisciplineName && x.Modality == modality).Count() > 0;

            if (success)
            {
                return;
            }

            MoodleCacheValues values = new MoodleCacheValues();
            values.Modality = modality;
            values.ApiIdentifier = apiDisciplineName;
            values.MoodleId = moodleCourseId;
            values.Type = MoodleCachaValueType.Course;

            Courses.Add(values);
        }

        public static void AddUser(long modality, string apiUsername, long moodleUserId)
        {
            if (Users == null)
            {
                Users = new ConcurrentBag<MoodleCacheValues>();
            }
            
            bool success = Users.Where(x => x.ApiIdentifier == apiUsername && x.Modality == modality).Count() > 0;

            if (success)
            {
                return;
            }

            MoodleCacheValues values = new MoodleCacheValues();
            values.Modality = modality;
            values.ApiIdentifier = apiUsername;
            values.MoodleId = moodleUserId;
            values.Type = MoodleCachaValueType.User;

            Users.Add(values);
        }


        public static long? GetCachedMoodleCategory(long modality, string apiCourseName)
        {
            if(Categories == null || Categories.Count == 0)
            {
                return null;
            }

            var success = Categories.Where(x => x.ApiIdentifier == apiCourseName && x.Modality == modality).FirstOrDefault();

            if(success.MoodleId == 0)
            {
                return null;
            }

            return success.MoodleId;
        }

        public static long? GetCachedMoodleCourse(long modality, string apiDisciplineName)
        {
            if (Courses == null || Courses.Count == 0)
            {
                return null;
            }

            var success = Courses.Where(x => x.ApiIdentifier == apiDisciplineName && x.Modality == modality).FirstOrDefault();

            if (success.MoodleId == 0)
            {
                return null;
            }

            return success.MoodleId;
        }

        public static long? GetCachedMoodleUser(long modality, string apiUsername)
        {
            if (Users == null || Users.Count == 0)
            {
                return null;
            }

            var success = Users.Where(x => x.ApiIdentifier == apiUsername && x.Modality == modality).FirstOrDefault();

            if (success.MoodleId == 0)
            {
                return null;
            }

            return success.MoodleId;
        }
    }
}
