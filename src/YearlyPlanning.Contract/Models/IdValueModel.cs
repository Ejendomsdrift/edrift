namespace YearlyPlanning.Contract.Models
{
    public class IdValueModel<TId, TValue>
    {
        public TId Id { get; set; }
        public TValue Value { get; set; }
    }
}
