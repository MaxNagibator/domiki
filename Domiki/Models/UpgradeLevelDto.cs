namespace Domiki.Web.Models
{
    public class UpgradeLevelDto
    {
        /// <summary>
        /// Значение уровня.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Сколько нужно ресурсов для перехода на этот уровень.
        /// </summary>
        public ResourceDto[] Resources { get; set; }
    }
}