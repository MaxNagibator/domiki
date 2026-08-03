using Domiki.Web.Data;
using Domiki.Web.Data.Entities;
using Domiki.Web.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Domiki.Web.Infrastructure;

public class PlayerEventManager
{
    private readonly ApplicationDbContext _context;

    public PlayerEventManager(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Record(int playerId, PlayerEventType type, object payload)
    {
        _context.PlayerEvents.Add(new()
        {
            PlayerId = playerId,
            Type = type,
            Date = DateTimeHelper.GetNowDate(),
            Data = JsonSerializer.Serialize(payload),
        });
    }

    public void RecordManufactureFinished(int playerId, int domikTypeId, Dictionary<int, int> producedByResourceTypeId)
    {
        var events = _context.PlayerEvents.Where(x => x.PlayerId == playerId && !x.Read && x.Type == PlayerEventType.ManufactureFinished).ToList();
        foreach (var playerEvent in events)
        {
            var payload = JsonSerializer.Deserialize<ManufactureFinishedPayload>(playerEvent.Data);
            if (payload?.DomikTypeId != domikTypeId)
            {
                continue;
            }

            foreach (var produced in producedByResourceTypeId)
            {
                var resource = payload.Resources.FirstOrDefault(x => x.ResourceTypeId == produced.Key);
                if (resource == null)
                {
                    payload.Resources.Add(new()
                    {
                        ResourceTypeId = produced.Key,
                        Value = produced.Value,
                    });
                }
                else
                {
                    resource.Value += produced.Value;
                }
            }

            payload.Cycles = Math.Max(1, payload.Cycles) + 1;
            playerEvent.Data = JsonSerializer.Serialize(payload);
            playerEvent.Date = DateTimeHelper.GetNowDate();
            return;
        }

        _context.PlayerEvents.Add(new()
        {
            PlayerId = playerId,
            Type = PlayerEventType.ManufactureFinished,
            Date = DateTimeHelper.GetNowDate(),
            Data = JsonSerializer.Serialize(new ManufactureFinishedPayload
            {
                DomikTypeId = domikTypeId,
                Resources = producedByResourceTypeId.Select(x => new ManufactureFinishedResourcePayload
                    {
                        ResourceTypeId = x.Key,
                        Value = x.Value,
                    })
                    .ToList(),
                Cycles = 1,
            }),
        });
    }

    /// <summary>
    /// Записывает в журнал итог кормления трудяги Корчмой после смены.
    /// </summary>
    /// <remarks>
    /// Сливает событие с уже существующим непрочитанным <see cref="PlayerEventType.WorkerMeal"/> с той же причиной
    /// <paramref name="reason"/>: счётчик и ресурсы суммируются, а имя и род трудяги обезличиваются (обнуляются).
    /// Новое (несливаемое) событие получает <c>variant</c> – случайный индекс текстового варианта.
    /// </remarks>
    /// <param name="playerId">Идентификатор игрока.</param>
    /// <param name="workerName">Имя накормленного трудяги; <see langword="null"/>, если кормление не состоялось.</param>
    /// <param name="workerGender">Грамматический род имени трудяги (<see cref="Workers.WorkerGender"/>); <see langword="null"/>, если кормление не состоялось.</param>
    /// <param name="reason"><see langword="null"/> – трудяга поел; <c>"forbidden"</c> – вся еда заповедана в кладовой; <c>"empty"</c> – еды нет вовсе.</param>
    /// <param name="resourcesByTypeId">Съеденные ресурсы по идентификатору типа; пустой словарь, если кормление не состоялось.</param>
    public void RecordWorkerMeal(int playerId, string? workerName, int? workerGender, string? reason, Dictionary<int, int> resourcesByTypeId)
    {
        var events = _context.PlayerEvents.Where(x => x.PlayerId == playerId && !x.Read && x.Type == PlayerEventType.WorkerMeal).ToArray()
            .Union(_context.PlayerEvents.Local.Where(x => x.PlayerId == playerId && !x.Read && x.Type == PlayerEventType.WorkerMeal))
            .ToList();
        foreach (var playerEvent in events)
        {
            var payload = JsonSerializer.Deserialize<WorkerMealPayload>(playerEvent.Data);
            if (payload == null || payload.Reason != reason)
            {
                continue;
            }

            foreach (var resource in resourcesByTypeId)
            {
                var existing = payload.Resources.FirstOrDefault(x => x.ResourceTypeId == resource.Key);
                if (existing == null)
                {
                    payload.Resources.Add(new()
                    {
                        ResourceTypeId = resource.Key,
                        Value = resource.Value,
                    });
                }
                else
                {
                    existing.Value += resource.Value;
                }
            }

            payload.Count++;
            payload.WorkerName = null;
            payload.WorkerGender = null;
            playerEvent.Data = JsonSerializer.Serialize(payload);
            playerEvent.Date = DateTimeHelper.GetNowDate();
            return;
        }

        _context.PlayerEvents.Add(new()
        {
            PlayerId = playerId,
            Type = PlayerEventType.WorkerMeal,
            Date = DateTimeHelper.GetNowDate(),
            Data = JsonSerializer.Serialize(new WorkerMealPayload
            {
                Count = 1,
                WorkerName = workerName,
                WorkerGender = workerGender,
                Variant = reason == null ? Random.Shared.Next(8) : 0,
                Reason = reason,
                Resources = resourcesByTypeId.Select(x => new WorkerMealResourcePayload
                    {
                        ResourceTypeId = x.Key,
                        Value = x.Value,
                    })
                    .ToList(),
            }),
        });
    }

    public RecapModel TakeRecap(int playerId, DateTime now)
    {
        var events = _context.PlayerEvents.AsNoTracking().Where(x => x.PlayerId == playerId && !x.Read).OrderBy(x => x.Date).Take(500).ToList();
        var lastSeen = _context.Players.AsNoTracking().Where(x => x.Id == playerId).Select(x => x.LastSeen).FirstOrDefault();
        if (events.Count > 0)
        {
            var ids = events.Select(x => x.Id).ToList();
            _context.PlayerEvents.Where(x => ids.Contains(x.Id)).ExecuteUpdate(s => s.SetProperty(x => x.Read, true));
        }

        var keepIds = _context.PlayerEvents.Where(x => x.PlayerId == playerId).OrderByDescending(x => x.Date).ThenByDescending(x => x.Id).Take(50).Select(x => x.Id);
        _context.PlayerEvents.Where(x => x.PlayerId == playerId && x.Read && !keepIds.Contains(x.Id)).ExecuteDelete();

        _context.Players.Where(x => x.Id == playerId).ExecuteUpdate(s => s.SetProperty(x => x.LastSeen, now));

        return new()
        {
            AwaySeconds = lastSeen == null ? 0 : Math.Max(0, (int)(now - lastSeen.Value).TotalSeconds),
            Events = events.Select(x => new RecapEventModel
                {
                    Type = x.Type,
                    Date = x.Date,
                    Data = JsonSerializer.Deserialize<JsonElement>(x.Data),
                })
                .ToList(),
        };
    }

    public List<RecapEventModel> GetRecentEvents(int playerId, int count = 30)
    {
        return _context.PlayerEvents.AsNoTracking()
            .Where(x => x.PlayerId == playerId)
            .OrderByDescending(x => x.Date)
            .ThenByDescending(x => x.Id)
            .Take(count)
            .ToList()
            .Select(x => new RecapEventModel
            {
                Type = x.Type,
                Date = x.Date,
                Data = JsonSerializer.Deserialize<JsonElement>(x.Data),
            })
            .ToList();
    }

    private sealed class ManufactureFinishedPayload
    {
        [JsonPropertyName("domikTypeId")]
        public int DomikTypeId { get; set; }

        [JsonPropertyName("resources")]
        public List<ManufactureFinishedResourcePayload> Resources { get; set; } = new();

        [JsonPropertyName("cycles")]
        public int Cycles { get; set; }
    }

    private sealed class ManufactureFinishedResourcePayload
    {
        [JsonPropertyName("resourceTypeId")]
        public int ResourceTypeId { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }

    private sealed class WorkerMealPayload
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("workerName")]
        public string? WorkerName { get; set; }

        [JsonPropertyName("workerGender")]
        public int? WorkerGender { get; set; }

        [JsonPropertyName("variant")]
        public int Variant { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("resources")]
        public List<WorkerMealResourcePayload> Resources { get; set; } = new();
    }

    private sealed class WorkerMealResourcePayload
    {
        [JsonPropertyName("resourceTypeId")]
        public int ResourceTypeId { get; set; }

        [JsonPropertyName("value")]
        public int Value { get; set; }
    }
}
