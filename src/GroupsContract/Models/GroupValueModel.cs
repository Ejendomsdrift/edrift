namespace GroupsContract.Models
{
    public class GroupValueModel<TId, TValue>
    {
        public TId GroupId { get; set; }
        public TValue Value { get; set; }
    }
}
