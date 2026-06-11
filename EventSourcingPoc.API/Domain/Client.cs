namespace EventSourcingPoc.API.Domain
{
    public record Rut(int Number, char Dv);
    public class Client
    {
        public string Id { get; private set; }
        public Rut Rut { get; private set; }
        public string Name { get; private set; }

    }
}
