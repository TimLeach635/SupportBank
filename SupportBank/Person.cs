namespace SupportBank
{
    public class Person
    {
        public string Name { get; set; }

        public bool Equals(Person person)
        {
            return Name == person.Name;
        }
    }
}