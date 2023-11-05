﻿namespace Domiki.Web.Business.Models
{
    public class DomikType
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LogicName { get; set; }

        /// <summary>
        /// Максимальное количество построек данного типа.
        /// </summary>
        public int MaxCount { get; set; }

        /// <summary>
        /// Максимальный уровень, до которого можно улучший.
        /// </summary>
        public int MaxLevel => UpgradeLevels.Length;

        public UpgradeLevel[] UpgradeLevels { get; set; }
    }
}