namespace SchedulingApiLib
{
    public class Person
    {
        private string _name;
        private string _email;

        public string Name => _name;
        public string Email => _email;

        internal Person(string name, string email)
        {
            _name = name;
            _email = email;
        }
    }
}