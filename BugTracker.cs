using System;
using System.Collections.Generic;
using System.Linq;

namespace LINQTasks
{
    public class BugTracker
    {
        private readonly List<Bug> _bugs = new List<Bug>();
        private readonly List<User> _users = new List<User>();

        public IReadOnlyCollection<User> Users => _users;
        public IReadOnlyCollection<Bug> Bugs => _bugs;


        public User CreateUser(string name, UserType userType)
        {
            var user = new User(name, userType);
            _users.Add(user);
            return user;
        }

        public Bug CreateBug(string info, User createdBy, Priority priority = Priority.Normal)
        {
            var bug = new Bug(info, createdBy, priority);
            _bugs.Add(bug);
            return bug;
        }

        /// <summary>
        /// Возвращает все открытые ошибки
        /// </summary>
        public IEnumerable<Bug> GetOpenBugs()
        {
            return Bugs.Where(x => x.Status != Status.Closed);
        }

        /// <summary>
        /// Возвращает все открытые ошибки с приоритетом не ниже priority
        /// </summary>
        public IEnumerable<Bug> GetOpenBugs(Priority priority)
        {
            return Bugs.Where(x => x.Status != Status.Closed && x.Priority >= priority);
        }

        /// <summary>
        /// Возвращает все ошибки назначенные на определенного пользователя 
        /// </summary>
        public IEnumerable<Bug> GetBugsByUser(User assignedTo)
        {
            return Bugs.Where(x => x.AssignedTo == assignedTo);
        }

        /// <summary>
        /// Возвращает ошибки сгруппированные по приоритету
        /// </summary>
        public IEnumerable<IGrouping<Priority, Bug>> GetBugsGroupeByPriority()
        {
            return Bugs.GroupBy(x => x.Priority);
        }

        /// <summary>
        /// Возвращается количество ошибок для каждого статуса
        /// </summary>
        public IEnumerable<Tuple<Status, int>> GetBugsCount()
        {
            return from b in Bugs
                   group b by b.Status into g
                   select new Tuple<Status, int>(g.Key, g.Count());
        }

        /// <summary>
        /// Возвращает все ошибки назначенные их создателю
        /// </summary>
        public IEnumerable<Bug> GetBugsAssignedToAuthor()
        {
            return Bugs.Where(x => x.CreatedBy == x.AssignedTo);
        }

        /// <summary>
        /// Возвращает пользователей на которых не назначена ни одна ошибка
        /// </summary>
        public IEnumerable<User> GetFreeUsers()
        {
            return from user in Users
                   join bug in Bugs on user equals bug.AssignedTo
                   into userBugs
                   where !userBugs.Any()
                   select user;

            //return from u in Users
            //       where u != (from b in Bugs
            //                   group Bugs by b.AssignedTo into g
            //                   select g.Key)
            //       select u;
        }

        /// <summary>
        /// Возвращает для каждого пользователя список назначенных ему ошибок
        /// Для пользоваетлеq на которых не назначено ни одной ошибки возвращается пустой список
        /// </summary>
        public IEnumerable<Tuple<User, IEnumerable<Bug>>> GetUsersBugs()
        {
            return from user in Users
                   join bug in Bugs on user equals bug.AssignedTo
                   into userBugs
                   select new Tuple<User, IEnumerable<Bug>>(user, userBugs);
            //return from u in Users
            //       select new Tuple<User, IEnumerable<Bug>>(u, from b in Bugs where b.AssignedTo == u select b);
        }

        /// <summary>
        /// Возвращает все ошибки отсортированные по статусу и приоритету (в рамках одинакового статуса)
        /// </summary>
        public IEnumerable<Bug> GetSortedBugs()
        {
            return from b in Bugs
                   orderby b.Status, b.Priority
                   select b;
        }
    }
}