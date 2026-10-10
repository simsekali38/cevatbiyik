using System.Text;
using CevatBiyik.Application.Interfaces;
using CevatBiyik.Application.Services;
using CevatBiyik.Domain.Entities;
using CevatBiyik.Infrastructure.Media;
using FluentValidation;

namespace CevatBiyik.UnitTests;
public sealed class FileLibraryTests
{
    public static byte[] Pdf(string extra = "") => CevatBiyik.Tests.PdfFixture.Create(extra);
    [Theory]
    [InlineData("/JavaScript")]
    [InlineData("/J#53")]
    [InlineData("/EmbeddedFile")]
    [InlineData("/Encrypt")]
    [InlineData("/ObjStm")]
    [InlineData("/Launch")]
    [InlineData("/OpenAction")]
    [InlineData("/AA")]
    [InlineData("/AcroForm")]
    [InlineData("/XFA")]
    [InlineData("/URI")]
    [InlineData("/GoToR")]
    [InlineData("/RichMedia")]
    [InlineData("/SubmitForm")]
    [InlineData("/J#61vaScript")]
    [InlineData("/La#75nch")]
    public void ActivePdfContent_IsRejected(string content) => Assert.Throws<ValidationException>(() => PdfPolicy.Validate(Pdf(content)));
    [Theory]
    [InlineData("x.pdf:run.exe")]
    [InlineData("x.pdf\u0000")]
    [InlineData("x.pdf ")]
    public void InvalidNames_AreRejected(string name) => Assert.Throws<ValidationException>(() => SafeFilePaths.Extension(name));
    [Fact]
    public async Task PrivateStore_RejectsUnsafeContentAndFailedScan_AndChecksDownloadIntegrity()
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-private-test-" + Guid.NewGuid().ToString("N"));
        var scanner = new TestScanner();
        var store = new FileDocumentStorage(root, Path.Combine(root, "public"), scanner);
        try
        {
            await Assert.ThrowsAsync<ValidationException>(() => store.StoreAsync(new MemoryStream(Pdf()), "x.exe", "application/pdf"));
            await Assert.ThrowsAsync<ValidationException>(() => store.StoreAsync(new MemoryStream(Pdf()), "x.pdf", "image/png"));
            await Assert.ThrowsAsync<ValidationException>(() => store.StoreAsync(new MemoryStream(Encoding.ASCII.GetBytes("MZ fake")), "x.pdf", "application/pdf"));
            await Assert.ThrowsAsync<ValidationException>(() => store.StoreAsync(new MemoryStream(new byte[FileImageStorage.MaxBytes + 1]), "x.pdf", "application/pdf"));
            scanner.Clean = false;
            await Assert.ThrowsAsync<ValidationException>(() => store.StoreAsync(new MemoryStream(Pdf()), "x.pdf", "application/pdf"));
            Assert.Empty(Directory.GetFiles(Path.Combine(root, "documents")));
            scanner.Clean = true;
            var stored = await store.StoreAsync(new MemoryStream(Pdf()), "../../original.pdf", "application/pdf");
            Assert.DoesNotContain("original", stored.StoragePath, StringComparison.Ordinal);
            await using (var opened = await store.OpenVerifiedAsync(stored.StoragePath, stored.Sha256))
            {
                Assert.Equal(Pdf().Length, opened.Length);
            }

            await File.AppendAllTextAsync(Path.Combine(root, stored.StoragePath.Replace('/', Path.DirectorySeparatorChar)), "tampered");
            await Assert.ThrowsAsync<ValidationException>(() => store.OpenVerifiedAsync(stored.StoragePath, stored.Sha256));
        }
        finally
        {
            if (Directory.Exists(Path.Combine(root, "documents")))
            {
                foreach (var file in Directory.GetFiles(Path.Combine(root, "documents")))
                {
                    File.Delete(file);
                }

                Directory.Delete(Path.Combine(root, "documents"));
            }
            if (Directory.Exists(root))
            {
                Directory.Delete(root);
            }
        }
    }
    [Fact]
    public void PrivateRoot_CannotBeUnderPublicRoot() => Assert.Throws<InvalidOperationException>(() => new FileDocumentStorage("C:/public/docs", "C:/public", new TestScanner()));
    [Fact]
    public async Task UnconfiguredScanner_FailsClosed()
    {
        using var scanner = new DefenderScanner(null);
        Assert.False(await scanner.IsCleanAsync("unused.pdf"));
    }
    [Fact]
    public async Task OrphanQuarantine_IsReversible_RejectsFreshFilesAndTraversal_AndIncludesPartialUploads()
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-orphans-test-" + Guid.NewGuid().ToString("N"));
        var privateRoot = Path.Combine(root, "private");
        var images = Path.Combine(root, "images");
        Directory.CreateDirectory(images);
        var key = "images/" + new string('a', 32) + ".webp.tmp";
        var full = Path.Combine(root, key.Replace('/', Path.DirectorySeparatorChar));
        var files = new ManagedFiles(root, privateRoot, TimeProvider.System);
        try
        {
            await File.WriteAllBytesAsync(full, new byte[] { 1, 2, 3 });
            Assert.Single(files.List(false, null));
            await Assert.ThrowsAsync<ValidationException>(() => files.QuarantineAsync(false, key, default));
            await Assert.ThrowsAsync<ValidationException>(() => files.QuarantineAsync(false, "../secret", default));
            File.SetLastWriteTimeUtc(full, DateTime.UtcNow.AddDays(-3));
            await files.QuarantineAsync(false, key, default);
            var quarantined = Path.Combine(privateRoot, "quarantine", "images", Path.GetFileName(full));
            Assert.False(File.Exists(full));
            Assert.True(File.Exists(quarantined));
            File.Move(quarantined, full);
            Assert.Equal(new byte[] { 1, 2, 3 }, await File.ReadAllBytesAsync(full));
        }
        finally
        {
            File.Delete(full);
            foreach (var folder in new[] { Path.Combine(privateRoot, "quarantine", "images"), Path.Combine(privateRoot, "quarantine"), privateRoot, images, root })
            {
                if (Directory.Exists(folder))
                {
                    Directory.Delete(folder);
                }
            }
        }
    }
    [Fact]
    public void FileScan_PagesWithoutRepeatingEntries()
    {
        var root = Path.Combine(Path.GetTempPath(), "cevat-pages-test-" + Guid.NewGuid().ToString("N"));
        var folder = Path.Combine(root, "images");
        Directory.CreateDirectory(folder);
        try
        {
            for (var i = 0; i < 105; i++)
            {
                File.WriteAllBytes(Path.Combine(folder, i.ToString("x32", System.Globalization.CultureInfo.InvariantCulture) + ".webp"), new byte[] { 1 });
            }

            var files = new ManagedFiles(root, Path.Combine(root, "private"), TimeProvider.System);
            var first = files.List(false, null);
            var second = files.List(false, first[^1].Path);
            Assert.Equal(100, first.Count);
            Assert.Equal(5, second.Count);
            Assert.Empty(first.Select(x => x.Path).Intersect(second.Select(x => x.Path), StringComparer.Ordinal));
        }
        finally
        {
            foreach (var file in Directory.GetFiles(folder))
            {
                File.Delete(file);
            }

            Directory.Delete(folder); Directory.Delete(root);
        }
    }
    [Fact]
    public async Task DocumentBusinessRules_SoftDeleteAndRestore_RequireVerifiedFile()
    {
        var catalog = new TestFileCatalog();
        var store = new TestDocumentStorage();
        var service = new DocumentService(new FakeRepository<PrivateDocument>(), catalog, store);
        await service.DeleteAsync(1);
        Assert.True(catalog.Document.IsDeleted);
        await Assert.ThrowsAsync<CevatBiyik.Application.Exceptions.NotFoundException>(() => service.DownloadAsync(1));
        store.Valid = false;
        await Assert.ThrowsAsync<ValidationException>(() => service.RestoreAsync(1));
        Assert.True(catalog.Document.IsDeleted);
        store.Valid = true;
        await service.RestoreAsync(1);
        Assert.False(catalog.Document.IsDeleted);
        Assert.True(catalog.Document.IsActive);
    }
    [Fact]
    public async Task Maintenance_DoesNotQuarantineKnownOrSoftDeletedRecords()
    {
        var catalog = new TestFileCatalog();
        var files = new TestManagedFiles();
        var maintenance = new FileMaintenance(catalog, files);
        var report = await maintenance.ReportAsync(true);
        Assert.Single(report.Orphans);
        Assert.Single(report.Missing);
        await Assert.ThrowsAsync<ValidationException>(() => maintenance.QuarantineAsync(true, "known"));
        Assert.False(files.Moved);
        await maintenance.QuarantineAsync(true, "orphan");
        Assert.True(files.Moved);
    }
    [Fact]
    public async Task ImageRecovery_RejectsMissingFilesBeforeReactivatingRecord()
    {
        var catalog = new TestFileCatalog { Media = new MediaAsset { Id = 1, IsDeleted = true, IsActive = false } };
        var files = new TestManagedFiles();
        var maintenance = new FileMaintenance(catalog, files);
        await Assert.ThrowsAsync<ValidationException>(() => maintenance.RestoreImageAsync(1));
        Assert.True(catalog.Media.IsDeleted);
        files.Existing = true;
        await maintenance.RestoreImageAsync(1);
        Assert.False(catalog.Media.IsDeleted);
        Assert.True(catalog.Media.IsActive);
    }
}
public sealed class TestScanner : IMalwareScanner
{
    public bool Clean { get; set; } = true;
    public Task<bool> IsCleanAsync(string fullPath, CancellationToken ct = default) => Task.FromResult(Clean);
}
public sealed class TestDocumentStorage : IDocumentStorage
{
    public bool Valid { get; set; } = true;
    public Task<StoredDocument> StoreAsync(Stream content, string fileName, string contentType, CancellationToken ct = default) => Task.FromResult(new StoredDocument("documents/" + new string('a', 32) + ".pdf", new string('A', 64), 10));
    public Task<Stream> OpenVerifiedAsync(string path, string sha256, CancellationToken ct = default) => Valid ? Task.FromResult<Stream>(new MemoryStream()) : throw new ValidationException("Test integrity failure.");
    public Task DeleteAsync(string path, CancellationToken ct = default) => Task.CompletedTask;
}
public sealed class TestFileCatalog : IFileCatalog
{
    public PrivateDocument Document { get; } = new() { Id = 1, IsActive = true };
    public MediaAsset? Media { get; init; }
    public Task<PrivateDocument?> DocumentAsync(long id, CancellationToken ct) => Task.FromResult<PrivateDocument?>(Document);
    public Task<MediaAsset?> MediaAsync(long id, CancellationToken ct) => Task.FromResult(Media);
    public Task<IReadOnlyList<DocumentDto>> DocumentsAsync(bool deleted, int page, CancellationToken ct) => Task.FromResult<IReadOnlyList<DocumentDto>>(Array.Empty<DocumentDto>());
    public Task<IReadOnlyList<FileRecord>> RecordsAsync(bool documents, long afterId, CancellationToken ct) => Task.FromResult<IReadOnlyList<FileRecord>>(new[] { new FileRecord(1, "known", true) });
    public Task<IReadOnlySet<string>> KnownAsync(bool documents, IReadOnlyList<string> paths, CancellationToken ct) => Task.FromResult<IReadOnlySet<string>>(paths.Where(x => x == "known").ToHashSet(StringComparer.Ordinal));
    public Task SaveAsync(CancellationToken ct) => Task.CompletedTask;
}
public sealed class TestManagedFiles : IManagedFiles
{
    public bool Existing { get; set; }
    public bool Moved { get; private set; }
    public bool Exists(bool documents, string path) => Existing;
    public IReadOnlyList<FileEntry> List(bool documents, string? afterPath) => new[] { new FileEntry("known", DateTime.UtcNow), new FileEntry("orphan", DateTime.UtcNow) };
    public Task QuarantineAsync(bool documents, string path, CancellationToken ct) { Moved = true; return Task.CompletedTask; }
}
