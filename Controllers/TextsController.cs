using InfoInfo.Data;
using InfoInfo.Infrastructure;
using InfoInfo.Models;
using InfoInfo.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfoInfo.Controllers
{
    public class TextsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TextsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Texts (Publiczny Index z filtrowaniem i stronicowaniem)
        public async Task<IActionResult> Index(string Fraza, string Autor, int? Kategoria, int PageNumber = 1)
        {
            TextIndexViewModel textIndexViewModel = new()
            {
                TextList = new()
            };

            // Budowanie zapytania
            var SelectedTexts = _context.Texts
                .Include(t => t.Category)
                .Include(t => t.Author)
                .Where(t => t.Active == true)
                .OrderByDescending(t => t.AddedDate)
                .AsQueryable();

            // Filtrowanie
            if (Kategoria != null)
            {
                SelectedTexts = SelectedTexts.Where(r => r.Category.CategoryId == Kategoria);
            }
            if (!String.IsNullOrEmpty(Autor))
            {
                SelectedTexts = SelectedTexts.Where(r => r.Author.Id == Autor);
            }
            if (!String.IsNullOrEmpty(Fraza))
            {
                SelectedTexts = SelectedTexts.Where(r => r.Content.Contains(Fraza) || r.Title.Contains(Fraza));
            }

            // Ustawienie danych do ViewModel
            textIndexViewModel.TextList.TextCount = SelectedTexts.Count();
            textIndexViewModel.TextList.PageNumber = PageNumber;
            textIndexViewModel.TextList.Author = Autor;
            textIndexViewModel.TextList.Phrase = Fraza;
            textIndexViewModel.TextList.Category = Kategoria;

            // Pobranie danych dla strony
            textIndexViewModel.Texts = await SelectedTexts
                .Skip((PageNumber - 1) * textIndexViewModel.TextList.PageSize)
                .Take(textIndexViewModel.TextList.PageSize)
                .ToListAsync();

            // Dane do list rozwijalnych
            ViewData["Category"] = new SelectList(_context.Categories.Where(c => c.Active == true).OrderBy(c => c.Name), "CategoryId", "Name", Kategoria);
            ViewData["Author"] = new SelectList(_context.Texts.Include(u => u.Author).Select(u => u.Author).Distinct(), "Id", "FullName", Autor);

            return View(textIndexViewModel);
        }

        // GET: List (Dla Administratora - lista wszystkich tekstów)
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> List()
        {
            var applicationDbContext = _context.Texts.Include(t => t.Category).Include(t => t.Author);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Texts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var selectedText = await _context.Texts
                .Include(t => t.Category)
                .Include(t => t.Author)
                .Include(t => t.Opinions)
                .ThenInclude(c => c.Author)
                .Where(t => t.Active == true && t.TextId == id)
                .FirstOrDefaultAsync();

            if (selectedText == null)
            {
                return NotFound();
            }

            TextWithOpinions textWithOpinions = new()
            {
                SelectedText = selectedText,
                ReadingTime = (int)Math.Ceiling((double)selectedText.Content.Length / 1400),
                CommentsCount = selectedText.Opinions.Count,
                NewOpinion = new()
                {
                    TextId = selectedText.TextId,
                    UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                    Text = selectedText
                }
            };

            textWithOpinions.RatingsCount = textWithOpinions.CommentsCount > 0
                ? selectedText.Opinions.Count(x => x.Rating != null && x.Rating != Rating.Unrated) : 0;

            textWithOpinions.AverageRating = textWithOpinions.RatingsCount > 0
                ? (float)selectedText.Opinions.Where(x => x.Rating != null && x.Rating != Rating.Unrated).Average(x => (int)x.Rating!)
                : 0f;

            textWithOpinions.Description = Variety.Phrase("komentarz", "komentarze", "komentarzy", textWithOpinions.CommentsCount);

            return View(textWithOpinions);
        }

        // GET: Texts/Create
        [Authorize(Roles = "admin, author")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Texts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Create([Bind("TextId,Title,Summary,Keywords,Content,Graphic,Active,AddedDate,CategoryId")] Text text)
        {
            if (ModelState.IsValid)
            {
                text.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                text.AddedDate = DateTime.Now;
                _context.Add(text);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(List));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
            return View(text);
        }

        // GET: Texts/Edit/5
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var text = await _context.Texts.FindAsync(id);
            if (text == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
            return View(text);
        }

        // POST: Texts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, author")]
        public async Task<IActionResult> Edit(int id, [Bind("TextId,Title,Summary,Keywords,Content,Graphic,Active,AddedDate,CategoryId,UserId")] Text text)
        {
            if (id != text.TextId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(text);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TextExists(text.TextId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(List));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", text.CategoryId);
            return View(text);
        }

        // GET: Texts/Delete/5
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var text = await _context.Texts
                .Include(t => t.Category)
                .Include(t => t.Author)
                .FirstOrDefaultAsync(m => m.TextId == id);
            if (text == null)
            {
                return NotFound();
            }

            return View(text);
        }

        // POST: Texts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var text = await _context.Texts.FindAsync(id);
            if (text != null)
            {
                _context.Texts.Remove(text);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        private bool TextExists(int id)
        {
            return _context.Texts.Any(e => e.TextId == id);
        }
    }
}