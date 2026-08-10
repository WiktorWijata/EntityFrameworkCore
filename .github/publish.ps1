param(
	[Parameter(Mandatory = $true)]
	[string]$V
)

$tag = "v$V"

$status = git status --porcelain
if ($status) {
    Write-Error "Są niezacommitowane zmiany. Zrób commit przed publikacją."
    exit 1
}

Write-Host "Wypychanie brancha..." -ForegroundColor Cyan
git push origin
if ($LASTEXITCODE -ne 0) {
    Write-Error "Nie udało się wypchnąć brancha."
    exit 1
}

Write-Host "Tworzenie taga $tag..." -ForegroundColor Cyan

git tag $tag
if ($LASTEXITCODE -ne 0) {
	Write-Error "Nie udało się utworzyć taga $tag."
	exit 1
}

git push origin $tag
if ($LASTEXITCODE -ne 0) {
	Write-Error "Nie udało się wypchnąć taga $tag."
	exit 1
}

Write-Host "Tag $tag został wypchnięty. GitHub Actions opublikuje paczkę NuGet." -ForegroundColor Green
