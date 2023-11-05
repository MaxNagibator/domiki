﻿using Domiki.Web.Business.Models;

namespace Domiki.Web.Business
{
    public class StaticEntities
    {
        public static List<ResourceType> ResourceTypes = new List<ResourceType>
            {
                new ResourceType { Id = 1, Name = "Золото", LogicName = "gold" },
                new ResourceType { Id = 2, Name = "Камень", LogicName = "stone" },
                new ResourceType { Id = 3, Name = "Дерево", LogicName = "wood" },
            };

        public static Dictionary<int, ResourceType> ResourceTypesDict = ResourceTypes.ToDictionary(x => x.Id, x => x);


        public static List<DomikType> DomikTypes = new List<DomikType>
            {
                new DomikType { Id = 1, Name = "Кузница", LogicName = "forge", MaxCount = 1,
                    UpgradeLevels = new UpgradeLevel[]
                    {
                        new UpgradeLevel
                        {
                            Value = 1,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 1 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 2,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 2 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 3,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 4 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 4,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 8 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 5,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 16 }
                            }
                        },
                    } },
                new DomikType { Id = 2, Name = "Барак", LogicName = "barracks", MaxCount = 5,
                    UpgradeLevels = new UpgradeLevel[]
                    {
                        new UpgradeLevel
                        {
                            Value = 1,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 1 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 2,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 2 }
                            }
                        },
                    } },
                new DomikType { Id = 3,
                    Name = "Каменоломня",
                    LogicName = "stone_mine",
                    MaxCount = 2,
                    UpgradeLevels = new UpgradeLevel[]
                    {
                        new UpgradeLevel
                        {
                            Value = 1,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 1 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 2,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 2 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 3,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 4 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 4,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 5 }
                            }
                        },
                    }
                },
                new DomikType { Id = 4, Name = "Золотой рудник", LogicName = "gold_mine", MaxCount = 2,
                    UpgradeLevels = new UpgradeLevel[]
                    {
                        new UpgradeLevel
                        {
                            Value = 1,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 1 }
                            }
                        },
                        new UpgradeLevel
                        {
                            Value = 2,
                            Resources = new Resource[]
                            {
                                new Resource { Type = ResourceTypesDict[1], Value = 2 }
                            }
                        },
                    } },
            };
    }
}