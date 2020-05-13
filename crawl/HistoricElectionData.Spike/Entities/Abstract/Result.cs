using System.Collections.Generic;

namespace HistoricElectionData.Spike.Entities.Abstract
{
    public abstract class Result<T> where T : Summary, new()
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public T Summary { get; set; } = new T();

        public List<Candidate> Results { get; set; } = new List<Candidate>();

        public virtual bool IsEmpty()
        {
            return (Results == null || Results.Count == 0)
                && (Summary == null || Summary.IsEmpty());
        }
    }
}