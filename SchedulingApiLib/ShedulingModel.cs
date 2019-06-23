using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.RegularExpressions;

namespace SchedulingApiLib
{
    public class SchedulingModel
    {
        private List<Person> _personList;
        private List<Meeting> _scheduledMeetings;

        public ReadOnlyCollection<Person> PersonList => _personList.AsReadOnly();

        public ReadOnlyCollection<Meeting> MeetingList => _scheduledMeetings.AsReadOnly();

        public SchedulingModel()
        {
            _personList = new List<Person>();
            _scheduledMeetings = new List<Meeting>();
        }
        
        public void CreatePerson(string name, string email)
        {
           if(string.IsNullOrEmpty(name) || !Regex.Match(name, "^[A-Z ][a-zA-Z ]*$").Success)
            {
                throw new ArgumentException("Invalid person name");
            }
           if(string.IsNullOrEmpty(email) || !Regex.IsMatch(email,
                @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
            {
                throw new ArgumentException("Invalid email");
            }
            if (!_personList.Any(a => a.Email.Equals(email)))
            {
                var newPerson = new Person(name, email);
                _personList.Add(newPerson);
            }
            else
                throw new ArgumentException("email id already exists");
        }

        public void CreateAMeeting(List<Person> persons, int startTime)
        {
            if(persons == null || !persons.Any())
            {
                throw new ArgumentException("Invalid person allocation");
            }
            if(!(startTime >= 1 && startTime <= 23))
            {
                throw new ArgumentException("Invalid time slot");
            }
            if (_scheduledMeetings.Count==0 || _scheduledMeetings.Any(a => a.CheckAvailability(persons, startTime)))
            {
                var newMeeting = new Meeting(persons, startTime);
                _scheduledMeetings.Add(newMeeting);
            }
            else
            {
                throw new ArgumentException($"Given time {startTime} not avialble");
            }
        }

        public List<Meeting> GetScheduledMettingByPerson(string email)
        {
            var scheduledMeeting = new List<Meeting>();
            foreach (var meeting in _scheduledMeetings)
            {
                if(meeting.ContainPerson(email))
                {
                    scheduledMeeting.Add(meeting);
                }
            }
            return scheduledMeeting;
        }

        public Dictionary<Person, IEnumerable<int>> GetAvailableTimeSlots(List<Person> selectedPerson)
        {
            var dicAvailableTimeByPerson = new Dictionary<Person, IEnumerable<int>>();
            foreach (var person in selectedPerson)
            {
                var availableTimeSlots = Enumerable.Range(1, 24).ToList();
                var scheduledMeetingsForaPerson= GetScheduledMettingByPerson(person.Email);
                var unavailableTimeSlots = new List<int>();
                foreach (var meeting in scheduledMeetingsForaPerson)
                {
                    unavailableTimeSlots.Add(meeting.StartTime);
                }
                dicAvailableTimeByPerson.Add(person, availableTimeSlots.Except(unavailableTimeSlots));
            }
            return dicAvailableTimeByPerson;
        }
    }
}
