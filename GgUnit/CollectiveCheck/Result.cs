namespace GgUnit.CollectiveCheck
{
    public class Result
    {
        public Result(State state, string name, string detail)
        {
            this.State = state;
            this.Name = name;
            this.Detail = detail;
        }

        public State State { get; protected internal set; }

        public string Name { get; protected set; }

        public string Detail { get; protected set; }

        public override string ToString()
        {
            return string.Format("State: {0}, Name: {1}, Detail: {2}", this.State, this.Name, this.Detail);
        }
    }
}