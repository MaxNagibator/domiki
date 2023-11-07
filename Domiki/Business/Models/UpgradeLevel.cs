namespace Domiki.Web.Business.Models
{
    public class UpgradeLevel
    {
        /// <summary>
        /// Значение уровня.
        /// </summary>
        public int Value { get; set; }

        /// <summary>
        /// Сколько нужно секунд для перехода на этот уровень.
        /// </summary>
        public int UpgradeSeconds { get; set; }

        /// <summary>
        /// Сколько нужно ресурсов для перехода на этот уровень.
        /// </summary>
        public Resource[] Resources { get; set; }
    }
}