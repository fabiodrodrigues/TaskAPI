namespace TaskAPI.Domain.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool Completed { get; set; }
    }
}
