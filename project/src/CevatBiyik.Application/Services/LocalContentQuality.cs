using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using CevatBiyik.Application.Validation;
using CevatBiyik.Domain.Entities;
namespace CevatBiyik.Application.Services;
public static class LocalContentQuality
{
    // Project review heuristics, not search-engine thresholds or a substitute for human review.
    public static IReadOnlyList<string> Check(Location item, IReadOnlyList<Location> peers) =>
        CheckAll(peers.Any(x => x.Id == item.Id) ? peers : peers.Append(item).ToArray())[item.Id];
    public static IReadOnlyDictionary<long, IReadOnlyList<string>> CheckAll(IReadOnlyList<Location> peers)
    {
        var result = new Dictionary<long, IReadOnlyList<string>>();
        var names = peers.SelectMany(x => Tokens(x.Name)).Append("kayseri").ToHashSet(StringComparer.Ordinal);
        var bodies = peers.ToDictionary(x => x.Id, x => Tokens(RichText.ToPlainText(x.Body)));
        var details = peers.ToDictionary(x => x.Id, x => Tokens(x.LocalDetails));
        var shingles = peers.ToDictionary(x => x.Id, x => Shingles(bodies[x.Id].Concat(details[x.Id]).Where(w => !names.Contains(w)).ToArray()));
        foreach (var item in peers)
        {
            var warnings = new List<string>();
            if (!item.IsActive || !item.IsReviewed || !item.FactsVerified)
            {
                warnings.Add("Yayın, inceleme ve yerel bilgi doğrulaması gerekli.");
            }
            if (!ContentPublication.Complete(item.Name, item.Summary, item.Body, item.LocalDetails) ||
                !RichText.IsSafe(item.Body) || !PlainText.IsSafe(item.LocalDetails) || !PlainText.IsSafe(item.Summary) || !PlainText.IsSafe(item.Name))
            {
                warnings.Add("Eksik veya güvenli olmayan içerik.");
            }
            var body = bodies[item.Id]; var local = details[item.Id];
            if (body.Length < 150 || local.Length < 40 || body.Distinct().Count() < 60 || local.Distinct().Count() < 25)
            {
                warnings.Add("İçerik yeterliliği kontrolü: ana metin 150, yerel açıklama 40 kelime ve anlamlı çeşitlilik gerekli.");
            }
            var own = shingles[item.Id];
            foreach (var peer in peers.Where(x => x.Id != item.Id && x.IsActive && x.IsReviewed))
            {
                var other = shingles[peer.Id];
                var smaller = own.Count <= other.Count ? own : other; var larger = own.Count <= other.Count ? other : own;
                if (smaller.Count > 0 && smaller.Count(larger.Contains) / (double)smaller.Count >= 0.75)
                {
                    warnings.Add("Başka bir yerel sayfayla güçlü metin benzerliği; özgün yerel değer incelemesi gerekli.");
                    break;
                }
            }
            result[item.Id] = warnings;
        }
        return result;
    }
    private static HashSet<string> Shingles(string[] words) => Enumerable.Range(0, Math.Max(0, words.Length - 4))
        .Select(i => string.Join(' ', words.Skip(i).Take(5))).ToHashSet(StringComparer.Ordinal);
    private static string[] Tokens(string text)
    {
        var normalized = text.ToLower(CultureInfo.GetCultureInfo("tr-TR")).Normalize(NormalizationForm.FormD);
        var cleaned = new string(normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark).ToArray()).Replace('ı', 'i');
        return Regex.Matches(cleaned, @"[\p{L}\p{N}]+").Select(x => x.Value).ToArray();
    }
}
