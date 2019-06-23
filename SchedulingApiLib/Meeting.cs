using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace SchedulingApiLib
{
    public class Meeting
    {
        private List<Person> _persons;
        private int _startTime;

        public ReadOnlyCollection<Person> InvolvedPersons => _persons.AsReadOnly();
        public int StartTime => _startTime;
        public int EndTime => StartTime + 1;

        public Meeting(List<Person> persons, int startTime)
        {
            _persons = persons;
            _startTime = startTime;
        }

        internal bool ContainPerson(string email)
        {
            return _persons.Any(a => a.Email.Equals(email));
        }

        internal bool CheckAvailability(List<Person> persons, int startTime)
        {
            foreach (var person in persons)
            {
                if(ContainPerson(person.Email))
                {
                    if (_startTime == startTime)
                        return false;
                }
            }
            return true;
        }
    }
}