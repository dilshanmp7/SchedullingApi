using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SchedulingApiLib;

namespace SchedulingApiTest
{
    [TestClass]
    public class SchedulingTest
    {
        private SchedulingModel _scheduledModel;
        [TestInitialize]
        public void InitializeTest()
        {
            _scheduledModel = new SchedulingModel();
        }
        #region Create person test cases 
        [TestMethod]
        public void CreatePersonMethodwithInvalidName()
        {
            ArgumentException ex = null;
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson(string.Empty, "user@gmail.com"));
            Assert.AreEqual("Invalid person name", ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson(null, "user@gmail.com"));
            Assert.AreEqual("Invalid person name", ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson("123456", "user@gmail.com"));
            Assert.AreEqual("Invalid person name", ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson("user12", "user@gmail.com"));
            Assert.AreEqual("Invalid person name", ex.Message);
        }

        [TestMethod]
        public void CreatePersonMethodwithInvalidEmailTest()
        {
            ArgumentException ex = null;
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson("User", "usergmail.com"));
            Assert.AreEqual("Invalid email", ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson("User", "user@gmailcom"));
            Assert.AreEqual("Invalid email",ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson("User", "user@gmail.k"));
            Assert.AreEqual("Invalid email", ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson("User", "user @gmail.com"));
            Assert.AreEqual("Invalid email", ex.Message);
        }

        [TestMethod]
        public void CreatePersonwithValidNameandEmail()
        {
            Assert.AreEqual(0, _scheduledModel.PersonList.Count);
            _scheduledModel.CreatePerson("User", "user@gmail.com");
            Assert.AreEqual(1, _scheduledModel.PersonList.Count);
            Assert.AreEqual("User", _scheduledModel.PersonList[0].Name);
            Assert.AreEqual("user@gmail.com", _scheduledModel.PersonList[0].Email);
        }

        [TestMethod]
        public void CreateExistingPerson()
        {
            Assert.AreEqual(0, _scheduledModel.PersonList.Count);
            _scheduledModel.CreatePerson("User", "user@gmail.com");
            Assert.AreEqual(1, _scheduledModel.PersonList.Count);
            var ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreatePerson("User A", "user@gmail.com"));
            Assert.AreEqual("email id already exists", ex.Message);
            Assert.AreEqual(1, _scheduledModel.PersonList.Count);
        }
        #endregion

        #region Create meeting test cases

        private void CreateRandomPersonList()
        {
            _scheduledModel.CreatePerson("User A", "userA@gmail.com");
            _scheduledModel.CreatePerson("User B", "userB@gmail.com");
            _scheduledModel.CreatePerson("User C", "userC@gmail.com");
        }

        [TestMethod]
        public void CreateMeetingWithoutPerson()
        {
            ArgumentException ex = null;
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreateAMeeting(_scheduledModel.PersonList.ToList(), 12));
            Assert.AreEqual("Invalid person allocation", ex.Message);
        }

        [TestMethod]
        public void CreateAMeetingForGivenTimeSlotInASpecificDay()
        {
            CreateRandomPersonList();
            Assert.AreEqual(0, _scheduledModel.MeetingList.Count);
            var selectedPersons = new List<Person>() { _scheduledModel.PersonList[0], _scheduledModel.PersonList[1] };
            _scheduledModel.CreateAMeeting(selectedPersons, 12);
            Assert.AreEqual(1, _scheduledModel.MeetingList.Count);
        }

        [TestMethod]
        public void CreateAMeetingForValidTimeSlotInASpecificDay()
        {
            ArgumentException ex = null;
            CreateRandomPersonList();
            Assert.AreEqual(0, _scheduledModel.MeetingList.Count);
            var selectedPersons = new List<Person>() { _scheduledModel.PersonList[0], _scheduledModel.PersonList[1] };
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreateAMeeting(selectedPersons, 0));
            Assert.AreEqual("Invalid time slot", ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreateAMeeting(selectedPersons, 24));
            Assert.AreEqual("Invalid time slot", ex.Message);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreateAMeeting(selectedPersons, -5));
            Assert.AreEqual("Invalid time slot", ex.Message);
            _scheduledModel.CreateAMeeting(selectedPersons, 12);
            Assert.AreEqual(1, _scheduledModel.MeetingList.Count);
        }

        [TestMethod]
        public void CreateAMeetingWithPersonConflict()
        {
            ArgumentException ex = null;
            CreateRandomPersonList();
            var firstSelection = new List<Person>() { _scheduledModel.PersonList[0] };
            _scheduledModel.CreateAMeeting(firstSelection, 9);
            ex = Assert.ThrowsException<ArgumentException>(() => _scheduledModel.CreateAMeeting(firstSelection, 9));
            Assert.AreEqual($"Given time 9 not avialble", ex.Message);
            Assert.AreEqual(1, _scheduledModel.MeetingList.Count);
        }

        #endregion

        #region ShowUpcomingMeeting

        [TestMethod]
        public void ShowUpComingMeetingsWithNull()
        {
            var scheduledMettingWithNUll = _scheduledModel.GetScheduledMettingByPerson(null);
            Assert.AreEqual(0, scheduledMettingWithNUll.Count);
        }
        [TestMethod]
        public void ShowUpComingMeetingsInASpecificDay()
        {
            CreateRandomPersonList();
            var firstSelection = new List<Person>() { _scheduledModel.PersonList[0] };
            _scheduledModel.CreateAMeeting(firstSelection, 9);
            _scheduledModel.CreateAMeeting(firstSelection, 13);
            var scheduledMeeting= _scheduledModel.GetScheduledMettingByPerson(_scheduledModel.PersonList[0].Email);
            Assert.AreEqual(2, scheduledMeeting.Count);
            Assert.AreEqual(firstSelection.First(), scheduledMeeting.First().InvolvedPersons.First());
            Assert.AreEqual(9, scheduledMeeting.First().StartTime);
            Assert.AreEqual(firstSelection.First(), scheduledMeeting.Last().InvolvedPersons.First());
            Assert.AreEqual(13, scheduledMeeting.Last().StartTime);
        }
        #endregion
        
        #region Available time for meetings given person
        [TestMethod]
        public void FindAvailableTimeSlotForGivenPerson()
        {
            CreateRandomPersonList();
            var selectedPerson = new List<Person>() { _scheduledModel.PersonList[0] };
            _scheduledModel.CreateAMeeting(selectedPerson, 9);
            _scheduledModel.CreateAMeeting(selectedPerson, 12);
            var availableTimeSlotsByPerson = _scheduledModel.GetAvailableTimeSlots(selectedPerson);
           
            Assert.AreEqual(1,availableTimeSlotsByPerson.Values.Count);
            Assert.AreEqual(22, availableTimeSlotsByPerson[selectedPerson.First()].Count());
            Assert.IsFalse(availableTimeSlotsByPerson[selectedPerson.First()].Contains(9));
            Assert.IsFalse(availableTimeSlotsByPerson[selectedPerson.First()].Contains(12));
        }
        [TestMethod]
        public void FindAvailableTimeSlotForGivenPersonGroup()
        {
            CreateRandomPersonList();
            var firstSelection = new List<Person>() { _scheduledModel.PersonList[0] };
            var secondSelection = new List<Person>() { _scheduledModel.PersonList[1], _scheduledModel.PersonList[0] };

            _scheduledModel.CreateAMeeting(firstSelection, 9);
            _scheduledModel.CreateAMeeting(firstSelection, 12);

            _scheduledModel.CreateAMeeting(secondSelection, 14);
            _scheduledModel.CreateAMeeting(secondSelection, 16);

            var availableTimeSlotsByPerson = _scheduledModel.GetAvailableTimeSlots(secondSelection);

            Assert.AreEqual(2, availableTimeSlotsByPerson.Count);

            Assert.AreEqual(20, availableTimeSlotsByPerson[firstSelection.First()].Count());
            Assert.IsFalse(availableTimeSlotsByPerson[firstSelection.First()].Contains(9));
            Assert.IsFalse(availableTimeSlotsByPerson[firstSelection.First()].Contains(12));
            Assert.IsFalse(availableTimeSlotsByPerson[firstSelection.First()].Contains(14));
            Assert.IsFalse(availableTimeSlotsByPerson[firstSelection.First()].Contains(16));

            Assert.AreEqual(22, availableTimeSlotsByPerson[secondSelection.First()].Count());
            Assert.IsFalse(availableTimeSlotsByPerson[secondSelection.First()].Contains(14));
            Assert.IsFalse(availableTimeSlotsByPerson[secondSelection.First()].Contains(16));
        }


        #endregion
    }
}
