namespace Domiki.Business.Models
{
    public class DomikType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogicName { get; set; }

        /// <summary>
        /// Максимальное количество для постройки.
        /// </summary>
        public int MaxCount { get; set; }

    }
}